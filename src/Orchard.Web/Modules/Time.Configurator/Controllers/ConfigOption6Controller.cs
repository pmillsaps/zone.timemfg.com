﻿using Orchard.Themes;
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
    public class ConfigOption6Controller : Controller
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        // GET: /ConfigOption6/
        public ActionResult Index()
        {
            return View(db.ConfigOption6.OrderBy(x => x.ConfigName).ThenBy(x => x.ConfigData).ToList());
        }

        // GET: /ConfigOption6/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigOption6 configoption6 = db.ConfigOption6.Find(id);
            if (configoption6 == null)
            {
                return HttpNotFound();
            }
            return View(configoption6);
        }

        // GET: /ConfigOption6/Create
        public ActionResult Create()
        {
            GenerateDropDowns();
            return View();
        }

        // POST: /ConfigOption6/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude="Id")] ConfigOption6 configoption6)
        {
            var Configs = db.ConfigOption6.FirstOrDefault(x => x.ConfigName == configoption6.ConfigName && x.ConfigData == configoption6.ConfigData && x.Key1 == configoption6.Key1
            && x.Key2 == configoption6.Key2 && x.Key3 == configoption6.Key3 && x.Key4 == configoption6.Key4 && x.Key5 == configoption6.Key5 && x.Key6 == configoption6.Key6
            && x.ConfigOption == configoption6.ConfigOption && x.Id != configoption6.Id);

            if (Configs != null) ModelState.AddModelError("", "Duplicate Option Created---Please Recheck Inputed Data");

            if (ModelState.IsValid)
            {
                db.ConfigOption6.Add(configoption6);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(configoption6);
            return View(configoption6);
        }

        // GET: /ConfigOption6/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigOption6 configoption6 = db.ConfigOption6.Find(id);
            if (configoption6 == null)
            {
                return HttpNotFound();
            }
            GenerateDropDowns();
            return View(configoption6);
        }

        // POST: /ConfigOption6/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ConfigOption6 configoption6)
        {
            var Configs = db.ConfigOption6.FirstOrDefault(x => x.ConfigName == configoption6.ConfigName && x.ConfigData == configoption6.ConfigData && x.Key1 == configoption6.Key1
            && x.Key2 == configoption6.Key2 && x.Key3 == configoption6.Key3 && x.Key4 == configoption6.Key4 && x.Key5 == configoption6.Key5 && x.Key6 == configoption6.Key6
            && x.ConfigOption == configoption6.ConfigOption && x.Id != configoption6.Id);

            if (Configs != null) ModelState.AddModelError("", "Duplicate Option Created---Please Recheck Inputed Data");

            if (ModelState.IsValid)
            {
                db.Entry(configoption6).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(configoption6);
            return View(configoption6);
        }

        // GET: /ConfigOption6/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigOption6 configoption6 = db.ConfigOption6.Find(id);
            if (configoption6 == null)
            {
                return HttpNotFound();
            }
            return View(configoption6);
        }

        // POST: /ConfigOption6/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ConfigOption6 configoption6 = db.ConfigOption6.Find(id);
            db.ConfigOption6.Remove(configoption6);
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
            var ConfigNameList = from firstList in db.ConfigOption6
                                 group firstList by firstList.ConfigName into newList1
                                 let x = newList1.FirstOrDefault()
                                 select x;

            var ConfigDataList = from secondList in db.ConfigOption6
                                 group secondList by secondList.ConfigData into newList2
                                 let x = newList2.FirstOrDefault()
                                 select x;

            var Key1List = from thirdList in db.ConfigOption6
                           group thirdList by thirdList.Key1 into newList3
                           let x = newList3.FirstOrDefault()
                           select x;

            var Key2List = from fourthList in db.ConfigOption6
                           group fourthList by fourthList.Key2 into newList4
                           let x = newList4.FirstOrDefault()
                           select x;

            var Key3List = from fifthList in db.ConfigOption6
                           group fifthList by fifthList.Key3 into newList5
                           let x = newList5.FirstOrDefault()
                           select x;

            var Key4List = from sixthList in db.ConfigOption6
                           group sixthList by sixthList.Key4 into newList6
                           let x = newList6.FirstOrDefault()
                           select x;

            var Key5List = from seventhList in db.ConfigOption6
                           group seventhList by seventhList.Key5 into newList7
                           let x = newList7.FirstOrDefault()
                           select x;

            var Key6List = from eigthList in db.ConfigOption6
                           group eigthList by eigthList.Key6 into newList8
                           let x = newList8.FirstOrDefault()
                           select x;

            var ConfigOptionList = from thirteenthList in db.ConfigOption6
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
            ViewBag.ConfigOption = new SelectList(ConfigOptionList.ToList(), "ConfigOption", "ConfigOption");
        }

        private void GenerateDropDowns(ConfigOption6 configoption6)
        {
            ViewBag.ConfigName = new SelectList(db.ConfigOption6.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName", configoption6.ConfigName);
            ViewBag.ConfigData = new SelectList(db.ConfigOption6.OrderBy(x => x.ConfigData), "ConfigData", "ConfigData", configoption6.ConfigData);
            ViewBag.Key1 = new SelectList(db.ConfigOption6.OrderBy(x => x.Key1), "Key1", "Key1", configoption6.Key1);
            ViewBag.Key2 = new SelectList(db.ConfigOption6.OrderBy(x => x.Key2), "Key2", "Key2", configoption6.Key2);
            ViewBag.Key3 = new SelectList(db.ConfigOption6.OrderBy(x => x.Key3), "Key3", "Key3", configoption6.Key3);
            ViewBag.Key4 = new SelectList(db.ConfigOption6.OrderBy(x => x.Key4), "Key4", "Key4", configoption6.Key4);
            ViewBag.Key5 = new SelectList(db.ConfigOption6.OrderBy(x => x.Key5), "Key5", "Key5", configoption6.Key5);
            ViewBag.Key6 = new SelectList(db.ConfigOption6.OrderBy(x => x.Key6), "Key6", "Key6", configoption6.Key6);
            ViewBag.ConfigOption = new SelectList(db.ConfigOption6.OrderBy(x => x.ConfigOption), "ConfigOption", "ConfigOption", configoption6.ConfigOption);
        }
    }
}
