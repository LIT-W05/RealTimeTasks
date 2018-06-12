using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using RealTimeTasks.Data;
using RealTimeTasks.Web.Models;

namespace RealTimeTasks.Web.Controllers
{
    public class Foo
    {
        public List<Bar> Bars { get; set; }
    }

    public class Bar
    {
        public Foo Foo { get; set; }
    }

    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            var repo = new UserRepository(Properties.Settings.Default.ConStr);
            var user = repo.GetByEmail(User.Identity.Name);
            return View(new IndexViewModel { UserId = user.Id });
        }

        public ActionResult GetTasks()
        {
            var repo = new TasksRepository(Properties.Settings.Default.ConStr);
            return Json(repo.GetActiveTasks().Select(t =>
            {
                return new
                {
                    Id = t.Id,
                    Title = t.Title,
                    HandledBy = t.HandledBy,
                    User = new
                    {
                        FirstName = t.User.FirstName,
                        LastName = t.User.LastName,
                        Email = t.User.EmailAddress
                    }
                };
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var repo = new UserRepository(Properties.Settings.Default.ConStr);
            var user = repo.Login(email, password);
            if (user == null)
            {
                return RedirectToAction("Index");
            }

            FormsAuthentication.SetAuthCookie(user.EmailAddress, false);
            return RedirectToAction("Index");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User user, string password)
        {
            var repo = new UserRepository(Properties.Settings.Default.ConStr);
            repo.AddUser(user, password);
            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}