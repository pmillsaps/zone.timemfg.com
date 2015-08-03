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
    public class ConfigOption7Controller : Controller
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ConfigOption7Controller(IOrchardServices services)
        {
            Services = services;
            db = new ConfiguratorEntities();
        }

        public ConfigOption7Controller(IOrchardServices services, ConfiguratorEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: /ConfigOption7/
        public ActionResult Index()
        {
            return View(db.ConfigOption7.OrderBy(x => x.ConfigName).ThenBy(x => x.ConfigData).ToList());
        }

        // GET: /ConfigOption7/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigOption7 configoption7 = db.ConfigOption7.Find(id);
            if (configoption7 == null)
            {
                return HttpNotFound();
            }
            return View(configoption7);
        }

        // GET: /ConfigOption7/Create
        public ActionResult Create()
        {
            GenerateDropDowns();
            return View();
        }

        // POST: /ConfigOption7/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude="Id")] ConfigOption7 configoption7)
        {
            var Configs = db.ConfigOption7.FirstOrDefault(x => x.ConfigName == configoption7.ConfigName && x.ConfigData == configoption7.ConfigData && x.Key1 == configoption7.Key1
            && x.Key2 == configoption7.Key2 && x.Key3 == configoption7.Key3 && x.Key4 == configoption7.Key4 && x.Key5 == configoption7.Key5 && x.Key6 == configoption7.Key6
            && x.Key7 == configoption7.Key7 && x.ConfigOption == configoption7.ConfigOption);

            if (Configs != null) ModelState.AddModelError("", "Duplicate Option Created---Please Recheck Data");

            if (ModelState.IsValid)
            {
                db.ConfigOption7.Add(configoption7);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(configoption7);
            return View(configoption7);
        }

        // GET: /ConfigOption7/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigOption7 configoption7 = db.ConfigOption7.Find(id);
            if (configoption7 == null)
            {
                return HttpNotFound();
            }
            GenerateDropDowns();
            return View(configoption7);
        }

        // POST: /ConfigOption7/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ConfigOption7 configoption7)
        {
            var Configs = db.ConfigOption7.FirstOrDefault(x => x.ConfigName == configoption7.ConfigName && x.ConfigData == configoption7.ConfigData && x.Key1 == configoption7.Key1
            && x.Key2 == configoption7.Key2 && x.Key3 == configoption7.Key3 && x.Key4 == configoption7.Key4 && x.Key5 == configoption7.Key5 && x.Key6 == configoption7.Key6
            && x.Key7 == configoption7.Key7 && x.ConfigOption == configoption7.ConfigOption && x.Id != configoption7.Id);

            if (Configs != null) ModelState.AddModelError("", "Duplicate Option Created---Please Recheck Data");

            if (ModelState.IsValid)
            {
                db.Entry(configoption7).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(configoption7);
            return View(configoption7);
        }

        // GET: /ConfigOption7/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigOption7 configoption7 = db.ConfigOption7.Find(id);
            if (configoption7 == null)
            {
                return HttpNotFound();
            }
            return View(configoption7);
        }

        // POST: /ConfigOption7/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ConfigOption7 configoption7 = db.ConfigOption7.Find(id);
            db.ConfigOption7.Remove(configoption7);
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
            var ConfigNameList = from firstList in db.ConfigOption7
                                 group firstList by firstList.ConfigName into newList1
                                 let x = newList1.FirstOrDefault()
                                 select x;

            var ConfigDataList = from secondList in db.ConfigOption7
                                 group secondList by secondList.ConfigData into newList2
                                 let x = newList2.FirstOrDefault()
                                 select x;

            var Key1List = from thirdList in db.ConfigOption7
                           group thirdList by thirdList.Key1 into newList3
                           let x = newList3.FirstOrDefault()
                           select x;

            var Key2List = from fourthList in db.ConfigOption7
                           group fourthList by fourthList.Key2 into newList4
                           let x = newList4.FirstOrDefault()
                           select x;

            var Key3List = from fifthList in db.ConfigOption7
                           group fifthList by fifthList.Key3 into newList5
                           let x = newList5.FirstOrDefault()
                           select x;

            var Key4List = from sixthList in db.ConfigOption7
                           group sixthList by sixthList.Key4 into newList6
                           let x = newList6.FirstOrDefault()
                           select x;

            var Key5List = from seventhList in db.ConfigOption7
                           group seventhList by seventhList.Key5 into newList7
                           let x = newList7.FirstOrDefault()
                           select x;

            var Key6List = from eigthList in db.ConfigOption7
                           group eigthList by eigthList.Key6 into newList8
                           let x = newList8.FirstOrDefault()
                           select x;

            var Key7List = from ninthList in db.ConfigOption7
                           group ninthList by ninthList.Key7 into newList9
                           let x = newList9.FirstOrDefault()
                           select x;

            var ConfigOptionList = from thirteenthList in db.ConfigOption7
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
            ViewBag.Key6 = new SelectList(Key6List.ToList(), "Key6", "Key6");
            ViewBag.Key7 = new SelectList(Key7List.ToList(), "Key7", "Key7");
            ViewBag.ConfigOption = new SelectList(ConfigOptionList.ToList(), "ConfigOption", "ConfigOption");
        }

        private void GenerateDropDowns(ConfigOption7 configoption7)
        {
            ViewBag.ConfigName = new SelectList(db.ConfigOption7.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName", configoption7.ConfigName);
            ViewBag.ConfigData = new SelectList(db.ConfigOption7.OrderBy(x => x.ConfigData), "ConfigData", "ConfigData", configoption7.ConfigData);
            ViewBag.Key1 = new SelectList(db.ConfigOption7.OrderBy(x => x.Key1), "Key1", "Key1", configoption7.Key1);
            ViewBag.Key2 = new SelectList(db.ConfigOption7.OrderBy(x => x.Key2), "Key2", "Key2", configoption7.Key2);
            ViewBag.Key3 = new SelectList(db.ConfigOption7.OrderBy(x => x.Key3), "Key3", "Key3", configoption7.Key3);
            ViewBag.Key4 = new SelectList(db.ConfigOption7.OrderBy(x => x.Key4), "Key4", "Key4", configoption7.Key4);
            ViewBag.Key5 = new SelectList(db.ConfigOption7.OrderBy(x => x.Key5), "Key5", "Key5", configoption7.Key5);
            ViewBag.Key6 = new SelectList(db.ConfigOption7.OrderBy(x => x.Key6), "Key6", "Key6", configoption7.Key6);
            ViewBag.Key7 = new SelectList(db.ConfigOption7.OrderBy(x => x.Key7), "Key7", "Key7", configoption7.Key7);
            ViewBag.ConfigOption = new SelectList(db.ConfigOption7.OrderBy(x => x.ConfigOption), "ConfigOption", "ConfigOption", configoption7.ConfigOption);
        }
    }
}
