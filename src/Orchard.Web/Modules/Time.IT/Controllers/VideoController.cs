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
    public class VideoController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: Video
        public ActionResult Index()
        {
            return View(db.Ref_VideoCard.OrderBy(x => x.VideoCard).ToList());
        }

        // GET: Video/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_VideoCard ref_VideoCard = db.Ref_VideoCard.Find(id);
            if (ref_VideoCard == null)
            {
                return HttpNotFound();
            }
            return View(ref_VideoCard);
        }

        // GET: Video/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Video/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] Ref_VideoCard ref_VideoCard)
        {
            if (ModelState.IsValid)
            {
                db.Ref_VideoCard.Add(ref_VideoCard);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ref_VideoCard);
        }

        // GET: Video/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_VideoCard ref_VideoCard = db.Ref_VideoCard.Find(id);
            if (ref_VideoCard == null)
            {
                return HttpNotFound();
            }
            return View(ref_VideoCard);
        }

        // POST: Video/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ref_VideoCard ref_VideoCard)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ref_VideoCard).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ref_VideoCard);
        }

        // GET: Video/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_VideoCard ref_VideoCard = db.Ref_VideoCard.Find(id);
            if (ref_VideoCard == null)
            {
                return HttpNotFound();
            }
            return View(ref_VideoCard);
        }

        // POST: Video/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ref_VideoCard ref_VideoCard = db.Ref_VideoCard.Find(id);
            db.Ref_VideoCard.Remove(ref_VideoCard);
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