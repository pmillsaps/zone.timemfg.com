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
    public class ComplexLookupsController : Controller
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        // GET: ComplexLookups
        public ActionResult Index()
        {
            return View(db.ComplexLookups.ToList());
        }

        // GET: ComplexLookups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComplexLookup complexLookup = db.ComplexLookups.Find(id);
            if (complexLookup == null)
            {
                return HttpNotFound();
            }
            return View(complexLookup);
        }

        // GET: ComplexLookups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ComplexLookups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ConfigName,ConfigData,LookupData")] ComplexLookup complexLookup)
        {
            if (ModelState.IsValid)
            {
                db.ComplexLookups.Add(complexLookup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(complexLookup);
        }

        // GET: ComplexLookups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComplexLookup complexLookup = db.ComplexLookups.Find(id);
            if (complexLookup == null)
            {
                return HttpNotFound();
            }
            return View(complexLookup);
        }

        // POST: ComplexLookups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ConfigName,ConfigData,LookupData")] ComplexLookup complexLookup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(complexLookup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(complexLookup);
        }

        // GET: ComplexLookups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComplexLookup complexLookup = db.ComplexLookups.Find(id);
            if (complexLookup == null)
            {
                return HttpNotFound();
            }
            return View(complexLookup);
        }

        // POST: ComplexLookups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ComplexLookup complexLookup = db.ComplexLookups.Find(id);
            db.ComplexLookups.Remove(complexLookup);
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
