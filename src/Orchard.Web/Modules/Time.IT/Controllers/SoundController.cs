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
    public class SoundController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: Sound
        public ActionResult Index()
        {
            return View(db.Ref_Sound.OrderBy(x => x.Sound).ToList());
        }

        // GET: Sound/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_Sound ref_Sound = db.Ref_Sound.Find(id);
            if (ref_Sound == null)
            {
                return HttpNotFound();
            }
            return View(ref_Sound);
        }

        // GET: Sound/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Sound/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] Ref_Sound ref_Sound)
        {
            if (ModelState.IsValid)
            {
                db.Ref_Sound.Add(ref_Sound);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ref_Sound);
        }

        // GET: Sound/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_Sound ref_Sound = db.Ref_Sound.Find(id);
            if (ref_Sound == null)
            {
                return HttpNotFound();
            }
            return View(ref_Sound);
        }

        // POST: Sound/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ref_Sound ref_Sound)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ref_Sound).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ref_Sound);
        }

        // GET: Sound/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_Sound ref_Sound = db.Ref_Sound.Find(id);
            if (ref_Sound == null)
            {
                return HttpNotFound();
            }
            return View(ref_Sound);
        }

        // POST: Sound/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ref_Sound ref_Sound = db.Ref_Sound.Find(id);
            db.Ref_Sound.Remove(ref_Sound);
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