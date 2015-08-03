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
            var structureSeq = db.StructureSeqs.OrderBy(x => x.ConfigName).ThenBy(x => x.ConfigData).ThenBy(x => x.ConfigData).ToList();

            // Creates the drop down list for ConfigName in the view
            var ddlConfigNames = db.StructureSeqs.Select(x => x.ConfigName).Distinct();
            List<SelectListItem> configNames = new List<SelectListItem>();
            foreach (var item in ddlConfigNames)
            {
                configNames.Add(new SelectListItem { Text = item, Value = item });
            }
            ViewBag.ConfigNames = configNames;

            // Creates the drop down list for ConfigData in the view
            var ddlConfigD = db.StructureSeqs.Select(x => x.ConfigData).Distinct();
            List<SelectListItem> configData = new List<SelectListItem>();
            foreach (var item in ddlConfigD)
            {
                configData.Add(new SelectListItem { Text = item, Value = item });
            }
            ViewBag.ConfigData = configData;

            // Returning the data based on the search filter
            if (!String.IsNullOrEmpty(ConfigNames))
            {
                if (!String.IsNullOrEmpty(ConfigData))
                {
                    structureSeq = db.StructureSeqs.Where(x => x.ConfigName == ConfigNames && x.ConfigData == ConfigData).OrderBy(x => x.ConfigName).ThenBy(x => x.ConfigData).ToList();

                    return View(structureSeq);
                }

                structureSeq = db.StructureSeqs.Where(x => x.ConfigName == ConfigNames).OrderBy(x => x.ConfigName).ThenBy(x => x.ConfigData).ToList();

                return View(structureSeq);
            }
            else if (!String.IsNullOrEmpty(ConfigData))
            {
                structureSeq = db.StructureSeqs.Where(x => x.ConfigData == ConfigData).OrderBy(x => x.ConfigName).ThenBy(x => x.ConfigData).ToList();

                return View(structureSeq);
            }
            else
            {
                return View(structureSeq);
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
        public ActionResult Create([Bind(Exclude="Id")] StructureSeq structureseq)
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
            //prevents a duplicate from being saved when editing
            var Configs = db.StructureSeqs.FirstOrDefault(x => x.ConfigName == structureseq.ConfigName && x.ConfigData == structureseq.ConfigData 
                && x.Sequence == structureseq.Sequence && x.Lookup == structureseq.Lookup && x.LookupSequence == structureseq.LookupSequence && x.Id != structureseq.Id);

            //displays if previous code found a duplicate
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
