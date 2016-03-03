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
    public class ConfiguratorNameController : Controller
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ConfiguratorNameController(IOrchardServices services)
        {
            Services = services;
            db = new ConfiguratorEntities();
        }

        public ConfiguratorNameController(IOrchardServices services, ConfiguratorEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: /ConfiguratorName/
        public ActionResult Index()
        {
            return View(db.ConfiguratorNames.OrderBy(x => x.ConfigName).ToList());
        }

        // GET: /ConfiguratorName/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfiguratorName configuratorname = db.ConfiguratorNames.Find(id);
            if (configuratorname == null)
            {
                return HttpNotFound();
            }
            return View(configuratorname);
        }

        // GET: /ConfiguratorName/Copy
        //this is used to copy all the configurator data in one configurator to a brand new one that you create
        public ActionResult Copy(int id = 0)
        {
            var configName = db.ConfiguratorNames.Where(x => x.Id == id).Select(x => new { cName = x.ConfigName }).Single();
            string conName = configName.cName;
            ViewBag.ConfiguratorName = conName;

            var structure = db.Structures.Where(x => x.ConfigName == conName).ToList();
            ViewBag.Structure = structure;

            var structureSeq = db.StructureSeqs.Where(x => x.ConfigName == conName).ToList();
            ViewBag.StructureSeq = structureSeq;

            var lookup = db.Lookups.Where(x => x.ConfigName == conName).ToList();
            ViewBag.Lookup = lookup;

            return View();
        }

        // POST: /ConfiguratorName/Copy
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Copy(ConfiguratorName configuratorname)
        {
            //prevents a duplicate from being created when copying
            var Configs = db.ConfiguratorNames.FirstOrDefault(x => x.ConfigName == configuratorname.ConfigName);

            if (Configs != null) ModelState.AddModelError("ConfigName", "Configurator Name Already Exists");

            if (ModelState.IsValid)
            {
                // Inserting the data in the Configurator Name, Structure, and Structure Sequence tables after validation
                ConfiguratorName configuratorNameNew = new ConfiguratorName { ConfigName = configuratorname.ConfigName };
                db.ConfiguratorNames.Add(configuratorNameNew);
                db.SaveChanges();

                //inputs copied data into structures table
                var cN = db.ConfiguratorNames.Find(configuratorname.Id);
                var st = db.Structures.Where(x => x.ConfigName == cN.ConfigName).ToList();
                foreach (var item in st)
                {
                    Structure structureNew = new Structure { ConfigName = configuratorname.ConfigName, ConfigData = item.ConfigData };
                    db.Structures.Add(structureNew);
                }
                db.SaveChanges();

                //inputs copied data into structure sequence table
                var stSq = db.StructureSeqs.Where(x => x.ConfigName == cN.ConfigName).ToList();
                foreach (var item in stSq)
                {
                    StructureSeq structureSeqNew = new StructureSeq
                    {
                        ConfigName = configuratorname.ConfigName,
                        ConfigData = item.ConfigData,
                        Sequence = item.Sequence,
                        Lookup = item.Lookup,
                        LookupSequence = item.LookupSequence,
                        Global = item.Global,
                        Notes = item.Notes
                    };
                    db.StructureSeqs.Add(structureSeqNew);
                }
                db.SaveChanges();

                //inputs copied data into lookups table
                var lkp = db.Lookups.Where(x => x.ConfigName == cN.ConfigName).ToList();
                foreach (var item in lkp)
                {
                    Lookup lookupNew = new Lookup
                    {
                        ConfigName = configuratorname.ConfigName,
                        ConfigData = item.ConfigData,
                        Sequence = item.Sequence,
                        Data = item.Data,
                        PickDefault = item.PickDefault,
                        Inactive = item.Inactive
                    };
                    db.Lookups.Add(lookupNew);
                }
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            //pulling information from each table
            var configName = db.ConfiguratorNames.Where(x => x.Id == configuratorname.Id).Select(x => new { cName = x.ConfigName }).Single();
            string conName = configName.cName;
            ViewBag.ConfiguratorName = conName;

            var structure = db.Structures.Where(x => x.ConfigName == conName).ToList();
            ViewBag.Structure = structure;

            var structureSeq = db.StructureSeqs.Where(x => x.ConfigName == conName).ToList();
            ViewBag.StructureSeq = structureSeq;

            var lookup = db.Lookups.Where(x => x.ConfigName == conName).ToList();
            ViewBag.Lookup = lookup;

            return View();
        }

        // GET: /ConfiguratorName/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /ConfiguratorName/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] ConfiguratorName configuratorname)
        {
            //prevents a duplicate from being created
            var Configs = db.ConfiguratorNames.FirstOrDefault(x => x.ConfigName == configuratorname.ConfigName);

            //displays if previous code found a duplicate
            if (Configs != null) ModelState.AddModelError("ConfigName", "Configurator Name Already Exists");

            if (ModelState.IsValid)
            {
                db.ConfiguratorNames.Add(configuratorname);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(configuratorname);
        }

        // GET: /ConfiguratorName/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfiguratorName configuratorname = db.ConfiguratorNames.Find(id);
            if (configuratorname == null)
            {
                return HttpNotFound();
            }
            return View(configuratorname);
        }

        // POST: /ConfiguratorName/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ConfiguratorName configuratorname)
        {
            //prevents a duplicate from being saved when editing
            var Configs = db.ConfiguratorNames.FirstOrDefault(x => x.ConfigName == configuratorname.ConfigName && x.Id != configuratorname.Id);

            //displays if previous code found a duplicate
            if (Configs != null) ModelState.AddModelError("ConfigName", "Duplicate Configurator Name");

            if (ModelState.IsValid)
            {
                db.Entry(configuratorname).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(configuratorname);
        }

        // GET: /ConfiguratorName/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfiguratorName configuratorname = db.ConfiguratorNames.Find(id);
            if (configuratorname == null)
            {
                return HttpNotFound();
            }
            return View(configuratorname);
        }

        // POST: /ConfiguratorName/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ConfiguratorName configuratorname = db.ConfiguratorNames.Find(id);
            db.ConfiguratorNames.Remove(configuratorname);
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
    }
}