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
    public class ConfigOptionsController : Controller
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ConfigOptionsController(IOrchardServices services)
        {
            Services = services;
            db = new ConfiguratorEntities();
        }

        public ConfigOptionsController(IOrchardServices services, ConfiguratorEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: ConfigOptions
        public ActionResult Index(string display, string ConfigNames, string ConfigData)
        {
            ViewBag.ConfigNames = new SelectList(db.ConfiguratorNames.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName");
            ViewBag.ConfigData = new SelectList(db.ConfigOptions.Select(x => new { x.ConfigData }).Distinct().OrderBy(x => x.ConfigData), "ConfigData", "ConfigData");

            if (String.IsNullOrEmpty(ConfigNames) && String.IsNullOrEmpty(ConfigData))
            {
                return View();
            }
            else
            {
                var configOptions = db.ConfigOptions.AsQueryable();
                if (!String.IsNullOrEmpty(ConfigNames)) configOptions = configOptions.Where(x => x.ConfigName == ConfigNames);
                if (!String.IsNullOrEmpty(ConfigData)) configOptions = configOptions.Where(x => x.ConfigData == ConfigData);
                if (display == "New") configOptions = configOptions.Where(x => x.PartNum.Contains("-NEW"));
                if (display == "Eng") configOptions = configOptions.Where(x => x.PartNum.Contains("-ENG"));

                return View(configOptions.OrderBy(x => x.ConfigName).ThenBy(x => x.ConfigData).ToList());
            }
        }

        // GET: ConfigOptions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigOption configOption = db.ConfigOptions.Find(id);
            if (configOption == null)
            {
                return HttpNotFound();
            }
            return View(configOption);
        }

        // GET: ConfigOptions/Create
        public ActionResult Create()
        {
            ViewBag.ConfigName = new SelectList(db.ConfiguratorNames.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName");
            return View();
        }

        // POST: ConfigOptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] ConfigOption configOption)
        {
            //prevents a duplicate from being created
            //duplicate checking
            var Configs = db.ConfigOptions.FirstOrDefault(x => x.ConfigName == configOption.ConfigName && x.ConfigData == configOption.ConfigData
                                                            && x.Key01 == configOption.Key01 && x.Key02 == configOption.Key02 && x.Key03 == configOption.Key03
                                                            && x.Key04 == configOption.Key04 && x.Key05 == configOption.Key05 && x.Key06 == configOption.Key06
                                                            && x.Key07 == configOption.Key07 && x.Key08 == configOption.Key08 && x.Key09 == configOption.Key09
                                                            && x.Key10 == configOption.Key10 && x.PartNum == configOption.PartNum 
                                                            && x.InActive == configOption.InActive);

            //displays if previous code found a duplicate
            if (Configs != null) ModelState.AddModelError("", "ConfigOption already exists---Please Recheck Data");

            if (ModelState.IsValid)
            {
                db.ConfigOptions.Add(configOption);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ConfigName = new SelectList(db.ConfiguratorNames.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName");
            return View(configOption);
        }

        // GET: ConfigOptions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigOption configOption = db.ConfigOptions.Find(id);
            if (configOption == null)
            {
                return HttpNotFound();
            }
            return View(configOption);
        }

        // POST: ConfigOptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ConfigOption configOption)
        {
            if (ModelState.IsValid)
            {
                db.Entry(configOption).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { ConfigNames = configOption.ConfigName, ConfigData = configOption.ConfigData});
            }
            return View(configOption);
        }

        // GET: ConfigOptions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigOption configOption = db.ConfigOptions.Find(id);
            if (configOption == null)
            {
                return HttpNotFound();
            }
            return View(configOption);
        }

        // POST: ConfigOptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ConfigOption configOption = db.ConfigOptions.Find(id);
            db.ConfigOptions.Remove(configOption);
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
