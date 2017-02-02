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
            //return View(db.MaintenanceDatas.ToList());
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
                mvm.CompanyName = item.CompanyName;
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
            return View(maintenanceData);
        }

        // GET: MaintenanceDatas/Create
        public ActionResult Create()
        {
            ViewBag.NoModal = false;
            ViewBag.ComputerId = new SelectList(db.Computers.OrderBy(x => x.Name), "Id", "Name");
            ViewBag.LicenseId = new SelectList(db.Licenses.OrderBy(x => x.Name), "Id", "Name");
            return View();
        }

        // POST: MaintenanceDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] MaintenanceData maintenanceData)
        {
            if (ModelState.IsValid)
            {
                db.MaintenanceDatas.Add(maintenanceData);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.NoModal = true;
            ViewBag.ComputerId = new SelectList(db.Computers.OrderBy(x => x.Name), "Id", "Name", maintenanceData.ComputerId);
            ViewBag.LicenseId = new SelectList(db.Licenses.OrderBy(x => x.Name), "Id", "Name", maintenanceData.LicenseId);
            return View(maintenanceData);
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
            EditMaintenanceDataViewModel edtMntc = new EditMaintenanceDataViewModel
            {
                Id = maintenanceData.Id,
                MntcDataDtlsId = mntcDetails.Id,
                CompanyName = maintenanceData.CompanyName,
                BudgetItem = maintenanceData.BudgetItem,
                Supplier = maintenanceData.Supplier,
                AccountNumber = maintenanceData.AccountNumber,
                OriginalPurchDate = maintenanceData.OriginalPurchDate,
                ExpirationDate = expDate,
                ComputerId = maintenanceData.ComputerId,
                LicenseId = maintenanceData.LicenseId
            };
            ViewBag.NoModal = false;
            ViewBag.ComputerId = new SelectList(db.Computers.OrderBy(x => x.Name), "Id", "Name", maintenanceData.ComputerId);
            ViewBag.LicenseId = new SelectList(db.Licenses.OrderBy(x => x.Name), "Id", "Name", maintenanceData.LicenseId);
            return View(edtMntc);
        }

        // POST: MaintenanceDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditMaintenanceDataViewModel edtMntc)
        {
            if (ModelState.IsValid)
            {
                // Assgning the values for MaintenanceDatas
                var mntncData = db.MaintenanceDatas.Find(edtMntc.Id);
                mntncData.CompanyName = edtMntc.CompanyName;
                mntncData.BudgetItem = edtMntc.BudgetItem;
                mntncData.Supplier = edtMntc.Supplier;
                mntncData.AccountNumber = edtMntc.AccountNumber;
                mntncData.OriginalPurchDate = edtMntc.OriginalPurchDate;
                mntncData.ComputerId = edtMntc.ComputerId;
                mntncData.LicenseId = edtMntc.LicenseId;
                // Assgning the values for MaintenanceDataDetails
                var mntncDtls = db.MaintenanceDataDetails.Find(edtMntc.MntcDataDtlsId);
                mntncDtls.ExpirationDate = edtMntc.ExpirationDate.Value;
                // Modifying values in the db
                db.MaintenanceDatas.Attach(mntncData);
                var entry = db.Entry(mntncData);
                entry.Property(e => e.CompanyName).IsModified = true;
                entry.Property(e => e.BudgetItem).IsModified = true;
                entry.Property(e => e.Supplier).IsModified = true;
                entry.Property(e => e.AccountNumber).IsModified = true;
                entry.Property(e => e.OriginalPurchDate).IsModified = true;
                entry.Property(e => e.ComputerId).IsModified = true;
                entry.Property(e => e.LicenseId).IsModified = true;
                db.MaintenanceDataDetails.Attach(mntncDtls);
                var entry2 = db.Entry(mntncDtls);
                entry2.Property(e => e.ExpirationDate).IsModified = true;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.NoModal = true;
            ViewBag.ComputerId = new SelectList(db.Computers.OrderBy(x => x.Name), "Id", "Name", edtMntc.ComputerId);
            ViewBag.LicenseId = new SelectList(db.Licenses.OrderBy(x => x.Name), "Id", "Name", edtMntc.LicenseId);
            return View(edtMntc);
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
