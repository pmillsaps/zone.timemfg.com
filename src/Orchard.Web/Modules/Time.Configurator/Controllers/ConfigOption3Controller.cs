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
    public class ConfigOption3Controller : Controller
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ConfigOption3Controller(IOrchardServices services)
        {
            Services = services;
            db = new ConfiguratorEntities();
        }

        public ConfigOption3Controller(IOrchardServices services, ConfiguratorEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: /ConfigOption3/
        public ActionResult Index()
        {
            return View(db.ConfigOption3.OrderBy(x => x.ConfigName).ThenBy(x => x.ConfigData).ToList());
        }

        // GET: /ConfigOption3/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigOption3 configoption3 = db.ConfigOption3.Find(id);
            if (configoption3 == null)
            {
                return HttpNotFound();
            }
            return View(configoption3);
        }

        // GET: /ConfigOption3/Create
        public ActionResult Create()
        {
            GenerateDropDowns();
            return View();
        }

        // POST: /ConfigOption3/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude="Id")] ConfigOption3 configoption3)
        {
            //prevents a duplicate from being created
            var Configs = db.ConfigOption3.FirstOrDefault(x => x.ConfigName == configoption3.ConfigName && x.ConfigData == configoption3.ConfigData && x.Key1 == configoption3.Key1
            && x.Key2 == configoption3.Key2 && x.Key3 == configoption3.Key3 && x.ConfigOption == configoption3.ConfigOption);

            //displays if previous code found a duplicate
            if (Configs != null) ModelState.AddModelError("", "Duplicate Option Created---Please Recheck Data");

            if (ModelState.IsValid)
            {
                db.ConfigOption3.Add(configoption3);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(configoption3);
            return View(configoption3);
        }

        // GET: /ConfigOption3/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigOption3 configoption3 = db.ConfigOption3.Find(id);
            if (configoption3 == null)
            {
                return HttpNotFound();
            }
            GenerateDropDowns();
            return View(configoption3);
        }

        // POST: /ConfigOption3/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ConfigOption3 configoption3)
        {
            //prevents a duplicate from being saved when editing
            var Configs = db.ConfigOption3.FirstOrDefault(x => x.ConfigName == configoption3.ConfigName && x.ConfigData == configoption3.ConfigData && x.Key1 == configoption3.Key1
            && x.Key2 == configoption3.Key2 && x.Key3 == configoption3.Key3 && x.ConfigOption == configoption3.ConfigOption && x.Id != configoption3.Id);

            //displays if previous code found a duplicate
            if (Configs != null) ModelState.AddModelError("", "Duplicate Option Created---Please Recheck Data");

            if (ModelState.IsValid)
            {
                db.Entry(configoption3).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(configoption3);
            return View(configoption3);
        }

        // GET: /ConfigOption3/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigOption3 configoption3 = db.ConfigOption3.Find(id);
            if (configoption3 == null)
            {
                return HttpNotFound();
            }
            return View(configoption3);
        }

        // POST: /ConfigOption3/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ConfigOption3 configoption3 = db.ConfigOption3.Find(id);
            db.ConfigOption3.Remove(configoption3);
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
            var ConfigNameList = from firstList in db.ConfigOption3
                                 group firstList by firstList.ConfigName into newList1
                                 let x = newList1.FirstOrDefault()
                                 select x;

            var ConfigDataList = from secondList in db.ConfigOption3
                                 group secondList by secondList.ConfigData into newList2
                                 let x = newList2.FirstOrDefault()
                                 select x;

            var Key1List = from thirdList in db.ConfigOption3
                           group thirdList by thirdList.Key1 into newList3
                           let x = newList3.FirstOrDefault()
                           select x;

            var Key2List = from fourthList in db.ConfigOption3
                           group fourthList by fourthList.Key2 into newList4
                           let x = newList4.FirstOrDefault()
                           select x;

            var Key3List = from fifthList in db.ConfigOption3
                           group fifthList by fifthList.Key3 into newList5
                           let x = newList5.FirstOrDefault()
                           select x;

            var ConfigOptionList = from thirteenthList in db.ConfigOption3
                                   group thirteenthList by thirteenthList.ConfigOption into newList13
                                   let x = newList13.FirstOrDefault()
                                   select x;

            ViewBag.ConfigName = new SelectList(ConfigNameList.ToList(), "ConfigName", "ConfigName");
            ViewBag.ConfigData = new SelectList(ConfigDataList.ToList(), "ConfigData", "ConfigData");
            ViewBag.Key1 = new SelectList(Key1List.ToList(), "Key1", "Key1");
            ViewBag.Key2 = new SelectList(Key2List.ToList(), "Key2", "Key2");
            ViewBag.Key3 = new SelectList(Key3List.ToList(), "Key3", "Key3");
            ViewBag.ConfigOption = new SelectList(ConfigOptionList.ToList(), "ConfigOption", "ConfigOption");
        }

        //This and above ViewBags pull in the data to put into the drop down lists
        private void GenerateDropDowns(ConfigOption3 configoption3)
        {
            ViewBag.ConfigName = new SelectList(db.ConfigOption3.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName", configoption3.ConfigName);
            ViewBag.ConfigData = new SelectList(db.ConfigOption3.OrderBy(x => x.ConfigData), "ConfigData", "ConfigData", configoption3.ConfigData);
            ViewBag.Key1 = new SelectList(db.ConfigOption3.OrderBy(x => x.Key1), "Key1", "Key1", configoption3.Key1);
            ViewBag.Key2 = new SelectList(db.ConfigOption3.OrderBy(x => x.Key2), "Key2", "Key2", configoption3.Key2);
            ViewBag.Key3 = new SelectList(db.ConfigOption3.OrderBy(x => x.Key3), "Key3", "Key3", configoption3.Key3);
            ViewBag.ConfigOption = new SelectList(db.ConfigOption3.OrderBy(x => x.ConfigOption), "ConfigOption", "ConfigOption", configoption3.ConfigOption);
        }
    }
}
