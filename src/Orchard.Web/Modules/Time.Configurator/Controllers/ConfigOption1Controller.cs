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
    public class ConfigOption1Controller : Controller
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ConfigOption1Controller(IOrchardServices services)
        {
            Services = services;
            db = new ConfiguratorEntities();
        }

        public ConfigOption1Controller(IOrchardServices services, ConfiguratorEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: /ConfigOption1/
        public ActionResult Index()
        {
            return View(db.ConfigOption1.OrderBy(x => x.ConfigName).ThenBy(x => x.ConfigData).ToList());
        }

        // GET: /ConfigOption1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigOption1 configoption1 = db.ConfigOption1.Find(id);
            if (configoption1 == null)
            {
                return HttpNotFound();
            }
            return View(configoption1);
        }

        // GET: /ConfigOption1/Create
        public ActionResult Create()
        {
            GenerateDropDowns();
            return View();
        }

        // POST: /ConfigOption1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude="Id")] ConfigOption1 configoption1)
        {
            var Configs = db.ConfigOption1.FirstOrDefault(x => x.ConfigName == configoption1.ConfigName && x.ConfigData == configoption1.ConfigData && x.Key1 == configoption1.Key1
            && x.ConfigOption == configoption1.ConfigOption && x.Id != configoption1.Id);

            if (Configs != null) ModelState.AddModelError("", "Duplicate Option Created---Please Recheck Inputed Data");

            if (ModelState.IsValid)
            {
                db.ConfigOption1.Add(configoption1);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(configoption1);
            return View(configoption1);
        }

        // GET: /ConfigOption1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigOption1 configoption1 = db.ConfigOption1.Find(id);
            if (configoption1 == null)
            {
                return HttpNotFound();
            }
            GenerateDropDowns();
            return View(configoption1);
        }

        // POST: /ConfigOption1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ConfigOption1 configoption1)
        {

            var Configs = db.ConfigOption1.FirstOrDefault(x => x.ConfigName == configoption1.ConfigName && x.ConfigData == configoption1.ConfigData && x.Key1 == configoption1.Key1
            && x.ConfigOption == configoption1.ConfigOption && x.Id != configoption1.Id);

            if (Configs != null) ModelState.AddModelError("", "Duplicate Option Created---Please Recheck Inputed Data");

            if (ModelState.IsValid)
            {
                db.Entry(configoption1).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(configoption1);
            return View(configoption1);
        }

        // GET: /ConfigOption1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigOption1 configoption1 = db.ConfigOption1.Find(id);
            if (configoption1 == null)
            {
                return HttpNotFound();
            }
            return View(configoption1);
        }

        // POST: /ConfigOption1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ConfigOption1 configoption1 = db.ConfigOption1.Find(id);
            db.ConfigOption1.Remove(configoption1);
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
            var ConfigNameList = from firstList in db.ConfigOption1
                                 group firstList by firstList.ConfigName into newList1
                                 let x = newList1.FirstOrDefault()
                                 select x;

            var ConfigDataList = from secondList in db.ConfigOption1
                                 group secondList by secondList.ConfigData into newList2
                                 let x = newList2.FirstOrDefault()
                                 select x;

            var Key1List = from thirdList in db.ConfigOption1
                           group thirdList by thirdList.Key1 into newList3
                           let x = newList3.FirstOrDefault()
                           select x;

            var ConfigOptionList = from thirteenthList in db.ConfigOption1
                                   group thirteenthList by thirteenthList.ConfigOption into newList13
                                   let x = newList13.FirstOrDefault()
                                   select x;

            ViewBag.ConfigName = new SelectList(ConfigNameList.ToList(), "ConfigName", "ConfigName");
            ViewBag.ConfigData = new SelectList(ConfigDataList.ToList(), "ConfigData", "ConfigData");
            ViewBag.Key1 = new SelectList(Key1List.ToList(), "Key1", "Key1");
            ViewBag.ConfigOption = new SelectList(ConfigOptionList.ToList(), "ConfigOption", "ConfigOption");
        }

        private void GenerateDropDowns(ConfigOption1 configoptions1)
        {
            ViewBag.ConfigName = new SelectList(db.ConfigOption1.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName", configoptions1.ConfigName);
            ViewBag.ConfigData = new SelectList(db.ConfigOption1.OrderBy(x => x.ConfigData), "ConfigData", "ConfigData", configoptions1.ConfigData);
            ViewBag.Key1 = new SelectList(db.ConfigOption1.OrderBy(x => x.Key1), "Key1", "Key1", configoptions1.Key1);
            ViewBag.ConfigOption = new SelectList(db.ConfigOption1.OrderBy(x => x.ConfigOption), "ConfigOption", "ConfigOption", configoptions1.ConfigOption);
        }
    }
}
