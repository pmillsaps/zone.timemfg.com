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
    public class MemoryController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: Memory
        public ActionResult Index()
        {
            return View(db.Ref_Memory.OrderBy(x => x.Name).ToList());
        }

        // GET: Memory/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_Memory ref_Memory = db.Ref_Memory.Find(id);
            if (ref_Memory == null)
            {
                return HttpNotFound();
            }
            return View(ref_Memory);
        }

        // GET: Memory/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Memory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] Ref_Memory ref_Memory)
        {
            if (ModelState.IsValid)
            {
                db.Ref_Memory.Add(ref_Memory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ref_Memory);
        }

        // GET: Memory/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_Memory ref_Memory = db.Ref_Memory.Find(id);
            if (ref_Memory == null)
            {
                return HttpNotFound();
            }
            return View(ref_Memory);
        }

        // POST: Memory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ref_Memory ref_Memory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ref_Memory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ref_Memory);
        }

        // GET: Memory/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_Memory ref_Memory = db.Ref_Memory.Find(id);
            if (ref_Memory == null)
            {
                return HttpNotFound();
            }
            return View(ref_Memory);
        }

        // POST: Memory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ref_Memory ref_Memory = db.Ref_Memory.Find(id);
            db.Ref_Memory.Remove(ref_Memory);
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