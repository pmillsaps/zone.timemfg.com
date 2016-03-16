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
using Time.Configurator.Helpers;
using Time.Configurator.Models;
using Time.Configurator.Services;
using Time.Configurator.ViewModels;
using Time.Data.EntityModels.Configurator;

namespace Time.Configurator.Controllers
{
    [Themed]
    [Authorize]
    public class SpecialDatasController : Controller
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        private ISpecialDataExportExcelHelper _scfgSpDtExHp = new SpecialDataExportExcelHelper();

        // These lists will hold the values for the Import method.  They are also used by the SplitData() method.
        public List<string> part = new List<string>();
        public List<decimal> quantity = new List<decimal>();
        public List<decimal> price = new List<decimal>();
        public List<string> datatypename = new List<string>();
        public List<int> datatype = new List<int>();
        public List<string> relatedopname = new List<string>();
        public List<int> relatedop = new List<int>();
        public bool relatedopEmpty = false; // To insert the Price in RelatedOpId if no RelatedOpId provided

        public SpecialDatasController(IOrchardServices services)
        {
            Services = services;
            db = new ConfiguratorEntities();
        }

        public SpecialDatasController(IOrchardServices services, ConfiguratorEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: SpecialDatas
        public ActionResult Index(int? SpecialConfigId)
        {

            GenerateDropDowns();

            if (SpecialConfigId == 0)
            {
                return View();
            }
            else
            {
                ViewBag.SCId = SpecialConfigId;

                var specialDatas = db.SpecialDatas.AsQueryable();
                if (SpecialConfigId != 0) specialDatas = specialDatas.Where(x => x.SpecialConfigId == SpecialConfigId);

                return View(specialDatas.OrderBy(x => x.SpecialDataTypeId).ThenBy(x => x.Part));
            }
        }

        // GET: SpecialDatas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpecialData specialData = db.SpecialDatas.Find(id);
            if (specialData == null)
            {
                return HttpNotFound();
            }
            return View(specialData);
        }

        // GET: SpecialDatas/Create
        public ActionResult Create(int? SpecialConfigId)
        {
            SpecialData specialData = db.SpecialDatas.Find(SpecialConfigId);
            GenerateDropDowns();
            return View(specialData);
        }

        // POST: SpecialDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] SpecialData specialData)
        {
            //prevents duplicates when creating a new customer
            var datas = db.SpecialDatas.FirstOrDefault(x => x.Part == specialData.Part);

            //displays if previous code found a duplicate
            if (datas != null) ModelState.AddModelError("", "Duplicate Part Created---Please Check Data");

            if (ModelState.IsValid)
            {
                db.SpecialDatas.Add(specialData);
                db.SaveChanges();

                return RedirectToAction("Index", new { SpecialConfigId = specialData.SpecialConfigId });
            }

            GenerateDropDowns(specialData);
            return View(specialData);
        }

        // GET: SpecialDatas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpecialData specialData = db.SpecialDatas.Find(id);
            if (specialData == null)
            {
                return HttpNotFound();
            }

            GenerateDropDowns(specialData);
            return View(specialData);
        }

        // POST: SpecialDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,SpecialConfigId,SpecialDataTypeId,Part,Quantity,Price,RelatedOpId")] SpecialData specialData)
        {
            //prevents a duplicate from being saved when editing
            var Datas = db.SpecialDatas.FirstOrDefault(x => x.SpecialConfigId == specialData.SpecialConfigId && x.SpecialDataTypeId == specialData.SpecialDataTypeId && x.Part == specialData.Part
                && x.Quantity == specialData.Quantity && x.Price == specialData.Price && x.Id != specialData.Id);

            //displays if previous code found a duplicate
            if (Datas != null) ModelState.AddModelError("", "Duplicate Lookup Created---Please Recheck Data");

            if (ModelState.IsValid)
            {
                db.Entry(specialData).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { SpecialConfigId = specialData.SpecialConfigId });
            }

            GenerateDropDowns(specialData);
            return View(specialData);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////Import///////////////////////////////////////////////////////////////////////////////////////////////////////////
        // GET: /Import
        // With this method the user can paste a list of Special Data and import it in
        public ActionResult Import()
        {
            SpecialDataImportViewModel spDataImVM = new SpecialDataImportViewModel();
            GenerateDropDowns();
            return View(spDataImVM);
        }

        // POST: /SpecialData/Import
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(SpecialDataImportViewModel spDataImVM)
        {
           // noOp used to insert a null into the Related Op Id for Assemblies
           int? noOp = null;

            if (ModelState.IsValid)
            {
                SplitSpecialData(spDataImVM.ImportData); // Calling the method to split the string from the view

                //for loop that goes through and inputs each piece of data
                for (int i = 0; i < part.Count(); i++)
                {
                        SpecialData newSpecialData = new SpecialData
                        {
                            Part = part[i],
                            Quantity = quantity[i],
                            Price = price[i],
                            SpecialConfigId = spDataImVM.SpecialConfigId,
                            SpecialDataTypeId = datatype[i],
                            RelatedOpId = (relatedopEmpty) ? noOp : relatedop[i]
                        };
                        //checking for duplicates, if inserts new record, else updates price and quantity of existing record
                        var spData = db.SpecialDatas.FirstOrDefault(x => x.SpecialConfigId == newSpecialData.SpecialConfigId && x.SpecialDataTypeId == newSpecialData.SpecialDataTypeId && x.Part == newSpecialData.Part && x.Id != newSpecialData.Id);
                        if (spData == null)
                        {
                            db.SpecialDatas.Add(newSpecialData);
                        }
                        else
                        {//posting the updates to the quantity and price if the var above finds a match
                            spData.Quantity = quantity[i];
                            spData.Price = price[i];
                            db.SpecialDatas.Attach(spData);
                            var entry = db.Entry(spData);
                            entry.Property(x => x.Quantity).IsModified = true;
                            entry.Property(x => x.Price).IsModified = true;
                        }
                }
                db.SaveChanges();
                return RedirectToAction("Index", new { SpecialConfigId = spDataImVM.SpecialConfigId });
            }
            GenerateDropDowns();
            return View(spDataImVM);
        }

        private void SplitSpecialData(string rawString)
        {
            //splits up the data that is being imported
            string[] newString = rawString.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            string[] testUpperBound = newString[0].Split(new String[] { "\t", "," }, StringSplitOptions.RemoveEmptyEntries);
            string[] parseString = rawString.Split(new String[] { "\t", ",", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            //arrays to store the Names, operations, and Ids
            var dataTypeId = db.SpecialDataTypes.Select(x => x.Id).ToArray();
            var dataTypeName = db.SpecialDataTypes.Select(x => x.Name).ToArray();
            var relatedOpId = db.SpecialRelatedOps.Select(x => x.Id).ToArray();
            var relatedOpName = db.SpecialRelatedOps.Select(x => x.Operation).ToArray();

            //this section if for if your part is an ASM(Assembly)
            // Looping through the parseString and assigning the values to the part, quantity, price, and datatypeid lists
            if (testUpperBound.Length == 4)
            {
                for (int i = 0; i < parseString.Count(); i += 4)
                {
                    part.Add(parseString[i]);
                }
                for (int i = 1; i < parseString.Count(); i += 4)
                {
                    quantity.Add(Convert.ToDecimal(parseString[i]));
                }
                for (int i = 2; i < parseString.Count(); i += 4)
                {
                    price.Add(Convert.ToDecimal(parseString[i]));
                }
                for (int i = 3; i < parseString.Count(); i += 4)
                {
                    string temp = parseString[i];
                    for (int j = 0; j < dataTypeName.Length; j++)
                    {
                        if (temp == dataTypeName[j])
                        {
                            datatype.Add(dataTypeId[j]);
                        }
                    }                     
                    relatedopEmpty = true;
                }
            }
            else if (testUpperBound.Length == 5)
            {
                //this section if for if your part is an MTL(Material)
                // Looping through the parseString and assigning the values to the part, quantity, price, datatypeid and relatedopid lists
                for (int i = 0; i < parseString.Count(); i += 5)
                {
                    part.Add(parseString[i]);
                }
                for (int i = 1; i < parseString.Count(); i += 5)
                {
                    quantity.Add(Convert.ToDecimal(parseString[i]));
                }
                for (int i = 2; i < parseString.Count(); i += 5)
                {
                    price.Add(Convert.ToDecimal(parseString[i]));
                }
                for (int i = 3; i < parseString.Count(); i += 5)
                {
                    string temp = parseString[i];
                    for (int j = 0; j < dataTypeName.Length; j++)
                    {
                        if (temp == dataTypeName[j])
                        {
                            datatype.Add(dataTypeId[j]);
                        }
                    }     
                }
                for (int i = 4; i < parseString.Count(); i += 5)
                {
                    string temp = parseString[i];
                    for (int j = 0; j < relatedOpName.Length; j++)
                    {
                        if (temp == relatedOpName[j])
                        {
                            relatedop.Add(relatedOpId[j]);
                        }
                    }            
                }
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////////////////////////////////////////Export///////////////////////////////////////////////////////////////////////////////////////////////////////////
        // GET: SpecialData/Export
        // This method exports a list of Special Data to an Excel sheet
        public ActionResult Export()
        {
            GenerateDropDowns();
            return View();
        }

        // POST: /SpecialData/Import
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Export(int SpecialConfigId)
        {
            if (SpecialConfigId == 0) ModelState.AddModelError("", "Please select a Special Configuration for the list.");

            if (ModelState.IsValid)
            {
                var sconfigs = _scfgSpDtExHp.GetSpecialDataForSpcConId(SpecialConfigId).Select(x => new SpecialDataViewModel
                {
                    part = x.Part,
                    quantity = x.Quantity,
                    price = x.Price,
                    specialDataTypeId = x.SpecialDataTypeId,
                    relatedOpId = x.RelatedOpId,
                }).ToList();

                return new ExporttoExcelResult(SpecialConfigId + "_Data_List", sconfigs);
            }

            GenerateDropDowns();
            return View();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // GET: SpecialDatas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpecialData specialData = db.SpecialDatas.Find(id);
            if (specialData == null)
            {
                return HttpNotFound();
            }
            return View(specialData);
        }

        // POST: SpecialDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SpecialData specialData = db.SpecialDatas.Find(id);
            db.SpecialDatas.Remove(specialData);
            db.SaveChanges();
            return RedirectToAction("Index", new { SpecialConfigId = specialData.SpecialConfigId });
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
            ViewBag.SpecialConfigId = new SelectList(db.SpecialConfigs.OrderBy(x => x.Name), "Id", "Name");
            ViewBag.SpecialDataTypeId = new SelectList(db.SpecialDataTypes, "Id", "Name");
            ViewBag.RelatedOpId = new SelectList(db.SpecialRelatedOps, "Id", "Operation");
        }

        private void GenerateDropDowns(SpecialData specialData)
        {
            ViewBag.SpecialConfigId = new SelectList(db.SpecialConfigs, "Id", "Name", specialData.SpecialConfigId);
            ViewBag.SpecialDataTypeId = new SelectList(db.SpecialDataTypes, "Id", "Name", specialData.SpecialDataTypeId);
            ViewBag.RelatedOpId = new SelectList(db.SpecialRelatedOps, "Id", "Operation", specialData.RelatedOpId);
        }

    }
}
