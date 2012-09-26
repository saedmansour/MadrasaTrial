using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Madrasa.Models;

namespace Madrasa.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (User.IsInRole("Student")){
                return RedirectToAction("Student");
            }
            if (User.IsInRole("Teacher")){
                return RedirectToAction("Teacher");
            }
            return View();
        }

        // TODO: reverse
        //[Authorize(Roles = "Student")]
        public ActionResult Student()
        {
            HomeworkDbContext homeworkDb    = new HomeworkDbContext();
            // TODO: get homework by courseID
            List<Homework> model            = (from homework in homeworkDb.DbSet select homework).ToList();
            return View(model);
        }

        // TODO: reverse
        //[Authorize(Roles = "Teacher")]
        public ActionResult Teacher()
        {
            return View();
        }
    }
}
