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
    public class ManufacturerController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: Manufacturer
        public ActionResult Index()
        {
            return View(db.Ref_Manufacturer.OrderBy(x => x.Name).ToList());
        }

        // GET: Manufacturer/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_Manufacturer ref_Manufacturer = db.Ref_Manufacturer.Find(id);
            if (ref_Manufacturer == null)
            {
                return HttpNotFound();
            }
            return View(ref_Manufacturer);
        }

        // GET: Manufacturer/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Manufacturer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] Ref_Manufacturer ref_Manufacturer)
        {
            if (ModelState.IsValid)
            {
                db.Ref_Manufacturer.Add(ref_Manufacturer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ref_Manufacturer);
        }

        // GET: Manufacturer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_Manufacturer ref_Manufacturer = db.Ref_Manufacturer.Find(id);
            if (ref_Manufacturer == null)
            {
                return HttpNotFound();
            }
            return View(ref_Manufacturer);
        }

        // POST: Manufacturer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ref_Manufacturer ref_Manufacturer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ref_Manufacturer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ref_Manufacturer);
        }

        // GET: Manufacturer/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_Manufacturer ref_Manufacturer = db.Ref_Manufacturer.Find(id);
            if (ref_Manufacturer == null)
            {
                return HttpNotFound();
            }
            return View(ref_Manufacturer);
        }

        // POST: Manufacturer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ref_Manufacturer ref_Manufacturer = db.Ref_Manufacturer.Find(id);
            db.Ref_Manufacturer.Remove(ref_Manufacturer);
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