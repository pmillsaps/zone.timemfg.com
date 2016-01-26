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
    public class MonitorController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: Monitor
        public ActionResult Index()
        {
            var monitors = db.Monitors.OrderBy(x => x.Ref_Manufacturer.Name).ThenBy(x => x.SerialNo).Include(m => m.Ref_Manufacturer).Include(m => m.Ref_MonitorSizes).Include(m => m.User);
            return View(monitors.ToList());
        }

        // GET: Monitor/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Monitor monitor = db.Monitors.Find(id);
            if (monitor == null)
            {
                return HttpNotFound();
            }
            return View(monitor);
        }

        // GET: Monitor/Create
        public ActionResult Create()
        {
            GetDropDowns(new Monitor());
            return View();
        }

        // POST: Monitor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] Monitor monitor)
        {
            if (ModelState.IsValid)
            {
                db.Monitors.Add(monitor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            GetDropDowns(monitor);
            return View(monitor);
        }

        private void GetDropDowns(Monitor monitor)
        {
            ViewBag.ManufacturerId = new SelectList(db.Ref_Manufacturer.OrderBy(x => x.Name), "Id", "Name", monitor.ManufacturerId);
            ViewBag.SizeId = new SelectList(db.Ref_MonitorSizes.OrderBy(x => x.Size), "Id", "Size", monitor.SizeId);
            ViewBag.UserId = new SelectList(db.Users.OrderBy(x => x.Name), "Id", "Name", monitor.UserId);
        }

        // GET: Monitor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Monitor monitor = db.Monitors.Find(id);
            if (monitor == null)
            {
                return HttpNotFound();
            }
            GetDropDowns(monitor);
            return View(monitor);
        }

        // POST: Monitor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Monitor monitor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(monitor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GetDropDowns(monitor);
            return View(monitor);
        }

        // GET: Monitor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Monitor monitor = db.Monitors.Find(id);
            if (monitor == null)
            {
                return HttpNotFound();
            }
            return View(monitor);
        }

        // POST: Monitor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Monitor monitor = db.Monitors.Find(id);
            db.Monitors.Remove(monitor);
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
