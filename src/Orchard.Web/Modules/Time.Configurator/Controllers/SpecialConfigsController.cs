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
using Time.Data.EntityModels.Configurator;

namespace Time.Configurator.Controllers
{
    //sets theme and requires you to log in to go to the page
    [Themed]
    [Authorize]
    public class SpecialConfigsController : Controller
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        //private string[] tokens;

        public SpecialConfigsController(IOrchardServices services)
        {
            Services = services;
            db = new ConfiguratorEntities();
        }

        public SpecialConfigsController(IOrchardServices services, ConfiguratorEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: SpecialConfigs
        public ActionResult Index()
        {
            return View(db.SpecialConfigs.ToList());
        }

        // GET: SpecialConfigs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpecialConfig specialConfig = db.SpecialConfigs.Find(id);
            if (specialConfig == null)
            {
                return HttpNotFound();
            }
            return View(specialConfig);
        }

        // GET: SpecialConfigs/Create
        public ActionResult Create()
        {
            ViewBag.SpecialCustomerId = new SelectList(db.SpecialCustomers, "Id", "Name");
            return View();
        }

        // POST: SpecialConfigs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] SpecialConfig specialConfig)
        {
            //prevent duplicates when creating
            var Configs = db.SpecialConfigs.FirstOrDefault(x => x.Name == specialConfig.Name);

            //displays if previous code found a duplicate
            if (Configs != null) ModelState.AddModelError("", "Duplicate Config Created---Please Check Data");

            if (ModelState.IsValid)
            {
                db.SpecialConfigs.Add(specialConfig);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SpecialCustomerId = new SelectList(db.SpecialCustomers, "Id", "Name", specialConfig.SpecialCustomerId);
            return View(specialConfig);
        }

        // GET: SpecialConfigs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpecialConfig specialConfig = db.SpecialConfigs.Find(id);
            if (specialConfig == null)
            {
                return HttpNotFound();
            }
            ViewBag.SpecialCustomerId = new SelectList(db.SpecialCustomers, "Id", "Name", specialConfig.SpecialCustomerId);
            return View(specialConfig);
        }

        // POST: SpecialConfigs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SpecialConfig specialConfig)
        {
            //prevents data from being duplicated when editing
            var sConfigs = db.SpecialConfigs.FirstOrDefault(x => x.Name == specialConfig.Name && x.SpecialCustomerId == specialConfig.SpecialCustomerId && x.Id != specialConfig.Id);

            //displays if previous code found a duplicate
            if (sConfigs != null) ModelState.AddModelError("", "Duplicate Special Config Created---Please Check Data");

            if (ModelState.IsValid)
            {
                db.Entry(specialConfig).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SpecialCustomerId = new SelectList(db.SpecialCustomers, "Id", "Name", specialConfig.SpecialCustomerId);
            return View(specialConfig);
        }

        // GET: SpecialConfigs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpecialConfig specialConfig = db.SpecialConfigs.Find(id);
            if (specialConfig == null)
            {
                return HttpNotFound();
            }
            return View(specialConfig);
        }

        // POST: SpecialConfigs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SpecialConfig specialConfig = db.SpecialConfigs.Find(id);
            db.SpecialConfigs.Remove(specialConfig);
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