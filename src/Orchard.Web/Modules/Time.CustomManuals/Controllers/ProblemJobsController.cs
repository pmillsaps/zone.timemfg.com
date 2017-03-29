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
    [Themed]
    public class ProblemJobsController : Controller
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ProblemJobsController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
        }

        private CustomManualsEntities db = new CustomManualsEntities();

        // GET: ProblemJobs
        public ActionResult Index()
        {
            ViewBag.Title = "Open Problem Jobs";
            return View(db.ProblemJobs.Where(x => x.JobCleared != true).OrderBy(x => x.SerialNumber).ToList());
        }

        // GET: ProblemJobs
        public ActionResult Ready()
        {
            ViewBag.Title = "Ready to Go Problem Jobs";
            return View("Index", db.ProblemJobs.Where(x => x.JobCleared != true && x.LastCheckedMessage.Contains("run if possible")).OrderBy(x => x.SerialNumber).ToList());
        }

        // GET: ProblemJobs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProblemJob problemJob = db.ProblemJobs.Find(id);
            if (problemJob == null)
            {
                return HttpNotFound();
            }
            return View(problemJob);
        }

        // GET: ProblemJobs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProblemJobs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,JobNumber,SerialNumber,EnteredDate,EntryPerson,Problem,Language,Language_Code,RequiredDate,LastCheckedDate,LastCheckedPerson,LastCheckedMessage,JobCleared,JobClearedDate,JobClearedBy,ManuallyCleared,ManuallyClearedBy,ManuallyClearedNote,AutomaticEntryReason")] ProblemJob problemJob)
        {
            if (ModelState.IsValid)
            {
                db.ProblemJobs.Add(problemJob);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(problemJob);
        }

        // GET: ProblemJobs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProblemJob problemJob = db.ProblemJobs.Find(id);
            if (problemJob == null)
            {
                return HttpNotFound();
            }
            return View(problemJob);
        }

        // POST: ProblemJobs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,JobNumber,SerialNumber,EnteredDate,EntryPerson,Problem,Language,Language_Code,RequiredDate,LastCheckedDate,LastCheckedPerson,LastCheckedMessage,JobCleared,JobClearedDate,JobClearedBy,ManuallyCleared,ManuallyClearedBy,ManuallyClearedNote,AutomaticEntryReason")] ProblemJob problemJob)
        {
            if (ModelState.IsValid)
            {
                db.Entry(problemJob).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(problemJob);
        }

        // GET: ProblemJobs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProblemJob problemJob = db.ProblemJobs.Find(id);
            if (problemJob == null)
            {
                return HttpNotFound();
            }
            return View(problemJob);
        }

        // POST: ProblemJobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProblemJob problemJob = db.ProblemJobs.Find(id);
            db.ProblemJobs.Remove(problemJob);
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