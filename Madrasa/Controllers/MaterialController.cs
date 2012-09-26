using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Madrasa.Models;
using System.Web.Script.Serialization;

namespace Madrasa.Controllers
{ 
    public class MaterialController : Controller
    {
        private readonly MaterialDbContext      _materialDbContext   = new MaterialDbContext();
        private readonly SubjectController      _subjects            = new SubjectController();   //To get Available subjects
        private readonly JavaScriptSerializer   _js                  = new JavaScriptSerializer();


        //
        // GET: /Materials/
        //
        public ViewResult Index()
        {
            return View(_materialDbContext.dbSet.ToList());
        }

        //
        // GET: /Materials/Details/5

        public ViewResult Details(int id)
        {
            Material material = _materialDbContext.dbSet.Find(id);
            return View(material);
        }

        //
        // GET: /Materials/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Materials/Create

        [HttpPost]
        public ActionResult Create(Material material)
        {
            if (ModelState.IsValid)
            {
                _materialDbContext.dbSet.Add(material);
                _materialDbContext.SaveChanges();
                return RedirectToAction("Index");  
            }
            return View(material);
        }
        
        //
        // GET: /Materials/Edit/5
        //
        public ActionResult Edit(int id)
        {
            Material material = _materialDbContext.dbSet.Find(id);
            return View(material);
        }

        //
        // POST: /Materials/Edit/5
        //
        [HttpPost]
        public ActionResult Edit(Material material)
        {
            if (ModelState.IsValid)
            {
                _materialDbContext.Entry(material).State = EntityState.Modified;
                _materialDbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(material);
        }

        //
        // GET: /Materials/Delete/5
 
        public ActionResult Delete(int id)
        {
            Material material = _materialDbContext.dbSet.Find(id);
            return View(material);
        }

        //
        // POST: /Materials/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Material material = _materialDbContext.dbSet.Find(id);
            _materialDbContext.dbSet.Remove(material);
            _materialDbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // GET: /Materials/AddMaterial

        public ActionResult AddMaterial()
        {
            List<SubjectView> subjectViewList = this._subjects.GetSubjectViewList();
            ViewBag.subjects = subjectViewList;
            return View();
        }

        //
        // POST: /Materials/AddMaterial
        //
        [HttpPost]
        public ActionResult AddMaterial(Material material)
        {
            if (ModelState.IsValid)
            {
                _materialDbContext.dbSet.Add(material);
                _materialDbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(material);
        }

        //
        // GET: /Materials/ViewMaterial
        //
        public ActionResult ViewMaterial()
        {
            List<SubjectView> subjectViewList = this._subjects.GetSubjectViewList();
            ViewBag.subjects = subjectViewList;
            // send list of questions for View
            List<Material> materialList = _materialDbContext.dbSet.ToList();
            return View(materialList);
        }

        //
        // POST: /Materials/ViewMaterial
        //
        [HttpPost]
        public ActionResult ViewMaterial(string deleteIdsList)
        {
            List<int> idsToDelete = (List<int>)_js.Deserialize(deleteIdsList, typeof(List<int>));
            foreach (int id in idsToDelete) {
                Material material = _materialDbContext.dbSet.Find(id);
                _materialDbContext.dbSet.Remove(material);
                _materialDbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        //
        //
        public Material GetMaterialBySubjectId(int subjectId)
        {
            var materials = (from material in _materialDbContext.dbSet select material).Where(material => material.subjectId == subjectId);
            if (!materials.Any()) {
                return null;
            }
            return materials.First();
        }

        // TODO: why is this here?
        protected override void Dispose(bool disposing)
        {
            _materialDbContext.Dispose();
            base.Dispose(disposing);
        }
    }
}