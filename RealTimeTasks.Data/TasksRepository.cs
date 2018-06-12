using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeTasks.Data
{
    public class TasksRepository
    {
        private readonly string _connectionString;

        public TasksRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddTask(TaskItem task)
        {
            using (var context = new RealTimeTasksDataContext(_connectionString))
            {
                context.TaskItems.InsertOnSubmit(task);
                context.SubmitChanges();
            }
        }

        public IEnumerable<TaskItem> GetActiveTasks()
        {
            using (var context = new RealTimeTasksDataContext(_connectionString))
            {
                var loadOptions = new DataLoadOptions();
                loadOptions.LoadWith<TaskItem>(t => t.User);
                context.LoadOptions = loadOptions;
                return context.TaskItems.Where(t => !t.IsCompleted).ToList();
            }
        }

        public void SetDoing(int taskId, int userId)
        {
            using (var context = new RealTimeTasksDataContext(_connectionString))
            {
                context.ExecuteCommand("UPDATE TaskItems SET HandledBy = {0} WHERE Id = {1}",
                    userId, taskId);
            }
        }

        public void SetCompleted(int taskId)
        {
            using (var context = new RealTimeTasksDataContext(_connectionString))
            {
                context.ExecuteCommand("UPDATE TaskItems SET IsCompleted = 1 WHERE Id = {0}",
                    taskId);
            }
        }
    }
}
