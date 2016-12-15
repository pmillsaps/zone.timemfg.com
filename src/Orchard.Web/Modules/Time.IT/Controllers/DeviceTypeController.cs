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
    public class DeviceTypeController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: DeviceType
        public ActionResult Index()
        {
            return View(db.Ref_DeviceType.OrderBy(x => x.DeviceType).ToList());
        }

        // GET: DeviceType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_DeviceType ref_DeviceType = db.Ref_DeviceType.Find(id);
            if (ref_DeviceType == null)
            {
                return HttpNotFound();
            }
            return View(ref_DeviceType);
        }

        // GET: DeviceType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DeviceType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] Ref_DeviceType ref_DeviceType)
        {
            if (ModelState.IsValid)
            {
                db.Ref_DeviceType.Add(ref_DeviceType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ref_DeviceType);
        }

        // GET: DeviceType/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_DeviceType ref_DeviceType = db.Ref_DeviceType.Find(id);
            if (ref_DeviceType == null)
            {
                return HttpNotFound();
            }
            return View(ref_DeviceType);
        }

        // POST: DeviceType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ref_DeviceType ref_DeviceType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ref_DeviceType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ref_DeviceType);
        }

        // GET: DeviceType/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_DeviceType ref_DeviceType = db.Ref_DeviceType.Find(id);
            if (ref_DeviceType == null)
            {
                return HttpNotFound();
            }
            return View(ref_DeviceType);
        }

        // POST: DeviceType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ref_DeviceType ref_DeviceType = db.Ref_DeviceType.Find(id);
            db.Ref_DeviceType.Remove(ref_DeviceType);
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