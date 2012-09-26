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
    public class RoleController : Controller
    {
        private readonly RoleDbContext           roleDbContext  = new RoleDbContext();
        private readonly JavaScriptSerializer    js             = new JavaScriptSerializer();
        //
        // GET: /Roles/

        public ActionResult Index()
        {
           return RedirectToAction("Edit");
        }

        //
        // GET: /Roles/Details/5

        public ViewResult Details(int id)
        {
            Role role = roleDbContext.dbSet.Find(id);
            return View(role);
        }

        //
        // GET: /Roles/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Roles/Create

        [HttpPost]
        public ActionResult Create(Role role)
        {
            if (ModelState.IsValid)
            {
                roleDbContext.dbSet.Add(role);
                roleDbContext.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(role);
        }

        //
        // GET: /Roles/Delete/5
 
        public ActionResult Delete(int id)
        {
            Role role = roleDbContext.dbSet.Find(id);
            return View(role);
        }

        //
        // POST: /Roles/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Role role = roleDbContext.dbSet.Find(id);
            roleDbContext.dbSet.Remove(role);
            roleDbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // Get: /Roles/Edit
        public ActionResult Edit()
        {
            ViewBag.roles = roleDbContext.dbSet.ToList();
            return View();
        }

        [HttpPost]
        //
        // Post: /Roles/Edit
        public ActionResult Edit(Role newRole, string deleteIdsList)
        {
            // delete check roles
            var idsToDelete = (List<int>)js.Deserialize(deleteIdsList, typeof(List<int>));
            foreach (var id in idsToDelete)
            {          
                Role role = roleDbContext.dbSet.Find(id);
                Roles.DeleteRole(newRole.role);
                roleDbContext.dbSet.Remove(role);
                roleDbContext.SaveChanges();
            }
            
            if (ModelState.IsValid)
            {
                //check if user add new role
                if (!string.IsNullOrEmpty(newRole.role))
                {
                    if (!Roles.RoleExists(newRole.role))
                    {
                        Roles.CreateRole(newRole.role);
                        roleDbContext.dbSet.Add(newRole);
                        roleDbContext.SaveChanges();
                    }
                }
            }
            return RedirectToAction("Edit");
        }
        protected override void Dispose(bool disposing)
        {
            roleDbContext.Dispose();
            base.Dispose(disposing);
        }
    }
}