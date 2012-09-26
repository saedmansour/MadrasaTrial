using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Madrasa.Models;
using Madrasa.Global;

namespace Madrasa.Controllers
{
    public class HomeworkController : Controller
    {
        private readonly HomeworkDbContext           _homeworkDb                  = new HomeworkDbContext();
        private readonly HomeworkSolvedDbContext     _homeworkSolvedDb            = new HomeworkSolvedDbContext();
        private readonly HomeWorkQuestionDbContext   _homeworkQuestionSolvedDb    = new HomeWorkQuestionDbContext();
        private readonly GreedyAlgorithm             _algorithm                   = new GreedyAlgorithm();
        private readonly UserController              _userDb                      = new UserController();
        private readonly SubjectController           _subjectDb                   = new SubjectController();
        private readonly CourseController            _courseDb                    = new CourseController();
        private readonly HomeworkStateDbContext      _homeworkStateDb             = new HomeworkStateDbContext(); 
        
        //
        // GET: /Homeworks/
        //
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /HomeWorks/AddRemoveHomework
        //
        [Authorize]
        public ActionResult Edit()
        {
            int userId = _userDb.GetUserIdByUsername(User.Identity.Name);
            ViewBag.courseList = _courseDb.GetCourseListByRoleAndId(userId, "Teacher");
            ViewBag.schoolId    = _userDb.GetSchoolIdByTeacherId(userId);
            ViewBag.homeWorks   = _homeworkDb.DbSet.ToList();
            ViewBag.subjects    = _subjectDb.GetSubjectViewList();
            return View();
        }

        //
        // Post: /HomeWorks/Edit
        //get homework for add, list for delete
        [HttpPost]
        public ActionResult Edit(Homework homeworkInput,string deleteHomeWorks, List<int> deleteIdsList)
        {
            if (!string.IsNullOrEmpty(deleteHomeWorks))
            {
                Delete(deleteIdsList);
                return RedirectToAction("Edit");
            }
            //add the homework u get
           _homeworkDb.DbSet.Add(homeworkInput);
           _homeworkDb.SaveChanges();
            return RedirectToAction("Edit");
        }

        //
        // GET: /HomeWorks/studentHomework
        [Authorize]
        public ActionResult StudentHomework()
        {
            int userId = _userDb.GetUserIdByUsername(User.Identity.Name);
            CreateViewBagOfHomework(userId);
            return View();
        }

        // GET: /HomeWorks/preSolveHomework
        //  Function to prepare before start solving homeWork
        [Authorize]
        public ActionResult PreSolveHomework(int baseSubjectIndex, int homeworkId)
        {
            int userId = _userDb.GetUserIdByUsername(User.Identity.Name);
            int courseId = GetCourseId(homeworkId);
            List<int> baseSubjectIdList = _courseDb.GetCourseSubjectIdList(courseId);
            if (baseSubjectIdList.Count() <= baseSubjectIndex)
            {
                return RedirectToAction("HomeworkSolved", new { homeWorkId = homeworkId });
            }
            return RedirectToAction("SolveHomework", new{   baseSubjectIndex = baseSubjectIndex,
                                                            subjectId = baseSubjectIdList[baseSubjectIndex] ,
                                                            homeworkId = homeworkId 
                                                        });
        }

        // GET: /HomeWorks/solveHomework
        //
        [Authorize]
        public ActionResult SolveHomework(int baseSubjectIndex,int subjectId, int homeworkId)
        {
            int userId = _userDb.GetUserIdByUsername(User.Identity.Name);
            if (IsHomeworkSolved(homeworkId, userId)) {
                return RedirectToAction("HomeworkSolved", new {homeworkId });
            }   
            //send First parameters to start homeWork
            HomeworkState newState=new HomeworkState(userId,baseSubjectIndex,subjectId,subjectId,0,0,homeworkId,0);
            //if state exist ,update else create
            if (IsUserStateExist(newState.UserId, newState.HomeworkId))
            {
                UpdateUserState(newState);
            }
            else
            {
                CreateUserState(newState);
            }
            return RedirectToAction("HomeworkFirstStage",new {homeworkId});
        }

        //GET: HomeworkFirstStage:
        //when we go down in tree subject we will be in FirstStage
        //otherWise we are in LastStage
        public ActionResult HomeworkFirstStage(int homeworkId)
        {
            ViewBag.homeworkId = homeworkId;
            int userId = _userDb.GetUserIdByUsername(User.Identity.Name);
            HomeworkState state = GetUserState(userId, homeworkId);
            HomeworkState nextState = new HomeworkState(GetUserState(userId,homeworkId));
            //update mark,and check if we need to go to children subject 
            if (_algorithm.IsGoDown(ref nextState))
            {
                List<int> subjectChildrenIds = _subjectDb.GetChildren(state.SubjectId);
                try {
                    nextState.SubjectId      = GetAvailableSubject(subjectChildrenIds);
                    nextState.Mark           = 0;
                    nextState.QuestionNumber = 0;
                }
                catch(SubjectNotFoundException)
                {
                    return RedirectToAction("HomeworkMaterial", new { state.HomeworkId });
                }
            }
            else
            {
                //check if we Finish the current Level, and go Up.
                if (state.QuestionNumber >= _algorithm.MaxNumberOfStageQuestion)
                {
                    //if we get to the base subject mean we finish Exam
                    if (state.SubjectId == state.BaseSubjectId)
                    {
                        return RedirectToAction("PreSolveHomeWork", new{baseSubjectIndex = state.IndexOfBaseSubject+1,
                            homeWorkId = state.HomeworkId});
                    }
                    else
                    {
                        nextState.SubjectId = _subjectDb.GetSubjectParentId(state.SubjectId);
                        nextState.Mark = -100; //TODO: need to change that we fail
                        UpdateUserState(nextState);
                        return RedirectToAction("HomeworkFirstStage", new { state.HomeworkId });
                    }
                    
                }
            }
            //continue with the next state.
            ViewBag.question = _algorithm.GetNextQuestion(nextState, userId);
            nextState.QuestionNumber++;
            UpdateUserState(nextState);
            return View();
        }


        //GET: HomeworkFirstStage:
        //when we go down in tree subject we will be in FirstStage
        //otherWise we are in LastStage
        [HttpPost]
        public ActionResult HomeworkFirstStage(int homeworkId, int questionId,string userAnswer )
        {
            ViewBag.homeworkId = homeworkId;
            int userId = _userDb.GetUserIdByUsername(User.Identity.Name);
            HomeworkState state = GetUserState(userId, homeworkId);
            _algorithm.UpdateMark(ref state, questionId, userAnswer);
            UpdateUserState(state);
            return RedirectToAction("HomeworkFirstStage",new{homeworkId});
        }
        
        //GET: /HomeWorks/HomeWorkMaterial
        //
        public ActionResult HomeWorkMaterial(int homeworkId)
        {
            ViewBag.homeworkId = homeworkId;
            int userId = _userDb.GetUserIdByUsername(User.Identity.Name);
            HomeworkState state = GetUserState(userId, homeworkId);
            Material material = _algorithm.GetMatrial(state.SubjectId);
            ViewBag.material = material;
            return View();
        }
        
        //Post: /HomeWorks/HomeWorkMaterial
        //
        [HttpPost]
        public ActionResult HomeWorkMaterial(int homeworkId, string a)
        {
            ViewBag.homeworkId = homeworkId;
            int userId = _userDb.GetUserIdByUsername(User.Identity.Name);
            HomeworkState state = GetUserState(userId, homeworkId);
            state.QuestionNumber = 0;
            UpdateUserState(state);
            return RedirectToAction("HomeWorkLastStage",new{state.HomeworkId});
        }

        

        //HomeWorkLastStage
        //in LastStage: we give questions with solution
        public ActionResult HomeWorkLastStage(int homeworkId)
        {
            ViewBag.homeworkId = homeworkId;
            int userId = _userDb.GetUserIdByUsername(User.Identity.Name);
            HomeworkState state = GetUserState(userId, homeworkId);
            HomeworkState nextState = new HomeworkState(GetUserState(userId, homeworkId));
            if (_algorithm.MaxNumberOfStageQuestion <= state.QuestionNumber)
            {
                state.Mark = 0;
                UpdateUserState(state);
                return RedirectToAction("HomeWorkFirstStage",new{homeworkId});
            }
            ViewBag.question = _algorithm.GetNextQuestion(state,userId);
            nextState.QuestionNumber++;
            UpdateUserState(nextState);
            return View();
        }

        //
        //Get: /Homework/HomeworkSolved
        //
        public ActionResult HomeworkSolved(int homeworkId)
        {
            ViewBag.homeworkId = homeworkId;
            return View();
        }

        //
        //Get: /Homework/reviewSolveHomework
        //
       public ActionResult HomeworkReview()
       {
           return View();
       }
/********************************************************************************************************************
 *                      End of actions
 *                      help funvtions
 * ******************************************************************************************************************/
        //createViewBagOfgrades : create viewBag.grades which contain grades
        //
        public void CreateViewBagOfgrades()
        {
            //create grades list
            List<int> gardes = new List<int>();
            for (int i = 0; i < 12; i++)
            {
                gardes.Add(i + 1);
            }
            //create select list of grades
            ViewBag.defaultGrade = 1;
            ViewBag.grades = new SelectList(gardes, ViewBag.defaultGrade);
        }
        
        //getAvailableSubject:
        //      get the children subject that never visited.
        public int GetAvailableSubject(List<int> subjectsId)
        {
            var checkedSubjects = (from question in _homeworkQuestionSolvedDb.DbSet select question.SubjectId).ToList();
            foreach (var item in subjectsId) {
                //if subjectsId[i] never visited return it.
                if (!checkedSubjects.Contains(item))
                {
                    return item;
                }
            }
            //create Query
            throw new SubjectNotFoundException();
        }
        
        //
        //CreateViewBagOfHomework
        //  get userId and return  ViewBag.homeWorks which contain all home work that user should do
        void CreateViewBagOfHomework(int userId)
        {
            DateTime currentTime    = DateTime.Now;
            List<Course> courseList = _courseDb.GetCourseListByRoleAndId(userId, "Student");
            List<int> coursesId = courseList.Select(course => course.Id).ToList();
            var homeworList    = (from homework in _homeworkDb.DbSet select homework);

            homeworList = homeworList.Where(homework =>
                homework.Start <= currentTime && homework.End >= currentTime && coursesId.Contains(homework.CourseId));

            ViewBag.homeWorks = homeworList.ToList();
        }

        //IsHomeworkSolved
        //  Get homeWork id and userId and check if the user solve this homwork
        public bool IsHomeworkSolved(int homeworkId, int userId)
        {
            var users = (from solvedHomework in _homeworkSolvedDb.DbSet select solvedHomework);
            // create Query : check if user did the exam
            users = users.Where(homework => homework.HomeworkId == homeworkId && homework.StudentId == userId);
            if (!users.Any())
            {
                return false;
            }
            return true;
        }

        //GetCourseId
        // get course id from homeWorkId
        public int GetCourseId(int homeworkId)
        {
            var homeworList = (from homework in _homeworkDb.DbSet select homework);
            homeworList = homeworList.Where(homework => homework.Id==homeworkId);
            if(!homeworList.Any())
            {
                throw  new HomeworkIdNotFoundException();
            }
            return homeworList.First().CourseId;
        }

        //Delete:
        //  Function get list of homework ids and delete
        public void Delete(List<int> homeworkListId)
        {
            var homeworkList = (from homework in _homeworkDb.DbSet select homework);
            homeworkList = homeworkList.Where(homework => homeworkListId.Contains(homework.Id));
            foreach (var course in homeworkList)
            {
                _homeworkDb.DbSet.Remove(course);
            }
            _homeworkDb.SaveChanges();

        }


        //isUserStateExist
        //  return true if student start exam and didnt finish
        private  bool IsUserStateExist(int userId,int homeworkId)
        {
            var homeworkStateList = (from prevState in _homeworkStateDb.DbSet select prevState);
            homeworkStateList = homeworkStateList.Where(prevState => prevState.HomeworkId == homeworkId && prevState.UserId == userId);
            if (homeworkStateList.Any())
            {
                return true;
            }
            return false;
        }

        //updaUserState:
        //
        private void UpdateUserState(HomeworkState state)
        {
            RemoveUserState(state.UserId, state.HomeworkId);
            //add new state
            _homeworkStateDb.DbSet.Add(state);
            _homeworkStateDb.SaveChanges();
        }

        //getUserState
        //
        private HomeworkState GetUserState(int userId, int homeworkId)
        {
            var homeworkStateList = (from prevState in _homeworkStateDb.DbSet select prevState);
            homeworkStateList = homeworkStateList.Where(prevState => prevState.HomeworkId == homeworkId && prevState.UserId == userId);
            if (!homeworkStateList.Any())
            {
                throw new HomeworkStateNotFoundException();
            }
            return homeworkStateList.First();
        }

        //finishUserState
        //  when user finish exam , remove his state from the table
        private void FinishUserState(int userId, int homeworkId)
        {
            RemoveUserState(userId,homeworkId);
        }

        //RemoveUserState
        //  
        private void  RemoveUserState(int userId,int homeworkId)
        {
            var homeworkStateList = (from prevState in _homeworkStateDb.DbSet select prevState);
            homeworkStateList = homeworkStateList.Where(prevState => prevState.HomeworkId == homeworkId && prevState.UserId == userId);
            if (!homeworkStateList.Any())
            {
                throw new HomeworkStateNotFoundException();
            }
            //remove previous state
            _homeworkStateDb.DbSet.Remove(homeworkStateList.First());
            _homeworkStateDb.SaveChanges();
        }

        //CreateUserState
        //
        private void CreateUserState(HomeworkState state)
        {
            _homeworkStateDb.DbSet.Add(state);
            _homeworkStateDb.SaveChanges();
        }

    }

    

    /********************************************************************************************************************
     *                      Exception
     *                      
     * ******************************************************************************************************************/
    public class HomeworkIdNotFoundException : Exception
    {
    }
    public class SubjectNotFoundException : Exception {
    }
    public class HomeworkStateNotFoundException : Exception
    {
    }
}
