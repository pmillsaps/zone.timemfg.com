using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.CustomManuals;

namespace Time.CustomManuals.Controllers
{
    //sets theme and requires you to log in to go to the page
    [Themed]
    [Authorize]
    public class LiftOptionsController : Controller
    {
        private CustomManualsEntities db = new CustomManualsEntities();

        // GET: LiftOptions
        public ActionResult Index()
        {
            return View(db.LiftOptions.OrderBy(x => x.OptionNo).ToList());
        }

        // GET: LiftOptions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiftOption liftOption = db.LiftOptions.Find(id);
            if (liftOption == null)
            {
                return HttpNotFound();
            }
            return View(liftOption);
        }

        // GET: LiftOptions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LiftOptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,OptionNo,Title,ShortTitle,Active,DistributorViewable")] LiftOption liftOption)
        {
            if (ModelState.IsValid)
            {
                db.LiftOptions.Add(liftOption);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(liftOption);
        }

        // GET: LiftOptions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiftOption liftOption = db.LiftOptions.Find(id);
            if (liftOption == null)
            {
                return HttpNotFound();
            }
            return View(liftOption);
        }

        // POST: LiftOptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OptionNo,Title,ShortTitle,Active,DistributorViewable")] LiftOption liftOption)
        {
            if (ModelState.IsValid)
            {
                db.Entry(liftOption).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(liftOption);
        }

        // GET: LiftOptions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiftOption liftOption = db.LiftOptions.Find(id);
            if (liftOption == null)
            {
                return HttpNotFound();
            }
            return View(liftOption);
        }

        // POST: LiftOptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LiftOption liftOption = db.LiftOptions.Find(id);
            db.LiftOptions.Remove(liftOption);
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
