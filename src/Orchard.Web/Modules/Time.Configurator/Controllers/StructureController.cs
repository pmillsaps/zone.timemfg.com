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
    public class StructureController : Controller
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public StructureController(IOrchardServices services)
        {
            Services = services;
            db = new ConfiguratorEntities();
        }

        public StructureController(IOrchardServices services, ConfiguratorEntities _db)
        {
           Services = services;
            db = _db;
        }

        // GET: Structures
        public ActionResult Index(string ConfigDropDown)
        {
            if (!String.IsNullOrEmpty(ConfigDropDown))
            {
                ViewData["ConfigDropDown"] = db.ConfiguratorNames.OrderBy(x => x.ConfigName).ToList();
                return View(db.Structures.Where(x => x.ConfigName == ConfigDropDown).OrderBy(x => x.ConfigName).ThenBy(x => x.ConfigData).ToList());
            }
            else
            {
                ViewData["ConfigDropDown"] = db.ConfiguratorNames.OrderBy(x => x.ConfigName).ToList();
                return View(db.Structures.OrderBy(x => x.ConfigName).ThenBy(x => x.ConfigData).ToList());
            }
            //return View(db.Structures.OrderBy(x => x.ConfigName).Distinct().ToList());
        }

        // GET: /Structure/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Structure structure = db.Structures.Find(id);
            if (structure == null)
            {
                return HttpNotFound();
            }
            var structureSeq = db.StructureSeqs.Where(x => x.ConfigName == structure.ConfigName && x.ConfigData == structure.ConfigData).ToList();
            ViewBag.StructureSeq = structureSeq;
            return View(structure);
        }

        //GET: /Strucute/Edit_Seq --- this method is being called from the Details view
        public ActionResult Edit_Seq(int? id)
        {
            StructureSeq structureSeq = db.StructureSeqs.Find(id);
            if (structureSeq == null)
            {
                return HttpNotFound();
            }
            return View(structureSeq);
        }

        // POST: /Structure/Edit_Seq
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Seq(StructureSeq structureSeq)
        {
            var Configs = db.StructureSeqs.FirstOrDefault(x => x.ConfigName == structureSeq.ConfigName && x.ConfigData == structureSeq.ConfigData 
                && x.Sequence == structureSeq.Sequence && x.Lookup == structureSeq.Lookup && x.LookupSequence == structureSeq.LookupSequence
                && x.Id != structureSeq.Id);

            if (Configs != null) ModelState.AddModelError("", "Duplicate Structure Created---Please Check Data");

            if (ModelState.IsValid)
            {
                db.Entry(structureSeq).State = EntityState.Modified;
                db.SaveChanges();
                var structure = db.Structures.First(x => x.ConfigName == structureSeq.ConfigName && x.ConfigData == structureSeq.ConfigData);
                return RedirectToAction("Details", new { id = structure.Id});
            }
            return View(structureSeq);
        }

        public ActionResult Add_Seq(int id)
        {

            return View();
        }

        public ActionResult Copy_Seq(StructureSeq structureSeq)
        {
            return View();
        }

        public ActionResult Import_Seq(StructureSeq structureSeq)
        {
            return View();
        }

        // GET: /Structure/Create
        public ActionResult Create()
        {
            GenerateDropDowns();
            return View();
        }

        // POST: /Structure/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude="Id")] Structure structure)
        {

            var Configs = db.Structures.FirstOrDefault(x => x.ConfigName == structure.ConfigName && x.ConfigData == structure.ConfigData);

            if (Configs != null) ModelState.AddModelError("", "Duplicate Structure Created---Please Check Data");

            if (ModelState.IsValid)
            {
                db.Structures.Add(structure);
                db.StructureSeqs.Add(new StructureSeq { ConfigName = structure.ConfigName, ConfigData = structure.ConfigData, Sequence = 1, Global = false});
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(structure);
            return View(structure);
        }

        // GET: /Structure/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Structure structure = db.Structures.Find(id);
            if (structure == null)
            {
                return HttpNotFound();
            }
            GenerateDropDowns();
            return View(structure);
        }

        // POST: /Structure/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Structure structure)
        {
            var Configs = db.Structures.FirstOrDefault(x => x.ConfigName == structure.ConfigName && x.ConfigData == structure.ConfigData && x.Id != structure.Id);

            if (Configs != null) ModelState.AddModelError("", "Duplicate Structure Created---Please Check Data");

            if (ModelState.IsValid)
            {
                db.Entry(structure).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(structure);
            return View(structure);
        }

        // GET: /Structure/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Structure structure = db.Structures.Find(id);
            if (structure == null)
            {
                return HttpNotFound();
            }
            return View(structure);
        }

        // POST: /Structure/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Structure structure = db.Structures.Find(id);
            db.Structures.Remove(structure);
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
            //var ConfigNameList = from firstList in db.Structures
            //                     group firstList by firstList.ConfigName into newList1
            //                     let x = newList1.FirstOrDefault()
            //                     select x;

            var ConfigDataList = from secondList in db.Structures
                                 group secondList by secondList.ConfigName into newList2
                                 let x = newList2.FirstOrDefault()
                                 select x;

            ViewBag.ConfigName = new SelectList(db.ConfiguratorNames.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName"); 
            ViewBag.ConfigData = new SelectList(ConfigDataList.ToList(), "ConfigData", "ConfigData");
        }

        private void GenerateDropDowns(Structure structure)
        {
            ViewBag.ConfigName = new SelectList(db.ConfiguratorNames.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName", structure.ConfigName);
            ViewBag.ConfigData = new SelectList(db.Structures.OrderBy(x => x.ConfigData), "ConfigData", "ConfigData", structure.ConfigData);
        }
    }
}
