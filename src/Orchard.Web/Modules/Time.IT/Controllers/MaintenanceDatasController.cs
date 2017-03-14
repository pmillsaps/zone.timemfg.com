using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.ITInventory;
using Time.IT.ViewModel;

namespace Time.IT.Controllers
{
    [Themed]
    [Authorize]
    public class MaintenanceDatasController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: MaintenanceDatas
        public ActionResult Index()
        {
            return View();
        }

        // Load the data for the Index table
        public ActionResult LoadMaintenanceData()
        {
            List<MaintenanceDataViewModel> model = new List<MaintenanceDataViewModel>();
            var MaintData = db.MaintenanceDatas;
            foreach (var item in MaintData)
            {
                MaintenanceDataViewModel mvm = new MaintenanceDataViewModel();
                mvm.Id = item.Id;
                mvm.CompanyName = item.MaintDataCompany.CompanyName;
                mvm.BudgetItem = item.BudgetItem;
                mvm.Supplier = item.Supplier;
                mvm.AccountNumber = item.AccountNumber;
                mvm.OriginalPurchDate = (item.OriginalPurchDate == null) ? "" : item.OriginalPurchDate.Value.ToShortDateString();
                var comp = db.Computers.FirstOrDefault(x => x.Id == item.ComputerId);
                mvm.ComputerName = (comp == null) ? "" : comp.Name;
                var license = db.Licenses.FirstOrDefault(x => x.Id == item.LicenseId);
                mvm.LicenseName = (license == null) ? "" : license.Name;
                var expDate = db.MaintenanceDataDetails.Where(x => x.MaintenanceDataId == item.Id).OrderByDescending(x => x.ExpirationDate).FirstOrDefault();
                mvm.ExpirationDate = (expDate != null) ? expDate.ExpirationDate.ToShortDateString() : "";
                model.Add(mvm);
            }
            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }

        // GET: MaintenanceDatas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaintenanceData maintenanceData = db.MaintenanceDatas.Find(id);
            if (maintenanceData == null)
            {
                return HttpNotFound();
            }
            ViewBag.History = db.MaintenanceDataDetails.Where(x => x.MaintenanceDataId == id).ToList();
            ViewBag.Computer = db.Computers.FirstOrDefault(x => x.Id == maintenanceData.ComputerId);
            ViewBag.License = db.Licenses.FirstOrDefault(x => x.Id == maintenanceData.LicenseId);
            return View(maintenanceData);
        }

        // GET: MaintenanceDatas/Create
        public ActionResult Create()
        {
            //ViewBag.NoModal = false;
            ViewBag.Duration = DurationYears("");
            ViewBag.ComputerId = new SelectList(db.Computers.OrderBy(x => x.Name), "Id", "Name");
            ViewBag.LicenseId = new SelectList(db.Licenses.OrderBy(x => x.Name), "Id", "Name");
            ViewBag.CompanyId = new SelectList(db.MaintDataCompanies.OrderBy(x => x.CompanyName), "Id", "CompanyName");
            return View();
            //return PartialView("_Create");
        }

        // POST: MaintenanceDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MaintenaceDataPlusDetailVM vm)
        {
            //bool status = false;
            if (ModelState.IsValid)
            {
                // Adding the Maintenance Data record
                db.MaintenanceDatas.Add(vm.MntcData);
                db.SaveChanges();
                // Adding the Maintenance Data Detail record
                var mntdId = db.MaintenanceDatas.OrderByDescending(x => x.Id).FirstOrDefault();
                vm.MntcDataDetail.MaintenanceDataId = mntdId.Id;
                db.MaintenanceDataDetails.Add(vm.MntcDataDetail);
                db.SaveChanges();
                //status = true;
                //return new JsonResult { Data = new { status = status } };
                return RedirectToAction("Index");
            }
            //ViewBag.NoModal = true;
            ViewBag.Duration = DurationYears(vm.MntcDataDetail.Duration);
            ViewBag.ComputerId = new SelectList(db.Computers.OrderBy(x => x.Name), "Id", "Name", vm.MntcData.ComputerId);
            ViewBag.LicenseId = new SelectList(db.Licenses.OrderBy(x => x.Name), "Id", "Name", vm.MntcData.LicenseId);
            ViewBag.CompanyId = new SelectList(db.MaintDataCompanies.OrderBy(x => x.CompanyName), "Id", "CompanyName");
            //return View(maintenanceData);
            //return new JsonResult { Data = new { status = status } };
            //return PartialView("_Create");
            return View();
        }

        // GET: MaintenanceDatas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaintenanceData maintenanceData = db.MaintenanceDatas.Find(id);
            if (maintenanceData == null)
            {
                return HttpNotFound();
            }
            var mntcDetails = db.MaintenanceDataDetails.Where(x => x.MaintenanceDataId == maintenanceData.Id).OrderByDescending(x => x.ExpirationDate).FirstOrDefault();
            var expDate = (mntcDetails != null) ? mntcDetails.ExpirationDate : DateTime.Now;
            var mntcDtlsId = (mntcDetails != null) ? mntcDetails.Id : 0;
            EditMaintenanceDataViewModel edtMntc = new EditMaintenanceDataViewModel
            {
                Id = maintenanceData.Id,
                MntcDataDtlsId = mntcDtlsId,
                CompanyId = maintenanceData.CompanyId,
                BudgetItem = maintenanceData.BudgetItem,
                Supplier = maintenanceData.Supplier,
                AccountNumber = maintenanceData.AccountNumber,
                OriginalPurchDate = maintenanceData.OriginalPurchDate,
                ExpirationDate = expDate,
                ComputerId = maintenanceData.ComputerId,
                LicenseId = maintenanceData.LicenseId
            };
            ViewBag.ComputerId = new SelectList(db.Computers.OrderBy(x => x.Name), "Id", "Name", maintenanceData.ComputerId);
            ViewBag.LicenseId = new SelectList(db.Licenses.OrderBy(x => x.Name), "Id", "Name", maintenanceData.LicenseId);
            ViewBag.CompanyId = new SelectList(db.MaintDataCompanies.OrderBy(x => x.CompanyName), "Id", "CompanyName", maintenanceData.CompanyId);
            //return PartialView("_Edit", edtMntc);
            return View(edtMntc);
        }

        // POST: MaintenanceDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditMaintenanceDataViewModel edtMntc)
        {
            //bool status = false;
            if (ModelState.IsValid)
            {
                // Assigning the values for MaintenanceDatas
                var mntncData = db.MaintenanceDatas.Find(edtMntc.Id);
                mntncData.CompanyId = edtMntc.CompanyId;
                mntncData.BudgetItem = edtMntc.BudgetItem;
                mntncData.Supplier = edtMntc.Supplier;
                mntncData.AccountNumber = edtMntc.AccountNumber;
                mntncData.OriginalPurchDate = edtMntc.OriginalPurchDate;
                mntncData.ComputerId = edtMntc.ComputerId;
                mntncData.LicenseId = edtMntc.LicenseId;
                // Modifying values in the db
                db.MaintenanceDatas.Attach(mntncData);// MaintenanceDatas
                var entry = db.Entry(mntncData);
                entry.Property(e => e.CompanyId).IsModified = true;
                entry.Property(e => e.BudgetItem).IsModified = true;
                entry.Property(e => e.Supplier).IsModified = true;
                entry.Property(e => e.AccountNumber).IsModified = true;
                entry.Property(e => e.OriginalPurchDate).IsModified = true;
                entry.Property(e => e.ComputerId).IsModified = true;
                entry.Property(e => e.LicenseId).IsModified = true;

                // Assigning the values for MaintenanceDataDetails
                if (edtMntc.MntcDataDtlsId > 0)
                {
                    var mntncDtls = db.MaintenanceDataDetails.Find(edtMntc.MntcDataDtlsId);
                    mntncDtls.ExpirationDate = edtMntc.ExpirationDate.Value;
                    db.MaintenanceDataDetails.Attach(mntncDtls);
                    var entry2 = db.Entry(mntncDtls);// MaintenanceDataDetails
                    entry2.Property(e => e.ExpirationDate).IsModified = true;
                }

                db.SaveChanges();
                //status = true;
                return RedirectToAction("Index");
            }
            ViewBag.ComputerId = new SelectList(db.Computers.OrderBy(x => x.Name), "Id", "Name", edtMntc.ComputerId);
            ViewBag.LicenseId = new SelectList(db.Licenses.OrderBy(x => x.Name), "Id", "Name", edtMntc.LicenseId);
            ViewBag.CompanyId = new SelectList(db.MaintDataCompanies.OrderBy(x => x.CompanyName), "Id", "CompanyName", edtMntc.CompanyId);
            //return new JsonResult { Data = new { status = status } };
            return View(edtMntc);
        }

        // GET: MaintenanceDatas/CopyRecord/5
        public ActionResult CopyRecord(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaintenanceData maintData = db.MaintenanceDatas.Find(id);
            if (maintData == null)
            {
                return HttpNotFound();
            }
            var mntcDetails = db.MaintenanceDataDetails.Where(x => x.MaintenanceDataId == maintData.Id).OrderByDescending(x => x.ExpirationDate).FirstOrDefault();
            MaintenaceDataPlusDetailVM vm = new MaintenaceDataPlusDetailVM();
            vm.MntcData = maintData;
            vm.MntcDataDetail = mntcDetails;
            ViewBag.Duration = DurationYears(mntcDetails.Duration);
            ViewBag.ComputerId = new SelectList(db.Computers.OrderBy(x => x.Name), "Id", "Name", maintData.ComputerId);
            ViewBag.LicenseId = new SelectList(db.Licenses.OrderBy(x => x.Name), "Id", "Name", maintData.LicenseId);
            ViewBag.CompanyId = new SelectList(db.MaintDataCompanies.OrderBy(x => x.CompanyName), "Id", "CompanyName", maintData.CompanyId);
            return View(vm);
        }

        // POST: MaintenanceDatas/CopyRecord
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CopyRecord(MaintenaceDataPlusDetailVM vm)
        {
            if (ModelState.IsValid)
            {
                // Adding the Maintenance Data record
                var mntncData = new MaintenanceData
                {
                    BudgetItem = vm.MntcData.BudgetItem,
                    Supplier = vm.MntcData.Supplier,
                    AccountNumber = vm.MntcData.AccountNumber,
                    OriginalPurchDate = vm.MntcData.OriginalPurchDate,
                    CompanyId = vm.MntcData.CompanyId,
                    ComputerId = vm.MntcData.ComputerId,
                    LicenseId = vm.MntcData.LicenseId
                };
                db.MaintenanceDatas.Add(mntncData);
                db.SaveChanges();
                // Adding the Maintenance Data Detail record
                var mntdId = db.MaintenanceDatas.OrderByDescending(x => x.Id).FirstOrDefault();
                //vm.MntcDataDetail.MaintenanceDataId = mntdId.Id;
                var mntncDataDetail = new MaintenanceDataDetail
                {
                    MaintenanceDataId = mntdId.Id,
                    PurchaseDate = vm.MntcDataDetail.PurchaseDate,
                    ExpirationDate = vm.MntcDataDetail.ExpirationDate,
                    AccountNumber = vm.MntcDataDetail.AccountNumber,
                    Duration = vm.MntcDataDetail.Duration,
                    Cost = vm.MntcDataDetail.Cost,
                    Monthly = vm.MntcDataDetail.Monthly,
                    Explanation = vm.MntcDataDetail.Explanation,
                    AlternateInfo = vm.MntcDataDetail.AlternateInfo,
                    PO_CC = vm.MntcDataDetail.PO_CC
                };
                db.MaintenanceDataDetails.Add(mntncDataDetail);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            ViewBag.Duration = DurationYears(vm.MntcDataDetail.Duration);
            ViewBag.ComputerId = new SelectList(db.Computers.OrderBy(x => x.Name), "Id", "Name", vm.MntcData.ComputerId);
            ViewBag.LicenseId = new SelectList(db.Licenses.OrderBy(x => x.Name), "Id", "Name", vm.MntcData.LicenseId);
            ViewBag.CompanyId = new SelectList(db.MaintDataCompanies.OrderBy(x => x.CompanyName), "Id", "CompanyName");
            return View();
        }

        // GET: MaintenanceDatas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaintenanceData maintenanceData = db.MaintenanceDatas.Find(id);
            if (maintenanceData == null)
            {
                return HttpNotFound();
            }
            ViewBag.Computer = db.Computers.FirstOrDefault(x => x.Id == maintenanceData.ComputerId);
            ViewBag.License = db.Licenses.FirstOrDefault(x => x.Id == maintenanceData.LicenseId);
            return View(maintenanceData);
        }

        // POST: MaintenanceDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MaintenanceData maintenanceData = db.MaintenanceDatas.Find(id);
            db.MaintenanceDatas.Remove(maintenanceData);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Loads the years for the Create page
        public SelectList DurationYears(string yearSelected)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 1; i < 21; i++)
            {
                if (i < 10)
                {
                    items.Add(new SelectListItem() { Text = i + " Year", Value = i + " Year", Selected = false });
                }
                else
                {
                    items.Add(new SelectListItem() { Text = i + " Years", Value = i + " Years", Selected = false });
                }
            }
            return new SelectList(items, "Value", "Text", yearSelected);

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
