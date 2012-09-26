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
    public class UserController : Controller
    {
        private UserDbContext           db                  = new UserDbContext();
        private AdminDbContext          adminDb             = new AdminDbContext();
        private TeacherDbContext        teacherDb           = new TeacherDbContext();
        private StudentDbContext        studentDb           = new StudentDbContext();
        private SchoolDbContext         schoolDb            = new SchoolDbContext();
        private RoleDbContext           roleDb              = new RoleDbContext();
        private SchoolClassDbContext    classDb             = new SchoolClassDbContext();
        private CourseController        courseDb            = new CourseController(); 
        private JavaScriptSerializer    js                  = new JavaScriptSerializer();

        //
        // GET: /Users/

        public ViewResult Index()
        {
            return View(db.dbSet.ToList());
        }

        //
        // GET: /Users/Details/5

        public ViewResult Details(int id)
        {
            User user = db.dbSet.Find(id);
            return View(user);
        }

        //
        // GET: /Users/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Users/Create

        [HttpPost]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                db.dbSet.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(user);
        }
        
        //
        // GET: /Users/Edit/5
 
        public ActionResult Edit(int id)
        {
            User user = db.dbSet.Find(id);
            return View(user);
        }

        //
        // POST: /Users/Edit/5

        [HttpPost]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        //
        // GET: /Users/Delete/5
 
        public ActionResult Delete(int id)
        {
            User user = db.dbSet.Find(id);
            return View(user);
        }

        //
        // POST: /Users/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            User user = db.dbSet.Find(id);
            db.dbSet.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // GET: /Users/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.schools = schoolDb.dbSet.ToList();
            ViewBag.roles   = roleDb.dbSet.ToList();
            ViewBag.classes = classDb.dbSet.ToList();
            return View();
        }

        // help function for adding users
        //

        //add Admin 
        public void AddAdmin(Admin admin)
        {
            adminDb.dbSet.Add(admin);
            adminDb.SaveChanges();
        }
        //add teacher
        public void AddTeacher(Teacher teacher)
        {
            teacherDb.DbSet.Add(teacher);
            teacherDb.SaveChanges();
        }
        //add user
        public void AddStudent(Student student)
        {
            studentDb.dbSet.Add(student);
            studentDb.SaveChanges();
        }

        //Add user with his role
        public void AddUserWithRole(string role,int id, Admin admin,Teacher teacher,Student student)
        {
            admin.userId = teacher.UserId = student.userId = id; 
            switch(role)
            {
                case "SchoolAdmin": AddAdmin(admin);
                    break;
                case "Teacher": AddTeacher(teacher);
                    break;
                case "Student": AddStudent(student);
                    break;
            }
        }

        //
        // POST: /Users/Register
        [HttpPost]
        public ActionResult Register(User user,string role, Admin admin,Teacher teacher,Student student)
        {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(user.userName, user.password, user.email, null, null, true, null, out createStatus);
                if (createStatus == MembershipCreateStatus.Success)
                {
                    //add to user database
                    db.dbSet.Add(user);
                    db.SaveChanges();
                    //add role
                    Roles.AddUserToRole(user.userName, role);
                    //add to our tables
                    int id = GetUserIdByUsername(user.userName);
                    AddUserWithRole(role,id,admin, teacher, student);
                    FormsAuthentication.SetAuthCookie(user.userName, false /* createPersistentCookie */);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
                return RedirectToAction("Index");
        }

        //
        // GET: /Users/ViewUser

        public ActionResult ViewUser()
        {
            return View(db.dbSet.ToList());
        }

        //
        // POST: /Users/ViewUser

        [HttpPost]
        public ActionResult ViewUser(string deleteIdsList)
        {
            List<int> idsToDelete = (List<int>)js.Deserialize(deleteIdsList, typeof(List<int>));
            foreach (int id in idsToDelete)
            {
                User user = db.dbSet.Find(id);
                Membership.DeleteUser(user.userName);
                db.dbSet.Remove(user);
                db.SaveChanges();
            }
            return RedirectToAction("Index");

        }

        //
        // Get: /Users/couresRegistration
        public ActionResult CouresRegistration()
        {
            if(User.IsInRole("Student"))
            {
                return RedirectToAction("StudentCouresRegistration");
            }
            if (User.IsInRole("Teacher"))
            {
                return RedirectToAction("TeacherCouresRegistration");
            }
            return View();
        }

        //
        // Get: /Users/couresRegistration
        public ActionResult StudentCouresRegistration()
        {
            List<Course> courseRegistrList = courseDb.GetTeacherRegisteredCourseList();
            List<int> teacherIdList = courseDb.GetTeacherIdsFromCourseRegistration();
            List<Teacher> teacherList = new List<Teacher>();

            foreach (var item in teacherIdList)
            {
                Teacher teacher = GetTeacherById(item);
                teacherList.Add(teacher);
            }
            ViewBag.teacherList = teacherList;
            ViewBag.courseList = courseRegistrList;
            return View();
        }

        [HttpPost]
        //
        // Post: /Users/couresRegistration
        //TODO: create new table and save for each student the teacher of his cousre , because for one course can be more than one teacher
        public ActionResult StudentCouresRegistration(int courseId,int teacherId)
        {
            int userId = GetUserIdByUsername(User.Identity.Name);
            courseDb.RegisterUserForCourse(userId, courseId, "Student");
            return RedirectToAction("StudentCouresRegistration");
        }

        //
        // Get: /Users/couresRegistration
        public ActionResult TeacherCouresRegistration()
        {
            ViewBag.CourseList = courseDb.GetAllCourses();
            return View();
        }

        //
        // Post: /Users/couresRegistration
        [HttpPost]
        public ActionResult TeacherCouresRegistration(int courseId)
        {
            int userId = GetUserIdByUsername(User.Identity.Name);
            courseDb.RegisterUserForCourse(userId, courseId, "Teacher");
            return RedirectToAction("TeacherCouresRegistration");
        }

        //
        // getUserIdByUserName:
        //  get username and return user ID.
        public int GetUserIdByUsername(string userName)
        {
            var usersId = (from user in db.dbSet select user);
            usersId = usersId.Where(user => user.userName == userName);
            foreach (var item in usersId)
            {
                return item.id; 
            }
            throw new UserNotFoundException();
        }

        //
        // getSchoolIdByAdminId:
        //  get adminID and return school ID.
        public List<int> GetSchoolIdByAdminId(int id)
        {
            List<int> ids = new List<int>();
            var schoolsId = (from admin in adminDb.dbSet select admin);
            schoolsId = schoolsId.Where(admin => admin.userId == id);
            foreach (var item in schoolsId)
            {
                ids.Add(item.schoolId);
            }
            return ids;
        }

        //
        // getSchoolIdByStudetId:
        //  get student id  and return school ID.
        public int GetSchoolIdByStudetId(int id)
        {
            var schoolsId = (from student in studentDb.dbSet  select student);
            schoolsId = schoolsId.Where(student => student.userId == id);
            foreach (var item in schoolsId)
            {
                return item.schoolId;
            }
            return -1;
        }
        //
        // getSchoolIdByTeacherId:
        //  get teacher id  and return school ID.
        public int GetSchoolIdByTeacherId(int id)
        {
            var schoolsId = (from teacher in teacherDb.DbSet select teacher);
            schoolsId = schoolsId.Where(teacher => teacher.UserId == id);
            foreach (var item in schoolsId)
            {
                return item.SchoolId;
            }
            return -1;
        }
        //
        // getGradeByStudentId:
        //  get student id  and grade.
        public int GetGradeByStudentId(int id)
        {
            var schoolsId = (from student in studentDb.dbSet  select student);
            schoolsId = schoolsId.Where(student => student.userId == id);
            if (!schoolsId.Any())
            {
                return -1;
            }
            //get classId
            int classId = schoolsId.First().classId;
            var classes = (from classs in classDb.dbSet select classs);
            classes = classes.Where(classs => classs.id == classId);
            //get grade from class
            return (!classes.Any()) ? -1 : classes.First().grade;
        }


        //GetTeacherById
        //get Id and return Course
        public Teacher GetTeacherById(int id)
        {
            var teacherList = (from teacher in teacherDb.DbSet select teacher);
            teacherList = teacherList.Where(teacher => teacher.Id == id);
            if (!teacherList.Any())
            {
                throw new TeacherNotFoundException();
            }
            return teacherList.First();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        //Error status
        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }

    public class TeacherNotFoundException : Exception
    {
    }

    public class UserNotFoundException : Exception
    {
    }
}