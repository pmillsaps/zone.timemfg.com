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
    public class ConfigOption5Controller : Controller
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ConfigOption5Controller(IOrchardServices services)
        {
            Services = services;
            db = new ConfiguratorEntities();
        }

        public ConfigOption5Controller(IOrchardServices services, ConfiguratorEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: /ConfigOption5/
        public ActionResult Index()
        {
            return View(db.ConfigOption5.OrderBy(x => x.ConfigName).ThenBy(x => x.ConfigData).ToList());
        }

        // GET: /ConfigOption5/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigOption5 configoption5 = db.ConfigOption5.Find(id);
            if (configoption5 == null)
            {
                return HttpNotFound();
            }
            return View(configoption5);
        }

        // GET: /ConfigOption5/Create
        public ActionResult Create()
        {
            GenerateDropDowns();
            return View();
        }

        // POST: /ConfigOption5/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude="Id")] ConfigOption5 configoption5)
        {
            //prevents a duplicate from being created
            var Configs = db.ConfigOption5.FirstOrDefault(x => x.ConfigName == configoption5.ConfigName && x.ConfigData == configoption5.ConfigData && x.Key1 == configoption5.Key1
            && x.Key2 == configoption5.Key2 && x.Key3 == configoption5.Key3 && x.Key4 == configoption5.Key4 && x.Key5 == configoption5.Key5 
            && x.ConfigOption == configoption5.ConfigOption);

            //displays if previous code found a duplicate
            if (Configs != null) ModelState.AddModelError("", "Duplicate Option Created---Please Recheck Data");

            if (ModelState.IsValid)
            {
                db.ConfigOption5.Add(configoption5);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(configoption5);
            return View(configoption5);
        }

        // GET: /ConfigOption5/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigOption5 configoption5 = db.ConfigOption5.Find(id);
            if (configoption5 == null)
            {
                return HttpNotFound();
            }
            GenerateDropDowns();
            return View(configoption5);
        }

        // POST: /ConfigOption5/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ConfigOption5 configoption5)
        {
            //prevents a duplicate from being saved when editing
            var Configs = db.ConfigOption5.FirstOrDefault(x => x.ConfigName == configoption5.ConfigName && x.ConfigData == configoption5.ConfigData && x.Key1 == configoption5.Key1
            && x.Key2 == configoption5.Key2 && x.Key3 == configoption5.Key3 && x.Key4 == configoption5.Key4 && x.Key5 == configoption5.Key5
            && x.ConfigOption == configoption5.ConfigOption && x.Id != configoption5.Id);

            //displays if previous code found a duplicate
            if (Configs != null) ModelState.AddModelError("", "Duplicate Option Created---Please Recheck Data");

            if (ModelState.IsValid)
            {
                db.Entry(configoption5).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(configoption5);
            return View(configoption5);
        }

        // GET: /ConfigOption5/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigOption5 configoption5 = db.ConfigOption5.Find(id);
            if (configoption5 == null)
            {
                return HttpNotFound();
            }
            return View(configoption5);
        }

        // POST: /ConfigOption5/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ConfigOption5 configoption5 = db.ConfigOption5.Find(id);
            db.ConfigOption5.Remove(configoption5);
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
            var ConfigNameList = from firstList in db.ConfigOption5
                                 group firstList by firstList.ConfigName into newList1
                                 let x = newList1.FirstOrDefault()
                                 select x;

            var ConfigDataList = from secondList in db.ConfigOption5
                                 group secondList by secondList.ConfigData into newList2
                                 let x = newList2.FirstOrDefault()
                                 select x;

            var Key1List = from thirdList in db.ConfigOption5
                           group thirdList by thirdList.Key1 into newList3
                           let x = newList3.FirstOrDefault()
                           select x;

            var Key2List = from fourthList in db.ConfigOption5
                           group fourthList by fourthList.Key2 into newList4
                           let x = newList4.FirstOrDefault()
                           select x;

            var Key3List = from fifthList in db.ConfigOption5
                           group fifthList by fifthList.Key3 into newList5
                           let x = newList5.FirstOrDefault()
                           select x;

            var Key4List = from sixthList in db.ConfigOption5
                           group sixthList by sixthList.Key4 into newList6
                           let x = newList6.FirstOrDefault()
                           select x;

            var Key5List = from seventhList in db.ConfigOption5
                           group seventhList by seventhList.Key5 into newList7
                           let x = newList7.FirstOrDefault()
                           select x;

            var ConfigOptionList = from thirteenthList in db.ConfigOption5
                                   group thirteenthList by thirteenthList.ConfigOption into newList13
                                   let x = newList13.FirstOrDefault()
                                   select x;

            ViewBag.ConfigName = new SelectList(ConfigNameList.ToList(), "ConfigName", "ConfigName");
            ViewBag.ConfigData = new SelectList(ConfigDataList.ToList(), "ConfigData", "ConfigData");
            ViewBag.Key1 = new SelectList(Key1List.ToList(), "Key1", "Key1");
            ViewBag.Key2 = new SelectList(Key2List.ToList(), "Key2", "Key2");
            ViewBag.Key3 = new SelectList(Key3List.ToList(), "Key3", "Key3");
            ViewBag.Key4 = new SelectList(Key4List.ToList(), "Key4", "Key4");
            ViewBag.Key5 = new SelectList(Key5List.ToList(), "Key5", "Key5");
            ViewBag.ConfigOption = new SelectList(ConfigOptionList.ToList(), "ConfigOption", "ConfigOption");
        }

        //This and above ViewBags pull in the data to put into the drop down lists
        private void GenerateDropDowns(ConfigOption5 configoption5)
        {
            ViewBag.ConfigName = new SelectList(db.ConfigOption5.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName", configoption5.ConfigName);
            ViewBag.ConfigData = new SelectList(db.ConfigOption5.OrderBy(x => x.ConfigData), "ConfigData", "ConfigData", configoption5.ConfigData);
            ViewBag.Key1 = new SelectList(db.ConfigOption5.OrderBy(x => x.Key1), "Key1", "Key1", configoption5.Key1);
            ViewBag.Key2 = new SelectList(db.ConfigOption5.OrderBy(x => x.Key2), "Key2", "Key2", configoption5.Key2);
            ViewBag.Key3 = new SelectList(db.ConfigOption5.OrderBy(x => x.Key3), "Key3", "Key3", configoption5.Key3);
            ViewBag.Key4 = new SelectList(db.ConfigOption5.OrderBy(x => x.Key4), "Key4", "Key4", configoption5.Key4);
            ViewBag.Key5 = new SelectList(db.ConfigOption5.OrderBy(x => x.Key5), "Key5", "Key5", configoption5.Key5);
            ViewBag.ConfigOption = new SelectList(db.ConfigOption5.OrderBy(x => x.ConfigOption), "ConfigOption", "ConfigOption", configoption5.ConfigOption);
        }
    }
}
