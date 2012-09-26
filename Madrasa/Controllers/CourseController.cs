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
    public class CourseController : Controller
    {
        private readonly CourseDbContext             _db                = new CourseDbContext();
        private readonly SubjectController           _subjects          = new SubjectController();
        private readonly JavaScriptSerializer        _js                = new JavaScriptSerializer();
        private readonly CourseRegistrationDbContext _courseRegistrDb   = new CourseRegistrationDbContext();
        //
        // GET: /Course/

        public ViewResult Index()
        {
            return View(_db.DbSet.ToList());
        }

        //
        // GET: /Course/Details/5

        public ViewResult Details(int id)
        {
            Course course = _db.DbSet.Find(id);
            return View(course);
        }

        //
        // GET: /Course/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Course/Create

        [HttpPost]
        public ActionResult Create(Course course)
        {
            if (ModelState.IsValid)
            {
                _db.DbSet.Add(course);
                _db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(course);
        }

        //
        // GET: /Course/Delete/5
 
        public ActionResult Delete(int id)
        {
            Course course = _db.DbSet.Find(id);
            return View(course);
        }

        //
        // POST: /Course/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Course course = _db.DbSet.Find(id);
            _db.DbSet.Remove(course);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // GET: /Course/Add
        public ActionResult Add()
        {
            List<SubjectView> subjectViewList = _subjects.GetSubjectViewList();
            ViewBag.subjects = subjectViewList;
            return View();
        }

        //
        // Post: /Course/Add
        [HttpPost]
        public ActionResult Add(CourseForView viewCourse)
        {
            Course course = new Course(viewCourse);
            if (ModelState.IsValid)
            {
                _db.DbSet.Add(course);
                _db.SaveChanges();
            }
            return RedirectToAction("Add");
        }

        //
        // GET: /Course/Edit
        public ActionResult Edit()
        {
            List<SubjectView> subjectViewList = _subjects.GetSubjectViewList();
            ViewBag.subjects = subjectViewList;
            return View(GetAllCourseForView());
        }

        //
        // Post: /Course/Edit
        [HttpPost]
        public ActionResult Edit(List<int> deleteIdsList)
        {
            var courseList = (from course in _db.DbSet select course);
            courseList = courseList.Where(course => deleteIdsList.Contains(course.Id));
            foreach (var course in courseList )
            {
                _db.DbSet.Remove(course);
            }
            _db.SaveChanges();
            return RedirectToAction("Edit");
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        //GetCourseById
        //get Id and return Course
        public Course GetCourseById(int id)
        {
            var courseList = (from course in _db.DbSet select course);
            courseList = courseList.Where(course => course.Id == id);
            if (!courseList.Any())
            {
                throw new CourseNotExistException();
            }
            return courseList.First();
        }

        //GetAllCourses
        // return all exist Courses
        public List<Course> GetAllCourses()
        {
            return _db.DbSet.ToList();
        }
        
        // Convert the list of courses to list of view courses
        //
        public List<CourseForView> GetAllCourseForView()
        {
            List<CourseForView> viewCoursList=new List<CourseForView>();
            List<Course> courseList = _db.DbSet.ToList();
            foreach (var course in courseList)
            {
                CourseForView viewCourse= new CourseForView(course);
                viewCoursList.Add(viewCourse);
            }
            return viewCoursList;
        }

        //GetCoursesByTeacherId
        //  get all coures that the teacher with given id is registerd to it.
        public List<Course> GetTeacherRegisteredCourseList()
        {
            List<Course> courseList = new List<Course>();
            var courseRegistrList = (from course in _courseRegistrDb.DbSet select course);
            courseRegistrList = courseRegistrList.Where(course => course.Role == "Teacher");
            foreach(var item in courseRegistrList)
            {
                Course course = GetCourseById(item.CourseId);
                courseList.Add(course);
            }
            return courseList;
        }

        //GetTeacherIdsFromCourseRegistration
        //  get list of exist register courses for teacher and return the teacher id of this courses with the same order
        public List<int> GetTeacherIdsFromCourseRegistration()
        {
            List<int> idList = new List<int>();
            var teacherIdList = (from course in _courseRegistrDb.DbSet select course);
            teacherIdList = teacherIdList.Where(course => course.Role == "Teacher");
            foreach(var item in teacherIdList)
            {
                idList.Add(item.UserId);
            }
            return idList;
        }

        //GetCourseListByTeacherId
        // get all the courses for the role with the given ID
        public List<Course> GetCourseListByRoleAndId(int id,string role)
        {
            var courseIdsList = (from course in _courseRegistrDb.DbSet select course);
            courseIdsList = courseIdsList.Where(course => course.Role == role && course.UserId == id);
            List<int> coursesId = courseIdsList.Select(course => course.CourseId).ToList();
            if (!coursesId.Any())
            {
                //return empty list
                return new List<Course>() ;
            }
            var courseList = (from course in _db.DbSet select course);
            courseList = courseList.Where(course => coursesId.Contains(course.Id));    
            
            return courseList.ToList();
        }

        //RegisterUserForCourse
        // get userId,courseId and role
        public void RegisterUserForCourse(int userId,int courseId,string role)
        {
            CourseRegistration register= new CourseRegistration(userId,courseId,role);
           _courseRegistrDb .DbSet.Add(register);
           _courseRegistrDb.SaveChanges();
        }

        //GetCourseSubjectIdList
        //  get list of subject Id of the course
        public List<int> GetCourseSubjectIdList(int courseId)
        {  
            var courseList = (from course in _db.DbSet select course);
            courseList = courseList.Where(course => course.Id == courseId);
            if(!courseList.Any())
            {
                throw new ExceptionCourseIdNotFound();
            }
            string subjectListJson = courseList.Select(course => course.SubjectIdJson).First();
            List<int> subjectIdList = (List<int>)_js.Deserialize(subjectListJson, typeof(List<int>));

            return subjectIdList;
        }
        
    }

    public class ExceptionCourseIdNotFound : Exception
    {
    }

    public class CourseNotExistException : Exception
    {
    }
}