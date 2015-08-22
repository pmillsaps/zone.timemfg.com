using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Time.Data.EntityModels.Configurator;

namespace Time.Configurator.Controllers
{
    //sets theme and requires you to log in to go to the page
    [Themed]
    [Authorize]
    public class StructureController : Controller
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        private string[] tokens;

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
        public ActionResult Index(string ConfigNames)
        {
            var structures = db.Structures.AsQueryable();
            if (!String.IsNullOrEmpty(ConfigNames)) structures = structures.Where(x => x.ConfigName == ConfigNames);

            ViewBag.ConfigNames = new SelectList(db.ConfiguratorNames.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName");

            return View(structures.OrderBy(x => x.ConfigName).ThenBy(x => x.ConfigData).ToList());
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

            //pulls in data for the structure sequence information in the structure details of each structure
            var structureSeq = db.StructureSeqs.Where(x => x.ConfigName == structure.ConfigName && x.ConfigData == structure.ConfigData).ToList();
            ViewBag.StructureSeq = structureSeq;

            //pulls in data for the complex lookup partial view information in the structure details of each structure
            List<Lookup> lookup = db.Lookups.Where(x => x.ConfigName == structure.ConfigName && x.ConfigData == structure.ConfigData).ToList();
            ViewBag.Lookup = lookup;

            //pulls in data for the complex structure partial view information in the structure details of each structure
            List<ComplexStructure> complexStructure = db.ComplexStructures.Where(x => x.ConfigName == structure.ConfigName && x.ConfigData == structure.ConfigData).ToList();
            ViewBag.ComplexStructure = complexStructure;

            //pulls in data for the complex lookup partial view information in the structure details of each structure
            List<ComplexLookup> complexLookup = db.ComplexLookups.Where(x => x.ConfigName == structure.ConfigName && x.ConfigData == structure.ConfigData).ToList();
            ViewBag.ComplexLookup = complexLookup;

            return View(structure);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////Edit_Seq/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //GET: /Structure/Edit_Seq --- this method is being called from the Details view
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
            //prevents duplicate data from being saved while editing
            var Configs = db.StructureSeqs.FirstOrDefault(x => x.ConfigName == structureSeq.ConfigName && x.ConfigData == structureSeq.ConfigData
                && x.Sequence == structureSeq.Sequence && x.Lookup == structureSeq.Lookup && x.LookupSequence == structureSeq.LookupSequence
                && x.Id != structureSeq.Id);

            //displays if previous code found a duplicate
            if (Configs != null) ModelState.AddModelError("", "Duplicate Sequence Created---Please Check Data");

            if (ModelState.IsValid)
            {
                db.Entry(structureSeq).State = EntityState.Modified;
                db.SaveChanges();
                var structure = db.Structures.First(x => x.ConfigName == structureSeq.ConfigName && x.ConfigData == structureSeq.ConfigData);
                return RedirectToAction("Details", new { id = structure.Id });
            }
            return View(structureSeq);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////////////////////////Add_Seq///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //GET: /Structure/Add_Seq
        public ActionResult Add_Seq(int id)
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
            ViewBag.ConfigName = structure.ConfigName;
            ViewBag.ConfigData = structure.ConfigData;
            var sequenceNum = db.StructureSeqs.Where(x => x.ConfigName == structure.ConfigName && x.ConfigData == structure.ConfigData).ToList().Max(x => Convert.ToInt32(x.Sequence));
            ViewBag.Sequence = sequenceNum + 1;
            return View();
        }

        // POST: /Structure/Add_Seq
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add_Seq([Bind(Exclude = "Id")] StructureSeq structureSeq)
        {
            //prevents duplicate code when adding a sequence
            var Configs = db.StructureSeqs.FirstOrDefault(x => x.ConfigName == structureSeq.ConfigName && x.ConfigData == structureSeq.ConfigData
                && x.Sequence == structureSeq.Sequence && x.Lookup == structureSeq.Lookup && x.LookupSequence == structureSeq.LookupSequence
                && x.Id != structureSeq.Id);

            //displays if previous code found a duplicate
            if (Configs != null) ModelState.AddModelError("", "Duplicate Sequence Created---Please Check Data");

            if (ModelState.IsValid)
            {
                db.StructureSeqs.Add(structureSeq);
                db.SaveChanges();
                var structure = db.Structures.First(x => x.ConfigName == structureSeq.ConfigName && x.ConfigData == structureSeq.ConfigData);
                return RedirectToAction("Details", new { id = structure.Id });
            }
            return View(structureSeq);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //////////////////////////////////////////////////////////////////////////////////////Import_Seq//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //GET: /Structure/Import_Seq
        public ActionResult Import_Seq(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StructureSeq structureSeq = db.StructureSeqs.Find(id);
            if (structureSeq == null)
            {
                return HttpNotFound();
            }
            Lookup lookup = new Lookup();
            lookup.ConfigName = structureSeq.ConfigName;
            lookup.ConfigData = structureSeq.ConfigData;
            lookup.Sequence = structureSeq.Sequence;
            lookup.PickDefault = false;
            lookup.Inactive = false;
            return View(lookup);
        }

        // POST: /Structure/Import_Seq
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import_Seq(Lookup lookup)
        {
            //if (String.IsNullOrEmpty(lookup.Data)) ModelState.AddModelError("Data", "Data field is required");

            if (ModelState.IsValid)
            {
                tokens = SplitLookupData(lookup.Data);

                //for loop that checks each entry in the import data box to see if it is a duplicate
                foreach (var item in tokens)
                {
                    var Configs = db.Lookups.FirstOrDefault(x => x.ConfigName == lookup.ConfigName && x.ConfigData == lookup.ConfigData && x.Sequence == lookup.Sequence && x.Data == item.Trim());

                    //displays if previous code found a duplicate
                    if (Configs != null) ModelState.AddModelError("", " Item '" + item + "' is a Duplicate---Remove Duplicate (No data imported)");
                }
            }
                
            if (ModelState.IsValid)
            {
                //for loop that loops through and inputs each piece of data
                foreach (var item in tokens)
                {
                    Lookup lookupNew = new Lookup
                    {
                        ConfigName = lookup.ConfigName,
                        ConfigData = lookup.ConfigData,
                        Sequence = lookup.Sequence,
                        Data = item.Trim(),
                        PickDefault = lookup.PickDefault,
                        Inactive = lookup.Inactive
                    };

                    db.Lookups.Add(lookupNew);
                }

                db.SaveChanges();
                return RedirectToAction("Index", "Lookup");
            }
            return View(lookup);
        }

        // This method is called from Import_Seq to parse the input in the Data textbox
        private string[] SplitLookupData(string rawString)
        {
            string[] newData = rawString.Split(new String[] { "\t", ",", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            return newData;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //////////////////////////////////////////////////////////////////////////////////////Copy_Seq/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // GET: /Structure/Copy_Seq
        public ActionResult Copy_Seq(int id)
        {
            StructureSeq structureSeq = db.StructureSeqs.Find(id);
            if (structureSeq == null)
            {
                return HttpNotFound();
            }

            // Creates the drop down list for ConfigName in the view
            var ddlConfigNames = db.ConfiguratorNames.Select(x => x.ConfigName).Distinct();
            List<SelectListItem> configNames = new List<SelectListItem>();
            foreach (var item in ddlConfigNames)
            {
                configNames.Add(new SelectListItem { Text = item, Value = item });
            }
            ViewBag.ConfigNamesTo = configNames;

            // Storing the Lookup data for the sequence in ViewBag
            var lookup = db.Lookups.Where(x => x.ConfigName == structureSeq.ConfigName && x.ConfigData == structureSeq.ConfigData).ToList();
            ViewBag.Lookup = lookup;
            return View(structureSeq);
        }

        // POST: /Structure/Copy_Seq
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Copy_Seq(StructureSeq structureSeq, string ConfigNamesTo)
        {
            // Alerting user if no item was selected in the drop down list
            if (String.IsNullOrEmpty(ConfigNamesTo)) ModelState.AddModelError("", "Select a Configurator from the list above.");

            //validation for structure data
            var ConfigStrct = db.Structures.FirstOrDefault(x => x.ConfigName == ConfigNamesTo && x.ConfigData == structureSeq.ConfigData);

            //displays if previous code found a duplicate
            if (ConfigStrct != null)
            {
                ModelState.AddModelError("", structureSeq.ConfigName + " and/or " + structureSeq.ConfigData + " already exists in Structure Table.");
            }
            if (ModelState.IsValid)
            {
                //validation for structure sequence data
                var ConfigSeq = db.StructureSeqs.FirstOrDefault(y => y.ConfigName == ConfigNamesTo && y.ConfigData == structureSeq.ConfigData && y.Sequence == structureSeq.Sequence);

                //displays if previous code found a duplicate
                if (ConfigSeq != null)
                {
                    ModelState.AddModelError("", structureSeq.ConfigName + " and/or " + structureSeq.ConfigData + " and/or Sequence " + structureSeq.Sequence + " already exists in StructureSeq Table.");
                }
            }
            if (ModelState.IsValid)
            {
                // Inserting the data in the Structure and StructureSeq tables after validation
                Structure structureNew = new Structure { ConfigName = ConfigNamesTo, ConfigData = structureSeq.ConfigData };
                db.Structures.Add(structureNew);
                StructureSeq structureSeqNew = new StructureSeq
                {
                    ConfigName = ConfigNamesTo,
                    ConfigData = structureSeq.ConfigData,
                    Sequence = structureSeq.Sequence,
                    Lookup = structureSeq.Lookup,
                    LookupSequence = structureSeq.LookupSequence,
                    Global = structureSeq.Global
                };
                db.StructureSeqs.Add(structureSeqNew);
                db.SaveChanges();

                // Call to Copy Lookup that copies each Lookup Data into new existing Configurator
                CopyLookups(structureSeq, ConfigNamesTo, structureNew);

                if (ModelState.IsValid)
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            // Creates the drop down list for ConfigName in the view
            var ddlConfigNames = db.ConfiguratorNames.Select(x => x.ConfigName).Distinct();
            List<SelectListItem> configNames = new List<SelectListItem>();
            foreach (var item in ddlConfigNames)
            {
                configNames.Add(new SelectListItem { Text = item, Value = item });
            }
            ViewBag.ConfigNamesTo = configNames;

            // Storing the Lookup data for the sequence in ViewBag
            var lookup = db.Lookups.Where(x => x.ConfigName == structureSeq.ConfigName && x.ConfigData == structureSeq.ConfigData).ToList();
            ViewBag.Lookup = lookup;

            return View(structureSeq);
        }

        /// <summary>
        /// This method copies each Lookup Data into new existing Configurator
        /// </summary>
        /// <param name="structureSeq"></param>
        /// <param name="ConfigNamesTo"></param>
        /// <param name="structureNew"></param>
        private void CopyLookups(StructureSeq structureSeq, string ConfigNamesTo, Structure structureNew)
        {
            //prevents a duplicate from being created when making a copy
            var lookupRows = db.Lookups.Where(x => x.ConfigName == structureSeq.ConfigName && x.ConfigData == structureSeq.ConfigData && x.Sequence == structureSeq.Sequence).ToList();
            foreach (var item in lookupRows)
            {
                var ConfigLkp = db.Lookups.FirstOrDefault(z => z.ConfigName == structureNew.ConfigName && z.ConfigData == item.ConfigData && z.Sequence == item.Sequence
                    && z.Data == item.Data);

                if (ConfigLkp != null)
                {
                }
                else
                {
                    Lookup lookupNew = new Lookup
                    {
                        ConfigName = ConfigNamesTo,
                        ConfigData = item.ConfigData,
                        Sequence = item.Sequence,
                        Data = item.Data,
                        PickDefault = item.PickDefault,
                        Inactive = item.Inactive
                    };
                    db.Lookups.Add(lookupNew);
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////////////////////////Add_CS///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //GET: /Structure/Add_CS
        //used to add in a new ComplexStructure
        public ActionResult Add_CS(int id)
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

            ViewBag.ConfigName = structure.ConfigName;
            ViewBag.ConfigData = structure.ConfigData;

            //makes the sequence number 1 if there were no previous sequences
            //without this it will not let you create a sequence if there were not any before, while also incrementing ones that had a sequenc
            var sequenceNum = db.ComplexStructures.Where(x => x.ConfigName == structure.ConfigName && x.ConfigData == structure.ConfigData).Max(x => (int?)x.Sequence);
            if (sequenceNum != null)
            {
                ViewBag.Sequence = sequenceNum + 1;
            }
            else
            {
                ViewBag.Sequence = 1;
            }

            GenerateComplexDropDowns(structure);
            return View();
        }

        // POST: /Structure/Add_CS
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add_CS([Bind(Exclude = "Id")] ComplexStructure complexStructure, StructureSeq structureSeq)
        {
            //prevents duplicate code when adding a sequence
            var Configs = db.ComplexStructures.FirstOrDefault(x => x.ConfigName == complexStructure.ConfigName && x.ConfigData == complexStructure.ConfigData
                && x.Sequence == complexStructure.Sequence && x.LookupData == complexStructure.LookupData && x.LookupSeq == complexStructure.LookupSeq
                && x.Id != complexStructure.Id);

            //displays if previous code found a duplicate
            if (Configs != null) ModelState.AddModelError("", "Duplicate Complex Structure Created---Please Check Data");

            if (ModelState.IsValid)
            {
                db.ComplexStructures.Add(complexStructure);
                db.SaveChanges();
                var structure = db.Structures.First(x => x.ConfigName == complexStructure.ConfigName && x.ConfigData == complexStructure.ConfigData);
                return RedirectToAction("Details", new { id = structure.Id });
            }
            GenerateComplexDropDowns(structureSeq);
            return View(complexStructure);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
        public ActionResult Create([Bind(Exclude = "Id")] Structure structure)
        {
            //prevents duplicate structures when creating a new structure
            var Configs = db.Structures.FirstOrDefault(x => x.ConfigName == structure.ConfigName && x.ConfigData == structure.ConfigData);

            //displays if previous code found a duplicate
            if (Configs != null) ModelState.AddModelError("", "Duplicate Structure Created---Please Check Data");

            if (ModelState.IsValid)
            {
                db.Structures.Add(structure);
                db.StructureSeqs.Add(new StructureSeq { ConfigName = structure.ConfigName, ConfigData = structure.ConfigData, Sequence = 1, Global = false });
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
            //prevents data from being duplicated when editing a structure
            var Configs = db.Structures.FirstOrDefault(x => x.ConfigName == structure.ConfigName && x.ConfigData == structure.ConfigData && x.Id != structure.Id);

            //displays if previous code found a duplicate
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
            var ConfigDataList = from secondList in db.Structures
                                 group secondList by secondList.ConfigData into newList2
                                 let x = newList2.FirstOrDefault()
                                 select x;

            ViewBag.ConfigName = new SelectList(db.ConfiguratorNames.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName");
            ViewBag.ConfigData = new SelectList(ConfigDataList.ToList(), "ConfigData", "ConfigData");
        }

        //This and above ViewBags pull in the data to put into the drop down lists
        private void GenerateDropDowns(Structure structure)
        {
            ViewBag.ConfigName = new SelectList(db.ConfiguratorNames.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName", structure.ConfigName);
            ViewBag.ConfigData = new SelectList(db.Structures.OrderBy(x => x.ConfigData), "ConfigData", "ConfigData", structure.ConfigData);
        }

        private void GenerateComplexDropDowns(Structure structure)
        {
            //prevent duplicates from showing up in drop down
            //without var list codes, every CFG and Global shows up in drop down and whatever else for the other drop downs
            var ConfigDataCSList = db.StructureSeqs.Where(x => x.ConfigName == structure.ConfigName).ToList();

            var SequenceCSList = from secondCSList in db.StructureSeqs
                                 group secondCSList by secondCSList.Sequence into newCSList2
                                 let x = newCSList2.FirstOrDefault()
                                 select x;

            ViewBag.LookupData = new SelectList(ConfigDataCSList.ToList(), "ConfigData", "ConfigData");
            ViewBag.LookupSeq = new SelectList(SequenceCSList.ToList(), "Sequence", "Sequence");
        }

        //This and above ViewBags pull in the data to put into the drop down lists
        private void GenerateComplexDropDowns(StructureSeq structureSeq)
        {
            ViewBag.LookupData = new SelectList(db.StructureSeqs.OrderBy(x => x.ConfigData), "ConfigData", "ConfigData", structureSeq.ConfigData);
            ViewBag.LookupSeq = new SelectList(db.StructureSeqs.OrderBy(x => x.Sequence), "Sequence", "Sequence", structureSeq.Sequence);
        }
    }
}