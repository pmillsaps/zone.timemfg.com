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
            var lookups = db.Lookups.AsQueryable();
            if (!String.IsNullOrEmpty(ConfigNames)) lookups = lookups.Where(x => x.ConfigName == ConfigNames);
            if (!String.IsNullOrEmpty(ConfigData)) lookups = lookups.Where(x => x.ConfigData == ConfigData);

            ViewBag.ConfigNames = new SelectList(db.ConfiguratorNames.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName");
            ViewBag.ConfigData = new SelectList(db.Lookups.Select(x => x.ConfigData).Distinct());

            return View(lookups.OrderBy(x => x.ConfigName).ThenBy(x => x.ConfigData).ToList());
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
            GenerateDropDowns();
            return View();
        }

        // POST: /Lookup/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] Lookup lookup)
        {
            //prevents a duplicate from being created
            //duplicate checking
            var Configs = db.Lookups.FirstOrDefault(x => x.ConfigName == lookup.ConfigName && x.ConfigData == lookup.ConfigData && x.Sequence == lookup.Sequence
            && x.Data == lookup.Data);

            //displays if previous code found a duplicate
            if (Configs != null) ModelState.AddModelError("", "Duplicate Lookup Created---Please Recheck Data");

            //error checking for the model
            var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();

            if (ModelState.IsValid)
            {
                db.Lookups.Add(lookup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(lookup);
            return View();
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
            GenerateDropDowns();
            return View(lookup);
        }

        // POST: /Lookup/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Lookup lookup)
        {
            //prevents a duplicate from being saved when editing
            var Configs = db.Lookups.FirstOrDefault(x => x.ConfigName == lookup.ConfigName && x.ConfigData == lookup.ConfigData && x.Sequence == lookup.Sequence
                && x.Data == lookup.Data && x.Id != lookup.Id);

            //displays if previous code found a duplicate
            if (Configs != null) ModelState.AddModelError("", "Duplicate Lookup Created---Please Recheck Data");

            if (ModelState.IsValid)
            {
                db.Entry(lookup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(lookup);
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

        private void GenerateDropDowns()
        {
            //prevent duplicates from showing up in drop down
            //without var list codes, every CFG and Global shows up in drop down and whatever else for the other drop downs
            var SequenceList = from thirdList in db.Lookups
                               group thirdList by thirdList.Sequence into newList3
                               let x = newList3.FirstOrDefault()
                               select x;
            var DataList = from fourthList in db.Lookups
                             group fourthList by fourthList.Data into newList4
                             let x = newList4.FirstOrDefault()
                             select x;

            ViewBag.ConfigName = new SelectList(db.ConfiguratorNames.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName");
            //ViewBag.ConfigData = new SelectList(db.Structures.OrderBy(x => x.ConfigData), "ConfigData", "ConfigData");                                          //shows all values
            ViewBag.ConfigData = new SelectList(db.Structures.Select(x => x.ConfigData).Distinct());                                                              //shows distinct values
            ViewBag.Sequence = new SelectList(db.Lookups.OrderBy(x => x.Sequence), "Sequence", "Sequence");
            ViewBag.Lookup = new SelectList(db.Lookups.OrderBy(x => x.Data), "Data", "Data");
        }

        //This and above ViewBags pull in the data to put into the drop down lists
        private void GenerateDropDowns(Lookup lookup)
        {
            ViewBag.ConfigName = new SelectList(db.ConfiguratorNames.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName", lookup.ConfigName);
            //ViewBag.ConfigData = new SelectList(db.Structures.OrderBy(x => x.ConfigData), "ConfigData", "ConfigData", lookup.ConfigData);                 //shows all values
            ViewBag.ConfigData = new SelectList(db.Structures.Select(x => x.ConfigData).Distinct(), lookup.ConfigData);                                     //shows distinct values
            ViewBag.Sequence = new SelectList(db.Lookups.OrderBy(x => x.Sequence), "Sequence", "Sequence", lookup.Sequence);
            ViewBag.Lookup = new SelectList(db.Lookups.OrderBy(x => x.Data), "Lookup", "Lookup", lookup.Data);
        }
    }
}
