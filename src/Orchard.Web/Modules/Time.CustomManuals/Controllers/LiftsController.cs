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
    public class LiftsController : Controller
    {
        private CustomManualsEntities db = new CustomManualsEntities();

        // GET: Lifts
        public ActionResult Index()
        {
            var lifts = db.Lifts.Include(l => l.LiftGroup);
            return View(lifts.ToList());
        }

        //Not used
        // GET: Lifts/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lift lift = db.Lifts.Find(id);
            if (lift == null)
            {
                return HttpNotFound();
            }
            return View(lift);
        }

        // GET: Lifts/Create
        public ActionResult Create()
        {
            ViewBag.Lift_Group = new SelectList(db.LiftGroups.OrderBy(x => x.Lift_Group), "Lift_Group", "Lift_Group");
            return View();
        }

        // POST: Lifts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Lift_ID,Lift_Group")] Lift lift)
        {
            var dupeCheck = db.Lifts.FirstOrDefault(x => x.Lift_Group == lift.Lift_Group && x.Lift_ID == lift.Lift_ID);
            if (dupeCheck != null) ModelState.AddModelError("", "Duplicate Lift Groups and Lift IDs are not Allowed...");

            if (ModelState.IsValid)
            {
                db.Lifts.Add(lift);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Lift_Group = new SelectList(db.LiftGroups.OrderBy(x => x.Lift_Group), "Lift_Group", "Lift_Group");
            return View(lift);
        }

        // GET: LiftGroups/CreateGroup
        public ActionResult CreateGroup()
        {
            return View();
        }

        // POST: LiftGroups/CreateGroup
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateGroup([Bind(Include = "Lift_ID,Lift_Group")] LiftGroup liftGroup)
        {
            var dupeCheck = db.LiftGroups.FirstOrDefault(x => x.Lift_Group == liftGroup.Lift_Group);
            if (dupeCheck != null) ModelState.AddModelError("", "Duplicate Lift Groups are not Allowed...");

            if (ModelState.IsValid)
            {
                db.LiftGroups.Add(liftGroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(liftGroup);
        }

        // GET: Lifts/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lift lift = db.Lifts.Find(id);
            if (lift == null)
            {
                return HttpNotFound();
            }
            ViewBag.Lift_Group = new SelectList(db.LiftGroups, "Lift_Group", "Lift_Group", lift.Lift_Group);
            return View(lift);
        }

        // POST: Lifts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Lift_ID,Lift_Group")] Lift lift)
        {
            var dupeCheck = db.Lifts.FirstOrDefault(x => x.Lift_Group == lift.Lift_Group && x.Lift_ID == lift.Lift_ID);
            if (dupeCheck != null) ModelState.AddModelError("", "Duplicate Lift Groups and Lift IDs are not Allowed...");

            if (ModelState.IsValid)
            {
                db.Entry(lift).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Lift_Group = new SelectList(db.LiftGroups, "Lift_Group", "Lift_Group", lift.Lift_Group);
            return View(lift);
        }

        // GET: Lifts/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lift lift = db.Lifts.Find(id);
            if (lift == null)
            {
                return HttpNotFound();
            }
            return View(lift);
        }

        // POST: Lifts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Lift lift = db.Lifts.Find(id);
            db.Lifts.Remove(lift);
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
