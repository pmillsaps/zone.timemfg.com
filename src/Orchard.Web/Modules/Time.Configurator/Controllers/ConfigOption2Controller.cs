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
    public class ConfigOption2Controller : Controller
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ConfigOption2Controller(IOrchardServices services)
        {
            Services = services;
            db = new ConfiguratorEntities();
        }

        public ConfigOption2Controller(IOrchardServices services, ConfiguratorEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: /ConfigOption2/
        public ActionResult Index()
        {
            return View(db.ConfigOption2.OrderBy(x => x.ConfigName).ThenBy(x => x.ConfigData).ToList());
        }

        // GET: /ConfigOption2/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigOption2 configoption2 = db.ConfigOption2.Find(id);
            if (configoption2 == null)
            {
                return HttpNotFound();
            }
            return View(configoption2);
        }

        // GET: /ConfigOption2/Create
        public ActionResult Create()
        {
            GenerateDropDowns();
            return View();
        }

        // POST: /ConfigOption2/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude="Id")] ConfigOption2 configoption2)
        {
            var Configs = db.ConfigOption2.FirstOrDefault(x => x.ConfigName == configoption2.ConfigName && x.ConfigData == configoption2.ConfigData && x.Key1 == configoption2.Key1
            && x.Key2 == configoption2.Key2);

            if (Configs != null) ModelState.AddModelError("", "Duplicate Option Created---Please Recheck Data");

            if (ModelState.IsValid)
            {
                db.ConfigOption2.Add(configoption2);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(configoption2);
            return View(configoption2);
        }

        // GET: /ConfigOption2/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigOption2 configoption2 = db.ConfigOption2.Find(id);
            if (configoption2 == null)
            {
                return HttpNotFound();
            }
            GenerateDropDowns();
            return View(configoption2);
        }

        // POST: /ConfigOption2/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ConfigOption2 configoption2)
        {
            var Configs = db.ConfigOption2.FirstOrDefault(x => x.ConfigName == configoption2.ConfigName && x.ConfigData == configoption2.ConfigData && x.Key1 == configoption2.Key1
                && x.Key2 == configoption2.Key2 && x.Id != configoption2.Id);

            if (Configs != null) ModelState.AddModelError("", "Duplicate Option Created---Please Recheck Data");

            if (ModelState.IsValid)
            {
                db.Entry(configoption2).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(configoption2);
            return View(configoption2);
        }

        // GET: /ConfigOption2/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigOption2 configoption2 = db.ConfigOption2.Find(id);
            if (configoption2 == null)
            {
                return HttpNotFound();
            }
            return View(configoption2);
        }

        // POST: /ConfigOption2/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ConfigOption2 configoption2 = db.ConfigOption2.Find(id);
            db.ConfigOption2.Remove(configoption2);
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
            var ConfigNameList = from firstList in db.ConfigOption2
                                 group firstList by firstList.ConfigName into newList1
                                 let x = newList1.FirstOrDefault()
                                 select x;

            var ConfigDataList = from secondList in db.ConfigOption2
                                 group secondList by secondList.ConfigData into newList2
                                 let x = newList2.FirstOrDefault()
                                 select x;

            var Key1List = from thirdList in db.ConfigOption2
                           group thirdList by thirdList.Key1 into newList3
                           let x = newList3.FirstOrDefault()
                           select x;

            var Key2List = from fourthList in db.ConfigOption2
                           group fourthList by fourthList.Key2 into newList4
                           let x = newList4.FirstOrDefault()
                           select x;

            var ConfigOptionList = from thirteenthList in db.ConfigOption10
                                   group thirteenthList by thirteenthList.ConfigOption into newList13
                                   let x = newList13.FirstOrDefault()
                                   select x;

            ViewBag.ConfigName = new SelectList(ConfigNameList.ToList(), "ConfigName", "ConfigName");
            ViewBag.ConfigData = new SelectList(ConfigDataList.ToList(), "ConfigData", "ConfigData");
            ViewBag.Key1 = new SelectList(Key1List.ToList(), "Key1", "Key1");
            ViewBag.Key2 = new SelectList(Key2List.ToList(), "Key2", "Key2");
            ViewBag.ConfigOption = new SelectList(ConfigOptionList.ToList(), "ConfigOption", "ConfigOption");
        }

        private void GenerateDropDowns(ConfigOption2 configoptions2)
        {
            ViewBag.ConfigName = new SelectList(db.ConfigOption2.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName", configoptions2.ConfigName);
            ViewBag.ConfigData = new SelectList(db.ConfigOption2.OrderBy(x => x.ConfigData), "ConfigData", "ConfigData", configoptions2.ConfigData);
            ViewBag.Key1 = new SelectList(db.ConfigOption2.OrderBy(x => x.Key1), "Key1", "Key1", configoptions2.Key1);
            ViewBag.Key2 = new SelectList(db.ConfigOption2.OrderBy(x => x.Key2), "Key2", "Key2", configoptions2.Key2);
            ViewBag.ConfigOption = new SelectList(db.ConfigOption2.OrderBy(x => x.ConfigOption), "ConfigOption", "ConfigOption", configoptions2.ConfigOption);
        }
    }
}
