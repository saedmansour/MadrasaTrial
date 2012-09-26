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
    public class SchoolClassController : Controller
    {
        private SchoolClassDbContext    _schoolClassDbContext   = new SchoolClassDbContext();
        private UserController          _userDb                 = new UserController();         // TODO: an instance of UserController? a controller? isn't that weird?
        private SchoolController        _schoolDb               = new SchoolController();
        private JavaScriptSerializer    _js                     = new JavaScriptSerializer();

        private const int GRADES_NUM = 12;
        private const int DEFAULT_GRADE = 9;

        //
        // GET: /SchoolClass/
        
        public ViewResult Index()
        {
            return View(_schoolClassDbContext.dbSet.ToList());
        }

        //
        // GET: /SchoolClass/Details/5
        
        public ViewResult Details(int id)
        {
            SchoolClass schoolClass = _schoolClassDbContext.dbSet.Find(id);
            return View(schoolClass);
        }

        //
        // GET: /SchoolClass/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /SchoolClass/Create

        [HttpPost]
        public ActionResult Create(SchoolClass schoolClass)
        {
            if (ModelState.IsValid)
            {
                _schoolClassDbContext.dbSet.Add(schoolClass);
                _schoolClassDbContext.SaveChanges();
                return RedirectToAction("Index");  
            }
            return View(schoolClass);
        }
        
        //
        // GET: /SchoolClass/Edit/5
 
        public ActionResult Edit(int id)
        {
            int userId = _userDb.GetUserIdByUsername(User.Identity.Name);    // TODO: getUser(username)
            CreateViewBagOfSchoolsName(userId);                             //send schools name to client
            CreateViewBagOfgrades();                                        //send grades to client
            SchoolClass schoolClass = _schoolClassDbContext.dbSet.Find(id);
            return View(schoolClass);
        }

        //
        // POST: /Classes/Edit/5

        [HttpPost]
        public ActionResult Edit(SchoolClass classs, string schoolName)
        {
            if (ModelState.IsValid)
            {
                classs.schoolId = _schoolDb.getSchoolIdByName(schoolName);
                _schoolClassDbContext.Entry(classs).State = EntityState.Modified;
                _schoolClassDbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(classs);
        }

        //
        // GET: /Classes/Delete/5
 
        public ActionResult Delete(int id)
        {
            SchoolClass schoolClass = _schoolClassDbContext.dbSet.Find(id);
            return View(schoolClass);
        }

        //
        // POST: /Classes/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            SchoolClass classs = _schoolClassDbContext.dbSet.Find(id);
            _schoolClassDbContext.dbSet.Remove(classs);
            _schoolClassDbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        //createViewBagOfgrades : create viewBag.grades which contain grades
        //
        public void CreateViewBagOfgrades()
        {
            //create grades list
            List<int> gardes = new List<int>();
            for (int i = 1; i <= GRADES_NUM; i++)        //12 should be a constant
            {
                gardes.Add(i);
            }
            //create select list of grades
            ViewBag.defaultGrade = DEFAULT_GRADE;
            ViewBag.grades = new SelectList(gardes, ViewBag.defaultGrade);
        }

        //
        //createViewBagOfSchoolsName : create viewBag.schoolsName which contain schools name
        //
        public void CreateViewBagOfSchoolsName(int userId)
        {
            //get shcoolId
            List<int> schoolId = _userDb.GetSchoolIdByAdminId(userId);
            //get schools name List
            List<string> schoolsName = _schoolDb.getSchoolNameById(schoolId);
            //create select list of schools
            ViewBag.defaultSchool = (schoolsName.Count != 0) ? schoolsName[0] : null;
            ViewBag.schools = new SelectList(schoolsName, ViewBag.defaultSchool);
        }

        //
        // GET: /Classs/AddClass
        [Authorize]
        public ActionResult AddClass()
        {
            //get user Id
            int userId = _userDb.GetUserIdByUsername(User.Identity.Name);
            //send schools name to client
            CreateViewBagOfSchoolsName(userId);
            //send grades to client
            CreateViewBagOfgrades();
            return View();
        }

        //
        // POST: /Classs/AddClass

        [HttpPost]
        public ActionResult AddClass(SchoolClass classs, string schoolName)
        {
            if (ModelState.IsValid)
            {
                classs.schoolId = _schoolDb.getSchoolIdByName(schoolName);
                _schoolClassDbContext.dbSet.Add(classs);
                _schoolClassDbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            // If we got this far, something failed, redisplay form
            return View(classs);
        }

        //
        // GET: /Classes/ViewClass

        public ActionResult ViewClass()
        {
            List<SchoolClass> classes = _schoolClassDbContext.dbSet.ToList();
            ViewBag.schoolsId = (from classs in _schoolClassDbContext.dbSet select classs.schoolId).ToList();
            ViewBag.schoolsName = _schoolDb.getSchoolNameById(ViewBag.schoolsId);
            return View(_schoolClassDbContext.dbSet.ToList());
        }

        //
        // POST: /Classes/ViewClass

        [HttpPost]
        public ActionResult ViewClass(string deleteIdsList)
        {
            List<int> idsToDelete = (List<int>)_js.Deserialize(deleteIdsList, typeof(List<int>));
            foreach (int id in idsToDelete)
            {
                SchoolClass classs = _schoolClassDbContext.dbSet.Find(id);
                _schoolClassDbContext.dbSet.Remove(classs);
                _schoolClassDbContext.SaveChanges();
            }
            return RedirectToAction("Index");

        }

        protected override void Dispose(bool disposing)
        {
            _schoolClassDbContext.Dispose();
            base.Dispose(disposing);
        }
    }
}