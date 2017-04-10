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
    public class NICSpeedController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: NICSpeed
        public ActionResult Index()
        {
            return View(db.Ref_NICSpeed.OrderBy(x => x.NIC_Speed).ToList());
        }

        // GET: NICSpeed/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_NICSpeed ref_NICSpeed = db.Ref_NICSpeed.Find(id);
            if (ref_NICSpeed == null)
            {
                return HttpNotFound();
            }
            return View(ref_NICSpeed);
        }

        // GET: NICSpeed/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NICSpeed/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] Ref_NICSpeed ref_NICSpeed)
        {
            if (ModelState.IsValid)
            {
                db.Ref_NICSpeed.Add(ref_NICSpeed);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ref_NICSpeed);
        }

        // GET: NICSpeed/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_NICSpeed ref_NICSpeed = db.Ref_NICSpeed.Find(id);
            if (ref_NICSpeed == null)
            {
                return HttpNotFound();
            }
            return View(ref_NICSpeed);
        }

        // POST: NICSpeed/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ref_NICSpeed ref_NICSpeed)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ref_NICSpeed).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ref_NICSpeed);
        }

        // GET: NICSpeed/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_NICSpeed ref_NICSpeed = db.Ref_NICSpeed.Find(id);
            if (ref_NICSpeed == null)
            {
                return HttpNotFound();
            }
            return View(ref_NICSpeed);
        }

        // POST: NICSpeed/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ref_NICSpeed ref_NICSpeed = db.Ref_NICSpeed.Find(id);
            db.Ref_NICSpeed.Remove(ref_NICSpeed);
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