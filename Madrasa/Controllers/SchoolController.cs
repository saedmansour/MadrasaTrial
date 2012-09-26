using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Madrasa.Models;
using System.Web.Script.Serialization;

namespace Madrasa.Controllers
{ 
    public class SchoolController : Controller
    {
        private SchoolDbContext db = new SchoolDbContext();
        private JavaScriptSerializer js = new JavaScriptSerializer();

        // GET: /Schools/

        public ViewResult Index()
        {
            return View(db.dbSet.ToList());
        }

        //
        // GET: /Schools/Details/5

        public ViewResult Details(int id)
        {
            School school = db.dbSet.Find(id);
            return View(school);
        }

        //
        // GET: /Schools/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Schools/Create

        [HttpPost]
        public ActionResult Create(School school)
        {
            if (ModelState.IsValid)
            {
                db.dbSet.Add(school);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(school);
        }
        
        //
        // GET: /Schools/Edit/5
 
        public ActionResult Edit(int id)
        {
            School school = db.dbSet.Find(id);
            return View(school);
        }

        //
        // POST: /Schools/Edit/5

        [HttpPost]
        public ActionResult Edit(School school)
        {
            if (ModelState.IsValid)
            {
                db.Entry(school).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(school);
        }

        //
        // GET: /Schools/Delete/5
 
        public ActionResult Delete(int id)
        {
            School school = db.dbSet.Find(id);
            return View(school);
        }

        //
        // POST: /Schools/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            School school = db.dbSet.Find(id);
            db.dbSet.Remove(school);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //
        // GET: /Schools/AddSchool

        public ActionResult AddSchool()
        {
            List<string> languageList = new List<string>() {"Choose language", "Arabic", "English" };
            SelectList language = new SelectList(languageList);
            ViewBag.language = language;
            return View();
        }

        //
        // POST: /Schools/AddSchool

        [HttpPost]
        public ActionResult AddSchool(School school)
        {  
            if (ModelState.IsValid)
            {
                db.dbSet.Add(school);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // If we got this far, something failed, redisplay form
            return View(school);
        }

        //
        // GET: /Schools/viewschool

        public ActionResult ViewSchool()
        {
            return View(db.dbSet.ToList());
        }

        //
        // POST: /Schools/ViewSchool

        [HttpPost]
        public ActionResult ViewSchool(string deleteIdsList)
        {
            List<int> idsToDelete = (List<int>)js.Deserialize(deleteIdsList, typeof(List<int>));
            foreach (int id in idsToDelete)
            {
                School school = db.dbSet.Find(id);
                db.dbSet.Remove(school);
                db.SaveChanges();
            }
            return RedirectToAction("Index");

        }
        //
        // getSchoolNameById:
        //  get ids of school and return school names.
        public List<string> getSchoolNameById(List<int> ids)
        {
            List<string> names = new List<string>();
            var schoolsName = (from school in db.dbSet select school);
            schoolsName = schoolsName.Where(school =>  ids.Contains( school.id ));
            foreach (var item in schoolsName)
            {
                names.Add(item.name);
            }
            return names;
        }

        //
        // getSchoolIdByName:
        //  get name of school and return school id.
        public int getSchoolIdByName(string name)
        {
            var schoolsId = (from school in db.dbSet select school);
            schoolsId = schoolsId.Where(school =>  school.name==name);
            foreach (var item in schoolsId)
            {
                return item.id;
            }
            return -1;
        }
        //
        // getSchoolsId:
        //  get all schools id
        public List<int> getSchoolsId()
        {
            List<int> schoolsId = (from school in db.dbSet select school.id).ToList();
            return schoolsId;
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}