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
    public class InstallHourlyRatesController : Controller
    {
        private string ErrorMessage { get; set; }
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public InstallHourlyRatesController(IOrchardServices services)
        {
            Services = services;
        }
        private VSWQuotesEntities db = new VSWQuotesEntities();

        // GET: InstallHourlyRates
        public ActionResult Index()
        {
            return View(db.InstallHourlyRates.ToList());
        }

        // GET: InstallHourlyRates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstallHourlyRate installHourlyRate = db.InstallHourlyRates.Find(id);
            if (installHourlyRate == null)
            {
                return HttpNotFound();
            }
            return View(installHourlyRate);
        }

        // GET: InstallHourlyRates/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InstallHourlyRates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] InstallHourlyRate installHourlyRate)
        {
            if (ModelState.IsValid)
            {
                db.InstallHourlyRates.Add(installHourlyRate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(installHourlyRate);
        }

        // GET: InstallHourlyRates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstallHourlyRate installHourlyRate = db.InstallHourlyRates.Find(id);
            if (installHourlyRate == null)
            {
                return HttpNotFound();
            }
            return View(installHourlyRate);
        }

        // POST: InstallHourlyRates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(InstallHourlyRate installHourlyRate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(installHourlyRate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(installHourlyRate);
        }

        // GET: InstallHourlyRates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstallHourlyRate installHourlyRate = db.InstallHourlyRates.Find(id);
            if (installHourlyRate == null)
            {
                return HttpNotFound();
            }
            return View(installHourlyRate);
        }

        // POST: InstallHourlyRates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InstallHourlyRate installHourlyRate = db.InstallHourlyRates.Find(id);
            db.InstallHourlyRates.Remove(installHourlyRate);
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
