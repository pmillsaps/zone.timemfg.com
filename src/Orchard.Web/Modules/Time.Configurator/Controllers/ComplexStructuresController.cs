using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.Configurator;

namespace Time.Configurator.Controllers
{
    [Themed]
    [Authorize]
    public class ComplexStructuresController : Controller
    {        
        private ConfiguratorEntities db = new ConfiguratorEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ComplexStructuresController(IOrchardServices services)
        {
            Services = services;
            db = new ConfiguratorEntities();
        }

        public ComplexStructuresController(IOrchardServices services, ConfiguratorEntities _db)
        {
            Services = services;
            db = _db;
        }



        // GET: ComplexStructures
        public ActionResult Index()
        {
            return View(db.ComplexStructures.ToList());
        }

        // GET: ComplexStructures/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComplexStructure complexStructure = db.ComplexStructures.Find(id);
            if (complexStructure == null)
            {
                return HttpNotFound();
            }
            return View(complexStructure);
        }

        // GET: ComplexStructures/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ComplexStructures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] ComplexStructure complexStructure)
        {
            if (ModelState.IsValid)
            {
                db.ComplexStructures.Add(complexStructure);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(complexStructure);
        }

        // GET: ComplexStructures/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComplexStructure complexStructure = db.ComplexStructures.Find(id);
            if (complexStructure == null)
            {
                return HttpNotFound();
            }
            return View(complexStructure);
        }

        // POST: ComplexStructures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ComplexStructure complexStructure)
        {
            if (ModelState.IsValid)
            {
                db.Entry(complexStructure).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(complexStructure);
        }

        // GET: ComplexStructures/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComplexStructure complexStructure = db.ComplexStructures.Find(id);
            if (complexStructure == null)
            {
                return HttpNotFound();
            }
            return View(complexStructure);
        }

        // POST: ComplexStructures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ComplexStructure complexStructure = db.ComplexStructures.Find(id);
            db.ComplexStructures.Remove(complexStructure);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
