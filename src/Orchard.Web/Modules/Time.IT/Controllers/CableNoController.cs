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
    public class CableNoController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: CableNo
        public ActionResult Index()
        {
            return View(db.Ref_CableNo.OrderBy(x => x.Name).ToList());
        }

        // GET: CableNo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_CableNo ref_CableNo = db.Ref_CableNo.Find(id);
            if (ref_CableNo == null)
            {
                return HttpNotFound();
            }
            return View(ref_CableNo);
        }

        // GET: CableNo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CableNo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] Ref_CableNo ref_CableNo)
        {
            if (ModelState.IsValid)
            {
                db.Ref_CableNo.Add(ref_CableNo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ref_CableNo);
        }

        // GET: CableNo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_CableNo ref_CableNo = db.Ref_CableNo.Find(id);
            if (ref_CableNo == null)
            {
                return HttpNotFound();
            }
            return View(ref_CableNo);
        }

        // POST: CableNo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ref_CableNo ref_CableNo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ref_CableNo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ref_CableNo);
        }

        // GET: CableNo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_CableNo ref_CableNo = db.Ref_CableNo.Find(id);
            if (ref_CableNo == null)
            {
                return HttpNotFound();
            }
            return View(ref_CableNo);
        }

        // POST: CableNo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ref_CableNo ref_CableNo = db.Ref_CableNo.Find(id);
            db.Ref_CableNo.Remove(ref_CableNo);
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