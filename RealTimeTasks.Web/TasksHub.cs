using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using RealTimeTasks.Data;

namespace RealTimeTasks.Web
{
    public class TasksHub : Hub
    {
        public void NewTask(string title)
        {
            var taskRepo = new TasksRepository(Properties.Settings.Default.ConStr);
            var task = new TaskItem { Title = title, IsCompleted = false };
            taskRepo.AddTask(task);
            SendTasks();
        }

        private void SendTasks()
        {
            var taskRepo = new TasksRepository(Properties.Settings.Default.ConStr);
            var tasks = taskRepo.GetActiveTasks();
            Clients.All.renderTasks(tasks.Select(t => new
            {
                Id = t.Id,
                Title = t.Title,
                HandledBy = t.HandledBy,
                UserDoingIt = t.User != null ? $"{t.User.FirstName} {t.User.LastName}" : null,
            }));
        }

        public void GetAll()
        {
            SendTasks();
        }

        public void SetDoing(int taskId)
        {
            var userRepo = new UserRepository(Properties.Settings.Default.ConStr);
            var user = userRepo.GetByEmail(Context.User.Identity.Name);
            var taskRepo = new TasksRepository(Properties.Settings.Default.ConStr);
            taskRepo.SetDoing(taskId, user.Id);
            SendTasks();
        }

        public void SetDone(int taskId)
        {
            var taskRepo = new TasksRepository(Properties.Settings.Default.ConStr);
            taskRepo.SetCompleted(taskId);
            SendTasks();
        }
    }
}