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
    public class ConfigOption9Controller : Controller
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ConfigOption9Controller(IOrchardServices services)
        {
            Services = services;
            db = new ConfiguratorEntities();
        }

        public ConfigOption9Controller(IOrchardServices services, ConfiguratorEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: /ConfigOption9/
        public ActionResult Index()
        {
            return View(db.ConfigOption9.OrderBy(x => x.ConfigName).ThenBy(x => x.ConfigData).ToList());
        }

        // GET: /ConfigOption9/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigOption9 configoption9 = db.ConfigOption9.Find(id);
            if (configoption9 == null)
            {
                return HttpNotFound();
            }
            return View(configoption9);
        }

        // GET: /ConfigOption9/Create
        public ActionResult Create()
        {
            GenerateDropDowns();
            return View();
        }

        // POST: /ConfigOption9/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude="Id")] ConfigOption9 configoption9)
        {
            //prevents a duplicate from being created
            var Configs = db.ConfigOption9.FirstOrDefault(x => x.ConfigName == configoption9.ConfigName && x.ConfigData == configoption9.ConfigData && x.Key1 == configoption9.Key1
            && x.Key2 == configoption9.Key2 && x.Key3 == configoption9.Key3 && x.Key4 == configoption9.Key4 && x.Key5 == configoption9.Key5 && x.Key6 == configoption9.Key6
            && x.Key7 == configoption9.Key7 && x.Key8 == configoption9.Key8 && x.Key9 == configoption9.Key9 && x.ConfigOption == configoption9.ConfigOption);

            //displays if previous code found a duplicate
            if (Configs != null) ModelState.AddModelError("", "Duplicate Option Created---Please Recheck Data");

            if (ModelState.IsValid)
            {
                db.ConfigOption9.Add(configoption9);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(configoption9);
            return View(configoption9);
        }

        // GET: /ConfigOption9/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigOption9 configoption9 = db.ConfigOption9.Find(id);
            if (configoption9 == null)
            {
                return HttpNotFound();
            }
            GenerateDropDowns();
            return View(configoption9);
        }

        // POST: /ConfigOption9/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ConfigOption9 configoption9)
        {
            //prevents a duplicate from being saved when editing
            var Configs = db.ConfigOption9.FirstOrDefault(x => x.ConfigName == configoption9.ConfigName && x.ConfigData == configoption9.ConfigData && x.Key1 == configoption9.Key1
            && x.Key2 == configoption9.Key2 && x.Key3 == configoption9.Key3 && x.Key4 == configoption9.Key4 && x.Key5 == configoption9.Key5 && x.Key6 == configoption9.Key6
            && x.Key7 == configoption9.Key7 && x.Key8 == configoption9.Key8 && x.Key9 == configoption9.Key9 && x.ConfigOption == configoption9.ConfigOption);

            //displays if previous code found a duplicate
            if (Configs != null) ModelState.AddModelError("", "Duplicate Option Created---Please Recheck Data");

            if (ModelState.IsValid)
            {
                db.Entry(configoption9).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(configoption9);
            return View(configoption9);
        }

        // GET: /ConfigOption9/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigOption9 configoption9 = db.ConfigOption9.Find(id);
            if (configoption9 == null)
            {
                return HttpNotFound();
            }
            return View(configoption9);
        }

        // POST: /ConfigOption9/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ConfigOption9 configoption9 = db.ConfigOption9.Find(id);
            db.ConfigOption9.Remove(configoption9);
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
            var ConfigNameList = from firstList in db.ConfigOption9
                                 group firstList by firstList.ConfigName into newList1
                                 let x = newList1.FirstOrDefault()
                                 select x;

            var ConfigDataList = from secondList in db.ConfigOption9
                                 group secondList by secondList.ConfigData into newList2
                                 let x = newList2.FirstOrDefault()
                                 select x;

            var Key1List = from thirdList in db.ConfigOption9
                           group thirdList by thirdList.Key1 into newList3
                           let x = newList3.FirstOrDefault()
                           select x;

            var Key2List = from fourthList in db.ConfigOption9
                           group fourthList by fourthList.Key2 into newList4
                           let x = newList4.FirstOrDefault()
                           select x;

            var Key3List = from fifthList in db.ConfigOption9
                           group fifthList by fifthList.Key3 into newList5
                           let x = newList5.FirstOrDefault()
                           select x;

            var Key4List = from sixthList in db.ConfigOption9
                           group sixthList by sixthList.Key4 into newList6
                           let x = newList6.FirstOrDefault()
                           select x;

            var Key5List = from seventhList in db.ConfigOption9
                           group seventhList by seventhList.Key5 into newList7
                           let x = newList7.FirstOrDefault()
                           select x;

            var Key6List = from eigthList in db.ConfigOption9
                           group eigthList by eigthList.Key6 into newList8
                           let x = newList8.FirstOrDefault()
                           select x;

            var Key7List = from ninthList in db.ConfigOption9
                           group ninthList by ninthList.Key7 into newList9
                           let x = newList9.FirstOrDefault()
                           select x;

            var Key8List = from tenthList in db.ConfigOption9
                           group tenthList by tenthList.Key8 into newList10
                           let x = newList10.FirstOrDefault()
                           select x;

            var Key9List = from eleventhList in db.ConfigOption9
                           group eleventhList by eleventhList.Key9 into newList11
                           let x = newList11.FirstOrDefault()
                           select x;

            var ConfigOptionList = from thirteenthList in db.ConfigOption9
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
            ViewBag.Key8 = new SelectList(Key8List.ToList(), "Key8", "Key8");
            ViewBag.Key9 = new SelectList(Key9List.ToList(), "Key9", "Key9");
            ViewBag.ConfigOption = new SelectList(ConfigOptionList.ToList(), "ConfigOption", "ConfigOption");
        }

        //This and above ViewBags pull in the data to put into the drop down lists
        private void GenerateDropDowns(ConfigOption9 configOption9)
        {
            ViewBag.ConfigName = new SelectList(db.ConfigOption9.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName", configOption9.ConfigName);
            ViewBag.ConfigData = new SelectList(db.ConfigOption9.OrderBy(x => x.ConfigData), "ConfigData", "ConfigData", configOption9.ConfigData);
            ViewBag.Key1 = new SelectList(db.ConfigOption9.OrderBy(x => x.Key1), "Key1", "Key1", configOption9.Key1);
            ViewBag.Key2 = new SelectList(db.ConfigOption9.OrderBy(x => x.Key2), "Key2", "Key2", configOption9.Key2);
            ViewBag.Key3 = new SelectList(db.ConfigOption9.OrderBy(x => x.Key3), "Key3", "Key3", configOption9.Key3);
            ViewBag.Key4 = new SelectList(db.ConfigOption9.OrderBy(x => x.Key4), "Key4", "Key4", configOption9.Key4);
            ViewBag.Key5 = new SelectList(db.ConfigOption9.OrderBy(x => x.Key5), "Key5", "Key5", configOption9.Key5);
            ViewBag.Key6 = new SelectList(db.ConfigOption9.OrderBy(x => x.Key6), "Key6", "Key6", configOption9.Key6);
            ViewBag.Key7 = new SelectList(db.ConfigOption9.OrderBy(x => x.Key7), "Key7", "Key7", configOption9.Key7);
            ViewBag.Key8 = new SelectList(db.ConfigOption9.OrderBy(x => x.Key8), "Key8", "Key8", configOption9.Key8);
            ViewBag.Key9 = new SelectList(db.ConfigOption9.OrderBy(x => x.Key9), "Key9", "Key9", configOption9.Key9);
            ViewBag.ConfigOption = new SelectList(db.ConfigOption9.OrderBy(x => x.ConfigOption), "ConfigOption", "ConfigOption", configOption9.ConfigOption);
        }
    }
}
