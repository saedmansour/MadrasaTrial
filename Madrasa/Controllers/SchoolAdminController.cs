using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Madrasa.Models;
using System.Web.Security;
using System.Web.Script.Serialization;

namespace Madrasa.Controllers
{ 
    public class SchoolAdminController : Controller
    {
        private SchoolAdminDbContext        schoolAdminDbContext    = new SchoolAdminDbContext();
        private UserDbContext               userDbContext           = new UserDbContext();
        private SchoolDbContext             schoolDbContext         = new SchoolDbContext();
        private RoleDbContext               roleDbContext           = new RoleDbContext();
        private JavaScriptSerializer        js                      = new JavaScriptSerializer();
        //
        // GET: /SchoolAdmins/

        public ViewResult Index()
        {
            return View(schoolAdminDbContext.dbSet .ToList());
        }

        //
        // GET: /SchoolAdmins/Details/5

        public ViewResult Details(int id)
        {
            SchoolAdmin schooladmin = schoolAdminDbContext.dbSet.Find(id);
            return View(schooladmin);
        }

        //
        // GET: /SchoolAdmins/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /SchoolAdmins/Create

        [HttpPost]
        public ActionResult Create(SchoolAdmin schooladmin)
        {
            if (ModelState.IsValid)
            {
                schoolAdminDbContext.dbSet.Add(schooladmin);
                schoolAdminDbContext.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(schooladmin);
        }
        
        //
        // GET: /SchoolAdmins/Edit/5
 
        public ActionResult Edit(int id)
        {
            SchoolAdmin schoolAdmin = schoolAdminDbContext.dbSet.Find(id);
            return View(schoolAdmin);
        }

        //
        // POST: /SchoolAdmins/Edit/5

        [HttpPost]
        public ActionResult Edit(SchoolAdmin schoolAdmin)
        {
            if (ModelState.IsValid)
            {
                schoolAdminDbContext.Entry(schoolAdmin).State = EntityState.Modified;
                schoolAdminDbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(schoolAdmin);
        }

        //
        // GET: /SchoolAdmins/Delete/5
 
        public ActionResult Delete(int id)
        {
            SchoolAdmin schoolAdmin = schoolAdminDbContext.dbSet.Find(id);
            return View(schoolAdmin);
        }

        //
        // POST: /SchoolAdmins/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            SchoolAdmin schooladmin = schoolAdminDbContext.dbSet.Find(id);
            schoolAdminDbContext.dbSet.Remove(schooladmin);
            schoolAdminDbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        //
        //GET: /ScoolAdmins/AddSchoolAdmin

        public ActionResult AddSchoolAdmin()
        {
            ViewBag.users = userDbContext.dbSet.ToList();
            ViewBag.schools = schoolDbContext.dbSet.ToList();
            ViewBag.roles = roleDbContext.dbSet.ToList();
            return View();
        }

        //
        // POST: /SchoolAdmins/AddSchoolAdmin

        [HttpPost]
        public ActionResult AddSchoolAdmin(string userId,string schoolIdsList,string role)
        {
            if (userId != null)
            {
                int id = int.Parse(userId);
                // create list from string
                List<int> schoolsIdsToAdd = (List<int>)js.Deserialize(schoolIdsList, typeof(List<int>));
                //add the user admin for all the schools. 
                foreach (var item in schoolsIdsToAdd)
                {
                    //create admin
                    SchoolAdmin admin = new SchoolAdmin();
                    admin.schoolId = item;
                    admin.userId = id;
                    //add admin to database
                    schoolAdminDbContext.dbSet.Add(admin);
                    schoolAdminDbContext.SaveChanges();
                }
            }
            // If we got this far, something failed, redisplay form
            return RedirectToAction("Index");
        }

        //
        // POST: /SchoolAdmins/ViewSchoolAdmin

        [HttpPost]
        public ActionResult ViewSchoolAdmin(string deleteIdsList)
        {
            List<int> idsToDelete = (List<int>)js.Deserialize(deleteIdsList, typeof(List<int>));
            foreach (int id in idsToDelete)
            {
                SchoolAdmin admin = schoolAdminDbContext.dbSet.Find(id);
                schoolAdminDbContext.dbSet.Remove(admin);
                schoolAdminDbContext.SaveChanges();
            }
            return RedirectToAction("Index");

        }

        //
        // getSchoolIdByAdminId:
        //  get adminID and return school ID.
        public List<int> getSchoolIdByAdminId(int id)
        {
            List<int> ids=new List<int>(); 
            var schoolsId = (from school in schoolAdminDbContext.dbSet select school);
            schoolsId = schoolsId.Where(school => school.userId == id);
            foreach (var item in schoolsId)
            {
                ids.Add(item.schoolId);
            }
            return ids;
        }

        protected override void Dispose(bool disposing)
        {
            schoolAdminDbContext.Dispose();
            base.Dispose(disposing);
        }
    }
}