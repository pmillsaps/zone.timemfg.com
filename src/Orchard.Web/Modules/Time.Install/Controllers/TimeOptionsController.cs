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
using Time.Data.EntityModels.Install;

namespace Time.Install.Controllers
{
    [Themed]
    [Authorize]
    public class TimeOptionsController : Controller
    {
        private VSWQuotesEntities db = new VSWQuotesEntities();

        private string ErrorMessage { get; set; }
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public TimeOptionsController(IOrchardServices services)
        {
            Services = services;
        }

        // GET: TimeOptions
        public ActionResult Index(int? LiftFamilyId)
        {
            ViewBag.LiftFamilyId = new SelectList(db.LiftFamilies, "Id", "FamilyName");
            if (LiftFamilyId != null)
            {
                var timeOptions = db.TimeOptions.Include(t => t.LiftFamily).Where(x => x.LiftFamilyId == LiftFamilyId);
                return View(timeOptions.ToList());
            }
            else
            {
                return View();
            }
            //var timeOptions = db.TimeOptions.Include(t => t.LiftFamily);
            //return View(timeOptions.ToList());
        }

        // GET: TimeOptions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeOption timeOption = db.TimeOptions.Find(id);
            if (timeOption == null)
            {
                return HttpNotFound();
            }
            return View(timeOption);
        }

        // GET: TimeOptions/Create
        public ActionResult Create()
        {
            ViewBag.LiftFamilyId = new SelectList(db.LiftFamilies, "Id", "FamilyName");
            return View();
        }

        // POST: TimeOptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] TimeOption timeOption)
        {
            if (ModelState.IsValid)
            {
                db.TimeOptions.Add(timeOption);
                db.SaveChanges();
                return RedirectToAction("Index", new { LiftFamilyId = timeOption.LiftFamilyId });
            }

            ViewBag.LiftFamilyId = new SelectList(db.LiftFamilies, "Id", "FamilyName", timeOption.LiftFamilyId);
            return View(timeOption);
        }

        // GET: TimeOptions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeOption timeOption = db.TimeOptions.Find(id);
            if (timeOption == null)
            {
                return HttpNotFound();
            }
            ViewBag.LiftFamilyId = new SelectList(db.LiftFamilies, "Id", "FamilyName", timeOption.LiftFamilyId);
            return View(timeOption);
        }

        // POST: TimeOptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TimeOption timeOption)
        {
            if (ModelState.IsValid)
            {
                db.Entry(timeOption).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { LiftFamilyId = timeOption.LiftFamilyId });
            }
            ViewBag.LiftFamilyId = new SelectList(db.LiftFamilies, "Id", "FamilyName", timeOption.LiftFamilyId);
            return View(timeOption);
        }

        // GET: TimeOptions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeOption timeOption = db.TimeOptions.Find(id);
            if (timeOption == null)
            {
                return HttpNotFound();
            }
            return View(timeOption);
        }

        // POST: TimeOptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TimeOption timeOption = db.TimeOptions.Find(id);
            db.TimeOptions.Remove(timeOption);
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
