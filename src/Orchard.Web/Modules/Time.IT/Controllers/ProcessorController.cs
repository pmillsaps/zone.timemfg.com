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
    public class ProcessorController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: Processor
        public ActionResult Index()
        {
            return View(db.Ref_Processor.OrderBy(x => x.Processor).ToList());
        }

        // GET: Processor/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_Processor ref_Processor = db.Ref_Processor.Find(id);
            if (ref_Processor == null)
            {
                return HttpNotFound();
            }
            return View(ref_Processor);
        }

        // GET: Processor/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Processor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] Ref_Processor ref_Processor)
        {
            if (ModelState.IsValid)
            {
                db.Ref_Processor.Add(ref_Processor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ref_Processor);
        }

        // GET: Processor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_Processor ref_Processor = db.Ref_Processor.Find(id);
            if (ref_Processor == null)
            {
                return HttpNotFound();
            }
            return View(ref_Processor);
        }

        // POST: Processor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ref_Processor ref_Processor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ref_Processor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ref_Processor);
        }

        // GET: Processor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_Processor ref_Processor = db.Ref_Processor.Find(id);
            if (ref_Processor == null)
            {
                return HttpNotFound();
            }
            return View(ref_Processor);
        }

        // POST: Processor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ref_Processor ref_Processor = db.Ref_Processor.Find(id);
            db.Ref_Processor.Remove(ref_Processor);
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