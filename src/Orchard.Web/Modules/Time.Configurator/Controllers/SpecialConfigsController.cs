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
using Time.Configurator.Models;
using Time.Data.EntityModels.Configurator;

namespace Time.Configurator.Controllers
{
    //sets theme and requires you to log in to go to the page
    [Themed]
    [Authorize]
    public class SpecialConfigsController : Controller
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        //private string[] tokens;

        public SpecialConfigsController(IOrchardServices services)
        {
            Services = services;
            db = new ConfiguratorEntities();
        }

        public SpecialConfigsController(IOrchardServices services, ConfiguratorEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: SpecialConfigs
        public ActionResult Index()
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to View this Page")))
                return new HttpUnauthorizedResult();

            return View(db.SpecialConfigs.OrderBy(x => x.Name).ToList());
        }

        // GET: SpecialConfigs/Details/5
        public ActionResult Details(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to View this Page")))
                return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpecialConfig specialConfig = db.SpecialConfigs.Find(id);
            if (specialConfig == null)
            {
                return HttpNotFound();
            }

            //pulls in data for the Special Data information in the Special Config details of each special config
            var specialData = db.SpecialDatas.Where(x => x.SpecialConfigId == specialConfig.Id).OrderBy(x => x.SpecialDataTypeId).ThenBy(x => x.Part).ToList();
            ViewBag.SpecialData = specialData;

            return View(specialConfig);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////Edit_Data/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //GET: /SpecialConfig/Edit_Data --- this method is being called from the Details view
        public ActionResult Edit_Data(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to View this Page")))
                return new HttpUnauthorizedResult();

            SpecialData specialData = db.SpecialDatas.Find(id);
            if (specialData == null)
            {
                return HttpNotFound();
            }
            ViewBag.SpecialConfigId = new SelectList(db.SpecialConfigs, "Id", "Name", specialData.SpecialConfigId);
            ViewBag.SpecialDataTypeId = new SelectList(db.SpecialDataTypes, "Id", "Name", specialData.SpecialDataTypeId);
            ViewBag.RelatedOpId = new SelectList(db.SpecialRelatedOps, "Id", "Operation", specialData.RelatedOpId);
            return View(specialData);
        }

        // POST: /Structure/Edit_Seq
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Data(SpecialData specialData)
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to View this Page")))
                return new HttpUnauthorizedResult();

            //prevents duplicate data from being saved while editing
            var Datas = db.SpecialDatas.FirstOrDefault(x => x.SpecialConfigId == specialData.SpecialConfigId && x.SpecialDataTypeId == specialData.SpecialDataTypeId && x.Part == specialData.Part && x.Quantity == specialData.Quantity
                && x.Price == specialData.Price && x.Id != specialData.Id);

            //displays if previous code found a duplicate
            if (Datas != null) ModelState.AddModelError("", "Duplicate Special Data Created---Please Check Data");

            if (ModelState.IsValid)
            {
                db.Entry(specialData).State = EntityState.Modified;
                db.SaveChanges();
                var specialConfig = db.SpecialConfigs.First(x => x.Id == specialData.SpecialConfigId);
                return RedirectToAction("Details", new { id = specialConfig.Id });
            }
            ViewBag.SpecialConfigId = new SelectList(db.SpecialConfigs, "Id", "Name", specialData.SpecialConfigId);
            ViewBag.SpecialDataTypeId = new SelectList(db.SpecialDataTypes, "Id", "Name", specialData.SpecialDataTypeId);
            ViewBag.RelatedOpId = new SelectList(db.SpecialRelatedOps, "Id", "Operation", specialData.RelatedOpId);
            return View(specialData);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////Add_Data///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //GET: /SpecialConfig/Add_Data --- this method is being called from the Details view
        public ActionResult Add_Data(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to View this Page")))
                return new HttpUnauthorizedResult();

            SpecialData specialData = db.SpecialDatas.Find(id);
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpecialConfig specialConfig = db.SpecialConfigs.Find(id);
            if (specialConfig == null)
            {
                return HttpNotFound();
            }
            SpecialConfigViewModel specialConfigVM = new SpecialConfigViewModel();

            ViewBag.SpecialConfigId = new SelectList(db.SpecialConfigs, "Id", "Name", specialConfig.Id);
            ViewBag.SpecialDataTypeId = new SelectList(db.SpecialDataTypes, "Id", "Name", specialData.SpecialDataTypeId);
            ViewBag.RelatedOpId = new SelectList(db.SpecialRelatedOps, "Id", "Operation");

            return View(specialConfigVM);
        }

        // POST: /Structure/Add_Seq
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add_Data([Bind(Exclude = "Id")] SpecialConfigViewModel specialConfigVM)
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to View this Page")))
                return new HttpUnauthorizedResult();

            //prevents duplicate code when adding special data
            var Datas = db.SpecialDatas.FirstOrDefault(x => x.SpecialConfigId == specialConfigVM.SpecialConfigId && x.SpecialDataTypeId == specialConfigVM.SpecialDataTypeId && x.Part == specialConfigVM.Part && x.Quantity == (decimal)specialConfigVM.Quantity
                && x.Price == specialConfigVM.Price);

            //displays if previous code found a duplicate
            if (Datas != null) ModelState.AddModelError("", "Duplicate Special Data Created---Please Check Data");

            if (ModelState.IsValid)
            {
                SpecialData specialDataNew = new SpecialData
                {
                    SpecialConfigId = specialConfigVM.SpecialConfigId,
                    SpecialDataTypeId = specialConfigVM.SpecialDataTypeId,
                    Part = specialConfigVM.Part,
                    Quantity = (decimal)specialConfigVM.Quantity,
                    Price = specialConfigVM.Price,
                    RelatedOpId = specialConfigVM.RelatedOpId
                };
                db.SpecialDatas.Add(specialDataNew);
                db.SaveChanges();
                var specialConfig = db.SpecialConfigs.First(x => x.Id == specialConfigVM.SpecialConfigId);
                return RedirectToAction("Details", new { id = specialConfig.Id });
            }

            ViewBag.SpecialConfigId = new SelectList(db.SpecialConfigs, "Id", "Name");
            ViewBag.SpecialDataTypeId = new SelectList(db.SpecialDataTypes, "Id", "Name");
            ViewBag.RelatedOpId = new SelectList(db.SpecialRelatedOps, "Id", "Operation");
            return View(specialConfigVM);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //////////////////////////////////////////////////////////////////////////////////////////Copy_Data///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // GET: SpecialConfigs/Copy_Data
        public ActionResult Copy_Data(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to View this Page")))
                return new HttpUnauthorizedResult();

            SpecialConfig specialConfig = db.SpecialConfigs.Find(id);

            if (specialConfig == null)
            {
                return HttpNotFound();
            }

            ViewBag.SConfigNamesTo = new SelectList(db.SpecialConfigs.OrderBy(x => x.Name), "Id", "Name");

            //Storing the Special Data in ViewBag
            var specialData = db.SpecialDatas.Where(x => x.SpecialConfigId == specialConfig.Id).OrderBy(x => x.SpecialDataTypeId).ThenBy(x => x.Part).ToList();
            ViewBag.SpecialData = specialData;

            return View(specialConfig);
        }

        // POST: SpecialConfigs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Copy_Data(SpecialData specialData, int SConfigNamesTo, SpecialConfig specialConfig)
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to View this Page")))
                return new HttpUnauthorizedResult();

            var SConfigCheck = db.SpecialDatas.FirstOrDefault(x => x.SpecialConfigId == specialConfig.Id);
            var CopyCheck = db.SpecialDatas.FirstOrDefault(x => x.SpecialConfigId == SConfigNamesTo);

            // Alerting user if no item was selected in the drop down list
            if (SConfigNamesTo == 0 || SConfigNamesTo == null) ModelState.AddModelError("", "Select a Special Configuration from the list above.");

            if (SConfigNamesTo == SConfigCheck.SpecialConfigId) ModelState.AddModelError("", "Attempting to Duplicate to the Same Configuration. Please Make a Different Selection.");

            if (CopyCheck != null) ModelState.AddModelError("", "Attempting to Copy Data to a Configuration that already has Data.  Copy to an Empty Configuration.");

            var SConfigStrct = db.SpecialDatas.Where(x => x.SpecialConfigId == specialConfig.Id).ToList();

            if (ModelState.IsValid)
            {
                foreach (var item in SConfigStrct)
                {
                    var scDatas = db.SpecialDatas.FirstOrDefault(x => x.SpecialConfigId == specialConfig.Id && x.Part == item.Part && x.Quantity == item.Quantity && x.Price == item.Price);

                    if (scDatas == null)
                    {
                    }
                    else
                    {
                        // Inserting the data in the Special Data table
                        SpecialData specialDataNew = new SpecialData
                            {
                                Part = item.Part,
                                Quantity = item.Quantity,
                                Price = item.Price,
                                SpecialConfigId = SConfigNamesTo,
                                SpecialDataTypeId = item.SpecialDataTypeId,
                                RelatedOpId = item.RelatedOpId
                            };
                        db.SpecialDatas.Add(specialDataNew);
                    }
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            var ddlSConfigName = db.SpecialConfigs.Select(x => x.Name).Distinct();
            List<SelectListItem> sconfigNames = new List<SelectListItem>();
            foreach (var item in ddlSConfigName)
            {
                sconfigNames.Add(new SelectListItem { Text = item, Value = item });
            }
            ViewBag.SConfigNamesTo = sconfigNames;

            //Storing the Special Data in ViewBag
            var spcData = db.SpecialDatas.Where(x => x.SpecialConfigId == specialConfig.Id).ToList();
            ViewBag.SpecialData = spcData;

            return View(specialConfig);

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////Create/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // GET: SpecialConfigs/Create
        public ActionResult Create()
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to View this Page")))
                return new HttpUnauthorizedResult();

            ViewBag.SpecialCustomerId = new SelectList(db.SpecialCustomers.OrderBy(x => x.Name), "Id", "Name");
            return View();
        }

        // POST: SpecialConfigs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] SpecialConfig specialConfig)
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to View this Page")))
                return new HttpUnauthorizedResult();

            //prevent duplicates when creating
            var Configs = db.SpecialConfigs.FirstOrDefault(x => x.Name == specialConfig.Name);

            //displays if previous code found a duplicate
            if (Configs != null) ModelState.AddModelError("", "Duplicate Config Created---Please Check Data");

            if (ModelState.IsValid)
            {
                db.SpecialConfigs.Add(specialConfig);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SpecialCustomerId = new SelectList(db.SpecialCustomers, "Id", "Name", specialConfig.SpecialCustomerId);
            return View(specialConfig);
        }

        // GET: SpecialConfigs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to View this Page")))
                return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpecialConfig specialConfig = db.SpecialConfigs.Find(id);
            if (specialConfig == null)
            {
                return HttpNotFound();
            }
            ViewBag.SpecialCustomerId = new SelectList(db.SpecialCustomers, "Id", "Name", specialConfig.SpecialCustomerId);
            return View(specialConfig);
        }

        // POST: SpecialConfigs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SpecialConfig specialConfig)
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to View this Page")))
                return new HttpUnauthorizedResult();

            //prevents data from being duplicated when editing
            var sConfigs = db.SpecialConfigs.FirstOrDefault(x => x.Name == specialConfig.Name && x.SpecialCustomerId == specialConfig.SpecialCustomerId && x.Id != specialConfig.Id);

            //displays if previous code found a duplicate
            if (sConfigs != null) ModelState.AddModelError("", "Duplicate Special Config Created---Please Check Data");

            if (ModelState.IsValid)
            {
                db.Entry(specialConfig).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SpecialCustomerId = new SelectList(db.SpecialCustomers, "Id", "Name", specialConfig.SpecialCustomerId);
            return View(specialConfig);
        }

        // GET: SpecialConfigs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to View this Page")))
                return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpecialConfig specialConfig = db.SpecialConfigs.Find(id);
            if (specialConfig == null)
            {
                return HttpNotFound();
            }
            return View(specialConfig);
        }

        // POST: SpecialConfigs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to View this Page")))
                return new HttpUnauthorizedResult();

            SpecialConfig specialConfig = db.SpecialConfigs.Find(id);
            int? ConfigId = id;
            var specialConfigId = db.SpecialDatas.Where(x => x.SpecialConfigId == ConfigId).ToList();

            foreach (var item in specialConfigId)
            {
                var spDelete = db.SpecialDatas.FirstOrDefault(x => x.SpecialConfigId == specialConfig.Id);
                if (spDelete != null)
                {
                    db.SpecialDatas.Remove(spDelete);
                    db.SaveChanges();
                }
            }
            db.SpecialConfigs.Remove(specialConfig);
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