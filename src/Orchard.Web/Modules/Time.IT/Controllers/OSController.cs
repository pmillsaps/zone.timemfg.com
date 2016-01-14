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
    public class OSController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: OS
        public ActionResult Index()
        {
            return View(db.Ref_OS.OrderBy(x => x.OS).ToList());
        }

        // GET: OS/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_OS ref_OS = db.Ref_OS.Find(id);
            if (ref_OS == null)
            {
                return HttpNotFound();
            }
            return View(ref_OS);
        }

        // GET: OS/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OS/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] Ref_OS ref_OS)
        {
            if (ModelState.IsValid)
            {
                db.Ref_OS.Add(ref_OS);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ref_OS);
        }

        // GET: OS/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_OS ref_OS = db.Ref_OS.Find(id);
            if (ref_OS == null)
            {
                return HttpNotFound();
            }
            return View(ref_OS);
        }

        // POST: OS/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ref_OS ref_OS)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ref_OS).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ref_OS);
        }

        // GET: OS/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_OS ref_OS = db.Ref_OS.Find(id);
            if (ref_OS == null)
            {
                return HttpNotFound();
            }
            return View(ref_OS);
        }

        // POST: OS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ref_OS ref_OS = db.Ref_OS.Find(id);
            db.Ref_OS.Remove(ref_OS);
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