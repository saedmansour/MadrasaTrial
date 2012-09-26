using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Madrasa.Models;
using System.Data;
using System.Web.Script.Serialization;

namespace Madrasa.Controllers
{ 
    public class QuestionController : Controller
    {
        private readonly QuestionDbContext      _questionDbContext  = new QuestionDbContext();
        private readonly SubjectController      _subjects           = new SubjectController();
        private readonly JavaScriptSerializer   _js                 = new JavaScriptSerializer();

        public const int  LEVELS_NUM = 3;
        public const int  DEFAULT_LEVEL = 1;    //TODO: shouldn't it be 2? medium?
        

        //
        // GET: /Questions/
        //
        public ViewResult Index()
        {
            return View(_questionDbContext.dbSet.ToList());
        }

        //
        // GET: /Questions/Details/5
        //
        public ViewResult Details(int id)
        {
            Question question = _questionDbContext.dbSet.Find(id);
            return View(question);
        }

        //
        // GET: /Questions/Create
        //
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Questions/Create
        //
        [HttpPost]
        public ActionResult Create(Question quest)
        {
            if (ModelState.IsValid)
            {
                _questionDbContext.dbSet.Add(quest);
                _questionDbContext.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(quest);
        }
        
        //
        // GET: /Questions/Edit/5
        public ActionResult Edit(int id)
        {
            Question question = _questionDbContext.dbSet.Find(id);
            return View(question);
        }

        //
        // POST: /Questions/Edit/5
        [HttpPost]
        public ActionResult Edit(Question question)
        {
            if (ModelState.IsValid)
            {
                _questionDbContext.Entry(question).State = EntityState.Modified;
                _questionDbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(question);
        }

        //
        // GET: /Question/Delete/5
         public ActionResult Delete(int id)
        {
            Question question = _questionDbContext.dbSet.Find(id);
            return View(question);
        }

        //
        // POST: /Question/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Question question = _questionDbContext.dbSet.Find(id);
            _questionDbContext.dbSet.Remove(question);
            _questionDbContext.SaveChanges();
            return RedirectToAction("Index");
        }


        // TODO: ?
        protected override void Dispose(bool disposing)
        {
            _questionDbContext.Dispose();
            base.Dispose(disposing);
        }

        //CreateViewBagOfLevels
        //
        public void CreateViewBagOfLevels()
        {
            //create grades list
            List<int> levels = new List<int>();
            for (int i = 1; i <= LEVELS_NUM; i++)
            {
                levels.Add(i);
            }
            //create select list of grades
            ViewBag.defaultLevel = DEFAULT_LEVEL;
            ViewBag.levels = new SelectList(levels, ViewBag.defaultLevel);
        }

        //
        // GET: /Question/AddQuestion
        public ActionResult AddQuestion()
        {
            CreateViewBagOfLevels();
            List<SubjectView> subjectViewList = _subjects.GetSubjectViewList();
            ViewBag.subjects = subjectViewList;
            return View();
        }

        //
        // POST: /Question/AddQuestion
        [HttpPost]
        public ActionResult AddQuestion(Question question)
        {
            if (ModelState.IsValid)
            {
                _questionDbContext.dbSet.Add(question);
                _questionDbContext.SaveChanges();
                return RedirectToAction("AddQuestion");
            }
            return View(question);
        }

        //
        // GET: /Question/ViewQuestion
        public ActionResult ViewQuestion()
        {
            List<SubjectView> subjectViewList = _subjects.GetSubjectViewList();
            ViewBag.subjects = subjectViewList;
            // send list of questions to View
            List<Question> questionList = _questionDbContext.dbSet.ToList();   
            return View(questionList);
        }

        //
        // POST: /Question/ViewQuestion
        [HttpPost]
        public ActionResult ViewQuestion(string deleteIdsList)
        {
            List<int> idsToDelete = (List<int>)_js.Deserialize(deleteIdsList,typeof(List<int>));
            foreach (int id in idsToDelete)
            {
                Question question = _questionDbContext.dbSet.Find(id);
                _questionDbContext.dbSet.Remove(question);
                _questionDbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        
        //
        //getQuestionLevelById: return question id
        public int GetQuestionLevelById(int id)
        {
            var questions = (from question in _questionDbContext.dbSet select question);
            questions = questions.Where(question => question.id == id);  
            if(!questions.Any())
            {
                throw new QuestionNotFoundException();
            }
            return questions.First().level;
        }

        //
        //GetQuestionById
        public Question GetQuestionById(int questionId)
        {
            return _questionDbContext.dbSet.Find(questionId);
        }

        //GetQuestionListBySubjectId
        //
        public List<Question> GetQuestionListBySubjectId(int subjectId)
        {
            var questions = (from question in _questionDbContext.dbSet select question);
            questions = questions.Where(question => question.subjectId == subjectId);
            if (!questions.Any())
            {
                return new  List<Question>();
            }
            return questions.ToList();
        }
    }

    public class QuestionNotFoundException : Exception
    {
    }
}