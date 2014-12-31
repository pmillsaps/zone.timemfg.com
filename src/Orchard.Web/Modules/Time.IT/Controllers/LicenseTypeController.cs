using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.ITInventory;

namespace Time.IT.Controllers
{
    [Themed]
    public class LicenseTypeController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: LicenseType
        public ActionResult Index()
        {
            return View(db.Ref_LicenseType.ToList());
        }

        // GET: LicenseType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_LicenseType ref_LicenseType = db.Ref_LicenseType.Find(id);
            if (ref_LicenseType == null)
            {
                return HttpNotFound();
            }
            return View(ref_LicenseType);
        }

        // GET: LicenseType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LicenseType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LicenseType")] Ref_LicenseType ref_LicenseType)
        {
            if (ModelState.IsValid)
            {
                db.Ref_LicenseType.Add(ref_LicenseType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ref_LicenseType);
        }

        // GET: LicenseType/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_LicenseType ref_LicenseType = db.Ref_LicenseType.Find(id);
            if (ref_LicenseType == null)
            {
                return HttpNotFound();
            }
            return View(ref_LicenseType);
        }

        // POST: LicenseType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,LicenseType")] Ref_LicenseType ref_LicenseType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ref_LicenseType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ref_LicenseType);
        }

        // GET: LicenseType/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_LicenseType ref_LicenseType = db.Ref_LicenseType.Find(id);
            if (ref_LicenseType == null)
            {
                return HttpNotFound();
            }
            return View(ref_LicenseType);
        }

        // POST: LicenseType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ref_LicenseType ref_LicenseType = db.Ref_LicenseType.Find(id);
            db.Ref_LicenseType.Remove(ref_LicenseType);
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
