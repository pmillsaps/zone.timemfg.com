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
    public class LocationController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: Location
        public ActionResult Index()
        {
            return View(db.Ref_Location.OrderBy(x => x.Name).ToList());
        }

        // GET: Location/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_Location ref_Location = db.Ref_Location.Find(id);
            if (ref_Location == null)
            {
                return HttpNotFound();
            }
            return View(ref_Location);
        }

        // GET: Location/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Location/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] Ref_Location ref_Location)
        {
            if (ModelState.IsValid)
            {
                db.Ref_Location.Add(ref_Location);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ref_Location);
        }

        // GET: Location/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_Location ref_Location = db.Ref_Location.Find(id);
            if (ref_Location == null)
            {
                return HttpNotFound();
            }
            return View(ref_Location);
        }

        // POST: Location/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ref_Location ref_Location)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ref_Location).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ref_Location);
        }

        // GET: Location/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_Location ref_Location = db.Ref_Location.Find(id);
            if (ref_Location == null)
            {
                return HttpNotFound();
            }
            return View(ref_Location);
        }

        // POST: Location/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ref_Location ref_Location = db.Ref_Location.Find(id);
            db.Ref_Location.Remove(ref_Location);
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