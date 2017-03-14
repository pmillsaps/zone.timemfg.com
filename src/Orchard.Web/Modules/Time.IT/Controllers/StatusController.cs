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
    [Authorize]
    public class StatusController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: Status
        public ActionResult Index()
        {
            return View(db.Ref_Status.OrderBy(x => x.Status).ToList());
        }

        // GET: Status/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_Status ref_Status = db.Ref_Status.Find(id);
            if (ref_Status == null)
            {
                return HttpNotFound();
            }
            return View(ref_Status);
        }

        // GET: Status/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Status/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] Ref_Status ref_Status)
        {
            if (ModelState.IsValid)
            {
                db.Ref_Status.Add(ref_Status);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ref_Status);
        }

        // GET: Status/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_Status ref_Status = db.Ref_Status.Find(id);
            if (ref_Status == null)
            {
                return HttpNotFound();
            }
            return View(ref_Status);
        }

        // POST: Status/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ref_Status ref_Status)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ref_Status).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ref_Status);
        }

        // GET: Status/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_Status ref_Status = db.Ref_Status.Find(id);
            if (ref_Status == null)
            {
                return HttpNotFound();
            }
            return View(ref_Status);
        }

        // POST: Status/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ref_Status ref_Status = db.Ref_Status.Find(id);
            db.Ref_Status.Remove(ref_Status);
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