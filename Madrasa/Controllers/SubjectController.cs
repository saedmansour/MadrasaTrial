using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Madrasa.Models;
using System.Web.Script.Serialization;
using Madrasa.Global;
namespace Madrasa.Controllers
{ 
    public class SubjectController : Controller
    {
        private SubjectDbContext _subjectDbContext      = new SubjectDbContext();
        private const string IdSpliter                  = "/";

        //
        // GET: /Subject/
        //
        public ViewResult Index()
        {
            return View(_subjectDbContext.dbSet.ToList());
        }

        //
        // GET: /Subject/Details/5
        //
        public ViewResult Details(int id)
        {
            Subject subject = _subjectDbContext.dbSet.Find(id);
            return View(subject);
        }

        //
        // GET: /Subject/Create
        //
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Subject/Create
        //
        [HttpPost]
        public ActionResult Create(Subject subject)
        {
            if (ModelState.IsValid)
            {
                _subjectDbContext.dbSet.Add(subject);
                _subjectDbContext.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(subject);
        }
        
        //
        // GET: /Subject/Edit/5
        //
        public ActionResult Edit(int id)
        {
            Subject subject = _subjectDbContext.dbSet.Find(id);
            return View(subject);
        }

        //
        // POST: /Subject/Edit/5
        //
        [HttpPost]
        public ActionResult Edit(Subject subject)
        {
            if (ModelState.IsValid)
            {
                _subjectDbContext.Entry(subject).State = EntityState.Modified;
                _subjectDbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(subject);
        }

        //
        // GET: /Subject/Delete/5
        //
        public ActionResult Delete(int id)
        {
            Subject subject = _subjectDbContext.dbSet.Find(id);
            return View(subject);
        }

        //
        // POST: /Subject/Delete/5
        //
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Subject subject = _subjectDbContext.dbSet.Find(id);
            _subjectDbContext.dbSet.Remove(subject);
            _subjectDbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            _subjectDbContext.Dispose();
            base.Dispose(disposing);
        }

        //
        //createNextchildrenId:
        //return string like ancestorsIds/ID
        //
        public string CreateNextchildrenId(string ancestorsIds, int id)
        {
            if (ancestorsIds != null) {
                return ancestorsIds + IdSpliter + id.ToString();
            }
            return id.ToString();
        }

        //
        //SubjectListToSubjectViewList:
        //
        List<SubjectView> SubjectListToSubjectViewList(List<Subject> subjectList)
        {
            List<SubjectView> subjectViewList = new List<SubjectView>();
            foreach (Subject subject in subjectList)
            {
                //convert every subject to SubjectView
                SubjectView subjectView = new SubjectView(subject, IdSpliter);
                //add to subject to the list
                subjectViewList.Add(subjectView);
            }
            return subjectViewList;
        }

        //
        //GetSubjectViewList:
        // 
        public List<SubjectView> GetSubjectViewList()
        {
            // List of subjects from the table
            List<Subject> subjectList = _subjectDbContext.dbSet.ToList();
            //create new list of subjects for view from the subjects in the table 
            List<SubjectView> subjectViewList = SubjectListToSubjectViewList(subjectList);
            return subjectViewList;
        }

        //
        // GET: /Subject/AddNewSubject
        //
        public ActionResult AddNewSubject()
        {
            //Just send all the subjects for View
            List<SubjectView> subjectViewList = GetSubjectViewList();
            return View(subjectViewList);
        }

        //
        // POST: /Subject/AddNewSubject
        //
        [HttpPost]
        public ActionResult AddNewSubject(Subject subject)
        {
            if (ModelState.IsValid)
            {
                _subjectDbContext.dbSet.Add(subject);
                _subjectDbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag["errorMessage"] = "Error with model.";  //This message isn't used. Verify data client.
            return View( GetSubjectViewList());
        }

        //
        // GET: /Subject/DeleteSubject
        //
        public ActionResult DeleteSubject()
        {
            List<Subject> subjectList           = _subjectDbContext.dbSet.ToList();
            List<SubjectView> subjectViewList   = new List<SubjectView>();
            foreach (Subject subject in subjectList)
            {
                //convert every subject to SubjectView
                SubjectView subjectView= new SubjectView(subject, IdSpliter);
                //add to subject to the list
                subjectViewList.Add(subjectView);
            }
            return View(subjectViewList);
        }

        //
        // POST: /Subject/DeleteSubject
        // get flag DeleteSons of subject
        //
        [HttpPost]
        public ActionResult DeleteSubject(int id, string DeleteSons)
        {
            Subject subject = _subjectDbContext.dbSet.Find(id);
            //get Parent ID of the sons .
            string ancestorsIds = CreateNextchildrenId(subject.ancestorIdSplitStr, id);
            var subjects = (from sub in _subjectDbContext.dbSet select sub);
            //condition of remove if ancestorsIds contain subject ID 
            subjects = subjects.Where(sub => sub.id == id || (sub.ancestorIdSplitStr != null && sub.ancestorIdSplitStr.StartsWith(ancestorsIds)));
            //remove all the subjects 
            foreach (var sub in subjects)
            {
                _subjectDbContext.dbSet.Remove(sub);
            }
            _subjectDbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public List<int> GetChildren(int subjectId)
        {
            var childrenIds = (from subject in _subjectDbContext.dbSet select subject);
            string id = subjectId.ToString();
            //create Query : check if user did the exam
            //id can be in 3 ways : or id, or /id/ or id/
            childrenIds = childrenIds.Where(subject => subject.ancestorIdSplitStr == id 
                                                ||subject.ancestorIdSplitStr.EndsWith("/"+id)
                                                ||subject.ancestorIdSplitStr.Contains("/"+id+"/") 
                                                || subject.ancestorIdSplitStr.Contains(id+"/"));
            return childrenIds.Select(subject => subject.id).ToList();
        }

        //
        //getSubjectParentId:
        //  get the last parent Id
        public int GetSubjectParentId(int subjectId)
        {
            //Find subject 
            Subject subject = _subjectDbContext.dbSet.Find(subjectId);
            string ancestorsIds = subject.ancestorIdSplitStr;
            if(!string.IsNullOrEmpty(ancestorsIds))
            {
                //get ancestor Id and return last one
                List<string> ids= Helper.SeparateByString(ancestorsIds, IdSpliter);
                return int.Parse(ids.Last());
            }
            throw new AncestorNotExistException();
        }
    }

    public class AncestorNotExistException : Exception 
    {
    }
}