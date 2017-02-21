using MoreLinq;
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
    public class StructureSeqController : Controller
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public StructureSeqController(IOrchardServices services)
        {
            Services = services;
            db = new ConfiguratorEntities();
        }

        public StructureSeqController(IOrchardServices services, ConfiguratorEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: /StructureSeq/
        public ActionResult Index(string ConfigNames, string ConfigData)
        {
            ViewBag.ConfigNames = new SelectList(db.ConfiguratorNames.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName");
            ViewBag.ConfigData = new SelectList(db.StructureSeqs.Select(x => new { x.ConfigData }).Distinct().OrderBy(x => x.ConfigData), "ConfigData", "ConfigData");

            if (String.IsNullOrEmpty(ConfigNames) && String.IsNullOrEmpty(ConfigData))
            {
                return View();
            }
            else
            {
                //CODEREVIEW: Use Iqueryable to build the return data, this will avoid some roundtrips to the database
                var structureSeq = db.StructureSeqs.AsQueryable();
                if (!String.IsNullOrEmpty(ConfigNames)) structureSeq = structureSeq.Where(x => x.ConfigName == ConfigNames);
                if (!String.IsNullOrEmpty(ConfigData)) structureSeq = structureSeq.Where(x => x.ConfigData == ConfigData);

                return View(structureSeq.OrderBy(x => x.ConfigName).ThenBy(x => x.ConfigData).ToList());
            }
        }

        // GET: /StructureSeq/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StructureSeq structureseq = db.StructureSeqs.Find(id);
            if (structureseq == null)
            {
                return HttpNotFound();
            }
            return View(structureseq);
        }

        // GET: /StructureSeq/Create
        public ActionResult Create()
        {
            GenerateDropDowns();
            return View();
        }

        // POST: /StructureSeq/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] StructureSeq structureseq)
        {
            //prevents a duplicate from being created
            var Configs = db.StructureSeqs.FirstOrDefault(x => x.ConfigName == structureseq.ConfigName && x.ConfigData == structureseq.ConfigData
            && x.Sequence == structureseq.Sequence);

            //displays if previous code found a duplicate
            if (Configs != null) ModelState.AddModelError("", "Duplicate Structure Sequence Created---Please Recheck Data");

            if (ModelState.IsValid)
            {
                db.StructureSeqs.Add(structureseq);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(structureseq);
            return View(structureseq);
        }

        // GET: /StructureSeq/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string selected = "";// This variable sets the current Lookup selection in Drop Down List
            StructureSeq structureSeq = db.StructureSeqs.Find(id);
            if (structureSeq == null)
            {
                return HttpNotFound();
            }
            if (structureSeq.Lookup == null) selected = "-- Select --";
            else selected = structureSeq.Lookup.ToString();
            ViewBag.Selected = selected;
            ViewBag.Lookup = new SelectList(db.StructureSeqs.Select(x => new { x.ConfigData }).Distinct().OrderBy(x => x.ConfigData), "ConfigData", "ConfigData", selected);
            return View(structureSeq);
        }

        // POST: /StructureSeq/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StructureSeq structureSeq)
        {
            string selected = "";// This variable sets the current Lookup selection in Drop Down List
            //prevents a duplicate from being saved when editing
            var Configs = db.StructureSeqs.FirstOrDefault(x => x.ConfigName == structureSeq.ConfigName && x.ConfigData == structureSeq.ConfigData
                && x.Sequence == structureSeq.Sequence && x.Lookup == structureSeq.Lookup && x.LookupSequence == structureSeq.LookupSequence && x.Id != structureSeq.Id);

            //displays if previous code found a duplicate
            if (Configs != null) ModelState.AddModelError("", "Duplicate Structure Sequence Created---Please Recheck Data");

            if (ModelState.IsValid)
            {
                db.Entry(structureSeq).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { ConfigNames = structureSeq.ConfigName, ConfigData = structureSeq.ConfigData });
            }
            if (structureSeq.Lookup == null) selected = "-- Select --";
            else selected = structureSeq.Lookup.ToString();
            ViewBag.Selected = selected;
            ViewBag.Lookup = new SelectList(db.StructureSeqs.Select(x => new { x.ConfigData }).Distinct().OrderBy(x => x.ConfigData), "ConfigData", "ConfigData", selected);
            return View(structureSeq);
        }

        // GET: /StructureSeq/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StructureSeq structureseq = db.StructureSeqs.Find(id);
            if (structureseq == null)
            {
                return HttpNotFound();
            }
            return View(structureseq);
        }

        // POST: /StructureSeq/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StructureSeq structureseq = db.StructureSeqs.Find(id);
            db.StructureSeqs.Remove(structureseq);
            db.SaveChanges();
            return RedirectToAction("Index", new { ConfigNames = structureseq.ConfigName, ConfigData = structureseq.ConfigData });
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
            var SequenceList = from thirdList in db.StructureSeqs
                               group thirdList by thirdList.Sequence into newList3
                               let x = newList3.FirstOrDefault()
                               select x;
            var LookupList = from fourthList in db.StructureSeqs
                             group fourthList by fourthList.Lookup into newList4
                             let x = newList4.FirstOrDefault()
                             select x;
            var LookupSequenceList = from fifthList in db.StructureSeqs
                                     group fifthList by fifthList.LookupSequence into newList5
                                     let x = newList5.FirstOrDefault()
                                     select x;

            ViewBag.ConfigName = new SelectList(db.ConfiguratorNames.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName");
            //ViewBag.ConfigData = new SelectList(db.Structures.OrderBy(x => x.ConfigData), "ConfigData", "ConfigData");                                          //shows all values
            ViewBag.ConfigData = new SelectList(db.Structures.Select(x => x.ConfigData).Distinct());                                                              //shows distinct values
            ViewBag.Sequence = new SelectList(db.StructureSeqs.OrderBy(x => x.Sequence), "Sequence", "Sequence");
            ViewBag.Lookup = new SelectList(db.StructureSeqs.OrderBy(x => x.Lookup), "Lookup", "Lookup");
            ViewBag.LookupSequence = new SelectList(db.StructureSeqs.OrderBy(x => x.LookupSequence), "LookupSequence", "LookupSequence");
        }

        //This and above ViewBags pull in the data to put into the drop down lists
        private void GenerateDropDowns(StructureSeq structureseq)
        {
            ViewBag.ConfigName = new SelectList(db.ConfiguratorNames.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName", structureseq.ConfigName);
            //ViewBag.ConfigData = new SelectList(db.Structures.OrderBy(x => x.ConfigData), "ConfigData", "ConfigData", structureseq.ConfigData);                 //shows all values
            ViewBag.ConfigData = new SelectList(db.Structures.Select(x => x.ConfigData).Distinct(), structureseq.ConfigData);                                     //shows distinct values
            ViewBag.Sequence = new SelectList(db.StructureSeqs.OrderBy(x => x.Sequence), "Sequence", "Sequence", structureseq.Sequence);
            ViewBag.Lookup = new SelectList(db.StructureSeqs.OrderBy(x => x.Lookup), "Lookup", "Lookup", structureseq.Lookup);
            ViewBag.LookupSequence = new SelectList(db.StructureSeqs.OrderBy(x => x.LookupSequence), "LookupSequence", "LookupSequence", structureseq.LookupSequence);
        }
    }
}