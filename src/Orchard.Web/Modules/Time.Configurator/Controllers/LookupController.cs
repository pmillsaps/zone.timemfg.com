﻿using Orchard;
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
        public ActionResult Index()
        {
            return View(db.Lookups.OrderBy(x => x.ConfigName).ThenBy(x => x.ConfigData).ThenBy(x => x.Data).ToList());
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
        public ActionResult Create([Bind(Exclude="Id")] Lookup lookup)
        {
            var Configs = db.Lookups.FirstOrDefault(x => x.ConfigName == lookup.ConfigName && x.ConfigData == lookup.ConfigData && x.Sequence == lookup.Sequence
            && x.Data == lookup.Data);

            if (Configs != null) ModelState.AddModelError("", "Duplicate Lookup Created---Please Recheck Data");

            if (ModelState.IsValid)
            {
                db.Lookups.Add(lookup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(lookup);
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
            var Configs = db.Lookups.FirstOrDefault(x => x.ConfigName == lookup.ConfigName && x.ConfigData == lookup.ConfigData && x.Sequence == lookup.Sequence 
                && x.Data == lookup.Data && x.Id != lookup.Id);

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
            var ConfigNameList = from firstList in db.Lookups
                                 group firstList by firstList.ConfigName into newList1
                                 let x = newList1.FirstOrDefault()
                                 select x;

            var ConfigDataList = from secondList in db.Lookups
                                 group secondList by secondList.ConfigData into newList2
                                 let x = newList2.FirstOrDefault()
                                 select x;

            var SequenceList = from thirdList in db.Lookups
                               group thirdList by thirdList.Sequence into newList3
                               let x = newList3.FirstOrDefault()
                               select x;

            var DataList = from fourthList in db.Lookups
                           group fourthList by fourthList.Data into newList4
                           let x = newList4.FirstOrDefault()
                           select x;

            ViewBag.ConfigName = new SelectList(ConfigNameList.ToList(), "ConfigName", "ConfigName");
            ViewBag.ConfigData = new SelectList(ConfigDataList.ToList(), "ConfigData", "ConfigData");
            ViewBag.Sequence = new SelectList(SequenceList.ToList(), "Sequence", "Sequence");
            ViewBag.Data = new SelectList(DataList.ToList(), "Data", "Data");
        }

        private void GenerateDropDowns(Lookup lookup)
        {
            ViewBag.ConfigName = new SelectList(db.Lookups.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName", lookup.ConfigName);
            ViewBag.ConfigData = new SelectList(db.Lookups.OrderBy(x => x.ConfigData), "ConfigData", "ConfigData", lookup.ConfigData);
            ViewBag.Sequence = new SelectList(db.Lookups.OrderBy(x => x.Sequence), "Sequence", "Sequence", lookup.Sequence);
            ViewBag.Data = new SelectList(db.Lookups.OrderBy(x => x.Data), "Data", "Data", lookup.Data);
        }
    }
}
