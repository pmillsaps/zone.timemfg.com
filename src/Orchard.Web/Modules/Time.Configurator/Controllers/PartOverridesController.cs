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
    public class PartOverridesController : Controller
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public PartOverridesController(IOrchardServices services)
        {
            Services = services;
            db = new ConfiguratorEntities();
        }

        public PartOverridesController(IOrchardServices services, ConfiguratorEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: PartOverrides
        public ActionResult Index()
        {
            return View(db.PartOverrides.ToList());
        }

        // GET: PartOverrides/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PartOverride partOverride = db.PartOverrides.Find(id);
            if (partOverride == null)
            {
                return HttpNotFound();
            }
            return View(partOverride);
        }

        // GET: PartOverrides/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PartOverrides/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PartNum,Description")] PartOverride partOverride)
        {
            if (ModelState.IsValid)
            {
                db.PartOverrides.Add(partOverride);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(partOverride);
        }

        // GET: PartOverrides/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PartOverride partOverride = db.PartOverrides.Find(id);
            if (partOverride == null)
            {
                return HttpNotFound();
            }
            return View(partOverride);
        }

        // POST: PartOverrides/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PartNum,Description")] PartOverride partOverride)
        {
            if (ModelState.IsValid)
            {
                db.Entry(partOverride).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(partOverride);
        }

        // GET: PartOverrides/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PartOverride partOverride = db.PartOverrides.Find(id);
            if (partOverride == null)
            {
                return HttpNotFound();
            }
            return View(partOverride);
        }

        // POST: PartOverrides/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            PartOverride partOverride = db.PartOverrides.Find(id);
            db.PartOverrides.Remove(partOverride);
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
