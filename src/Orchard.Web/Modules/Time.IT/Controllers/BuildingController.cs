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
    public class BuildingController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: Building
        public ActionResult Index()
        {
            return View(db.Ref_Building.OrderBy(x => x.Name).ToList());
        }

        // GET: Building/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_Building ref_Building = db.Ref_Building.Find(id);
            if (ref_Building == null)
            {
                return HttpNotFound();
            }
            return View(ref_Building);
        }

        // GET: Building/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Building/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] Ref_Building ref_Building)
        {
            if (ModelState.IsValid)
            {
                db.Ref_Building.Add(ref_Building);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ref_Building);
        }

        // GET: Building/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_Building ref_Building = db.Ref_Building.Find(id);
            if (ref_Building == null)
            {
                return HttpNotFound();
            }
            return View(ref_Building);
        }

        // POST: Building/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ref_Building ref_Building)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ref_Building).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ref_Building);
        }

        // GET: Building/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_Building ref_Building = db.Ref_Building.Find(id);
            if (ref_Building == null)
            {
                return HttpNotFound();
            }
            return View(ref_Building);
        }

        // POST: Building/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ref_Building ref_Building = db.Ref_Building.Find(id);
            db.Ref_Building.Remove(ref_Building);
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