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
    public class MonitorSizeController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: MonitorSize
        public ActionResult Index()
        {
            return View(db.Ref_MonitorSizes.OrderBy(x => x.Size).ToList());
        }

        // GET: MonitorSize/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_MonitorSizes ref_MonitorSizes = db.Ref_MonitorSizes.Find(id);
            if (ref_MonitorSizes == null)
            {
                return HttpNotFound();
            }
            return View(ref_MonitorSizes);
        }

        // GET: MonitorSize/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MonitorSize/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] Ref_MonitorSizes ref_MonitorSizes)
        {
            if (ModelState.IsValid)
            {
                db.Ref_MonitorSizes.Add(ref_MonitorSizes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ref_MonitorSizes);
        }

        // GET: MonitorSize/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_MonitorSizes ref_MonitorSizes = db.Ref_MonitorSizes.Find(id);
            if (ref_MonitorSizes == null)
            {
                return HttpNotFound();
            }
            return View(ref_MonitorSizes);
        }

        // POST: MonitorSize/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ref_MonitorSizes ref_MonitorSizes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ref_MonitorSizes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ref_MonitorSizes);
        }

        // GET: MonitorSize/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_MonitorSizes ref_MonitorSizes = db.Ref_MonitorSizes.Find(id);
            if (ref_MonitorSizes == null)
            {
                return HttpNotFound();
            }
            return View(ref_MonitorSizes);
        }

        // POST: MonitorSize/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ref_MonitorSizes ref_MonitorSizes = db.Ref_MonitorSizes.Find(id);
            db.Ref_MonitorSizes.Remove(ref_MonitorSizes);
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