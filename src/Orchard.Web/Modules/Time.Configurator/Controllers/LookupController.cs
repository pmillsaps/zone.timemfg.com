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
    [Themed]
    [Authorize]
    public class LookupController : Controller
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public LookupController(IOrchardServices services)
        {
            Services = services;
            db = new ConfiguratorEntities();
        }

        public LookupController(IOrchardServices services, ConfiguratorEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: /Lookup/
        public ActionResult Index(string ConfigNames, string ConfigData)
        {
            var lookups = db.Lookups.OrderBy(x => x.ConfigName).ThenBy(x => x.ConfigData).ThenBy(x => x.Data).ToList();
            // Creates the drop down list for ConfigName in the view
            var ddlConfigNames = db.Lookups.Select(x => x.ConfigName).Distinct();
            List<SelectListItem> configNames = new List<SelectListItem>();
            foreach (var item in ddlConfigNames)
            {
                configNames.Add(new SelectListItem { Text = item, Value = item });
            }
            ViewBag.ConfigNames = configNames;
            // Creates the drop down list for ConfigData in the view
            var ddlConfigD = db.Lookups.Select(x => x.ConfigData).Distinct();
            List<SelectListItem> configData = new List<SelectListItem>();
            foreach (var item in ddlConfigD)
            {
                configData.Add(new SelectListItem { Text = item, Value = item });
            }
            ViewBag.ConfigData = configData;

            // Returning the data based on the search filter
            if (!String.IsNullOrEmpty(ConfigNames))
            {
                if (!String.IsNullOrEmpty(ConfigData))
                {
                    lookups = db.Lookups.Where(x => x.ConfigName == ConfigNames && x.ConfigData == ConfigData).OrderBy(x => x.ConfigName).ThenBy(x => x.ConfigData).ToList();

                    return View(lookups);
                }

                lookups = db.Lookups.Where(x => x.ConfigName == ConfigNames).OrderBy(x => x.ConfigName).ThenBy(x => x.ConfigData).ToList();

                return View(lookups);
            }
            else if (!String.IsNullOrEmpty(ConfigData))
            {
                lookups = db.Lookups.Where(x => x.ConfigData == ConfigData).OrderBy(x => x.ConfigName).ThenBy(x => x.ConfigData).ToList();

                return View(lookups);
            }
            else
            {
                return View(lookups);
            }
        }

        // GET: /Lookup/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lookup lookup = db.Lookups.Find(id);
            if (lookup == null)
            {
                return HttpNotFound();
            }
            return View(lookup);
        }

        // GET: /Lookup/Create
        public ActionResult Create()
        {
            // Creates the drop down list for ConfigName in the view
            var ddlConfigNames = db.ConfiguratorNames.Select(x => x.ConfigName).Distinct();
            List<SelectListItem> configNames = new List<SelectListItem>();
            foreach (var item in ddlConfigNames)
            {
                configNames.Add(new SelectListItem { Text = item, Value = item });
            }
            ViewBag.ConfigNamesTo = configNames;

            // Creates the drop down list for ConfigData in the view
            var ddlConfigData = db.Structures.Select(x => x.ConfigData).Distinct();
            List<SelectListItem> configData = new List<SelectListItem>();
            foreach (var item in ddlConfigData)
            {
                configData.Add(new SelectListItem { Text = item, Value = item });
            }
            ViewBag.ConfigDataTo = configData;
            Lookup lookup = new Lookup();
            return View(lookup);
        }

        // POST: /Lookup/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] Lookup lookup, string ConfigName, string ConfigData)
        {
            //duplicate checking
            var Configs = db.Lookups.FirstOrDefault(x => x.ConfigName == lookup.ConfigName && x.ConfigData == lookup.ConfigData && x.Sequence == lookup.Sequence
            && x.Data == lookup.Data);

            if (Configs != null) ModelState.AddModelError("", "Duplicate Lookup Created---Please Recheck Data");

            //error checking for the model
            var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();

            if (ModelState.IsValid)
            {
                db.Lookups.Add(lookup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            // Creates the drop down list for ConfigName in the view
            var ddlConfigNames = db.ConfiguratorNames.Select(x => x.ConfigName).Distinct();
            List<SelectListItem> configNames = new List<SelectListItem>();
            foreach (var item in ddlConfigNames)
            {
                configNames.Add(new SelectListItem { Text = item, Value = item });
            }
            ViewBag.ConfigNamesTo = configNames;

            // Creates the drop down list for ConfigData in the view
            var ddlConfigData = db.Structures.Select(x => x.ConfigData).Distinct();
            List<SelectListItem> configData = new List<SelectListItem>();
            foreach (var item in ddlConfigData)
            {
                configData.Add(new SelectListItem { Text = item, Value = item });
            }
            ViewBag.ConfigDataTo = configData;
            return View(lookup);
        }

        // GET: /Lookup/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lookup lookup = db.Lookups.Find(id);
            if (lookup == null)
            {
                return HttpNotFound();
            }
            return View(lookup);
        }

        // POST: /Lookup/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Lookup lookup)
        {
            var Configs = db.Lookups.FirstOrDefault(x => x.ConfigName == lookup.ConfigName && x.ConfigData == lookup.ConfigData && x.Sequence == lookup.Sequence
                && x.Data == lookup.Data && x.Id != lookup.Id);

            if (Configs != null) ModelState.AddModelError("", "Duplicate Lookup Created---Please Recheck Data");

            if (ModelState.IsValid)
            {
                db.Entry(lookup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(lookup);
        }

        // GET: /Lookup/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lookup lookup = db.Lookups.Find(id);
            if (lookup == null)
            {
                return HttpNotFound();
            }
            return View(lookup);
        }

        // POST: /Lookup/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Lookup lookup = db.Lookups.Find(id);
            db.Lookups.Remove(lookup);
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
