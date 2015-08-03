using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using MoreLinq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.Configurator;

namespace Time.Configurator.Controllers
{
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
        public ActionResult Index()
        {
            return View(db.StructureSeqs.OrderBy(x => x.ConfigName).ThenBy(x => x.ConfigData).ToList());
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
        public ActionResult Create([Bind(Exclude="Id")] StructureSeq structureseq)
        {
            var Configs = db.StructureSeqs.FirstOrDefault(x => x.ConfigName == structureseq.ConfigName && x.ConfigData == structureseq.ConfigData
            && x.Sequence == structureseq.Sequence);

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
            StructureSeq structureseq = db.StructureSeqs.Find(id);
            if (structureseq == null)
            {
                return HttpNotFound();
            }
            GenerateDropDowns();
            return View(structureseq);
        }

        // POST: /StructureSeq/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StructureSeq structureseq)
        {

            var Configs = db.StructureSeqs.FirstOrDefault(x => x.ConfigName == structureseq.ConfigName && x.ConfigData == structureseq.ConfigData 
                && x.Sequence == structureseq.Sequence && x.Lookup == structureseq.Lookup && x.LookupSequence == structureseq.LookupSequence && x.Id != structureseq.Id);

            if (Configs != null) ModelState.AddModelError("", "Duplicate Structure Sequence Created---Please Recheck Data");

            if (ModelState.IsValid)
            {
                db.Entry(structureseq).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(structureseq);
            return View(structureseq);
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
            //var ConfigNameList = from firstList in db.StructureSeqs
            //                     group firstList by firstList.ConfigName into newList1
            //                     let x = newList1.FirstOrDefault()
            //                     select x;
            //var ConfigDataList = from secondList in db.StructureSeqs
            //                     group secondList by secondList.ConfigData into newList2
            //                     let x = newList2.FirstOrDefault()
            //                     select x;
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
            //ViewBag.ConfigData = new SelectList(db.Structures.OrderBy(x => x.ConfigData), "ConfigData", "ConfigData");
            ViewBag.ConfigData = new SelectList(db.Structures.Select(x => x.ConfigData).Distinct());
            ViewBag.Sequence = new SelectList(db.StructureSeqs.OrderBy(x => x.Sequence), "Sequence", "Sequence");
            ViewBag.Lookup = new SelectList(db.StructureSeqs.OrderBy(x => x.Lookup), "Lookup", "Lookup");
            ViewBag.LookupSequence = new SelectList(db.StructureSeqs.OrderBy(x => x.LookupSequence), "LookupSequence", "LookupSequence");
        }

        private void GenerateDropDowns(StructureSeq structureseq)
        {
            ViewBag.ConfigName = new SelectList(db.ConfiguratorNames.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName", structureseq.ConfigName);
            //ViewBag.ConfigData = new SelectList(db.Structures.OrderBy(x => x.ConfigData), "ConfigData", "ConfigData", structureseq.ConfigData);
            ViewBag.ConfigData = new SelectList(db.Structures.Select(x => x.ConfigData).Distinct(), structureseq.ConfigData);
            ViewBag.Sequence = new SelectList(db.StructureSeqs.OrderBy(x => x.Sequence), "Sequence", "Sequence", structureseq.Sequence);
            ViewBag.Lookup = new SelectList(db.StructureSeqs.OrderBy(x => x.Lookup), "Lookup", "Lookup", structureseq.Lookup);
            ViewBag.LookupSequence = new SelectList(db.StructureSeqs.OrderBy(x => x.LookupSequence), "LookupSequence", "LookupSequence", structureseq.LookupSequence);
        }
    }
}
