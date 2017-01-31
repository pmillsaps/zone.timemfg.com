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
    public class MaintenanceDataDetailsController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: MaintenanceDataDetails
        public ActionResult Index()
        {
            return View();
            //var maintenanceDataDetails = db.MaintenanceDataDetails.Include(m => m.MaintenanceData);
            //return View(maintenanceDataDetails.ToList());
        }

        // Load the data for the Index table
        public ActionResult LoadMaintenanceDataDetails()
        {
            List<MaintenanceDataDetailViewModel> model = new List<MaintenanceDataDetailViewModel>();
            var MaintData = db.MaintenanceDataDetails.Include(m => m.MaintenanceData);
            foreach (var item in MaintData)
            {
                MaintenanceDataDetailViewModel mvm = new MaintenanceDataDetailViewModel();
                mvm.Id = item.Id;
                mvm.CompanyName = item.MaintenanceData.CompanyName;
                mvm.BudgetItem = item.MaintenanceData.BudgetItem;
                mvm.Supplier = item.MaintenanceData.Supplier;
                mvm.OriginalPurchDate = (item.MaintenanceData != null) ? item.MaintenanceData.OriginalPurchDate.Value.ToShortDateString() : "";
                mvm.PurchaseDate = (item.PurchaseDate != null) ? item.PurchaseDate.ToShortDateString() : "";
                mvm.ExpirationDate = (item.ExpirationDate != null) ? item.ExpirationDate.ToShortDateString() : "";
                mvm.AccountNumber = item.AccountNumber;
                mvm.Duration = item.Duration;
                mvm.Cost = item.Cost.ToString();
                mvm.Monthly = item.Monthly;
                mvm.Explanation = item.Explanation;
                mvm.AlternateInfo = item.AlternateInfo;
                model.Add(mvm);
            }
            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }

        // GET: MaintenanceDataDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaintenanceDataDetail maintenanceDataDetail = db.MaintenanceDataDetails.Find(id);
            if (maintenanceDataDetail == null)
            {
                return HttpNotFound();
            }
            return View(maintenanceDataDetail);
        }

        // GET: MaintenanceDataDetails/Create
        public ActionResult Create()
        {
            ViewBag.MaintenanceDataId = new SelectList(db.MaintenanceDatas, "Id", "BudgetItem");
            return View();
        }

        // POST: MaintenanceDataDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] MaintenanceDataDetail maintenanceDataDetail)
        {
            if (ModelState.IsValid)
            {
                db.MaintenanceDataDetails.Add(maintenanceDataDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaintenanceDataId = new SelectList(db.MaintenanceDatas, "Id", "BudgetItem", maintenanceDataDetail.MaintenanceDataId);
            return View(maintenanceDataDetail);
        }

        // GET: MaintenanceDataDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaintenanceDataDetail maintenanceDataDetail = db.MaintenanceDataDetails.Find(id);
            if (maintenanceDataDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaintenanceDataId = new SelectList(db.MaintenanceDatas, "Id", "BudgetItem", maintenanceDataDetail.MaintenanceDataId);
            return View(maintenanceDataDetail);
        }

        // POST: MaintenanceDataDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MaintenanceDataDetail maintenanceDataDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(maintenanceDataDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaintenanceDataId = new SelectList(db.MaintenanceDatas, "Id", "BudgetItem", maintenanceDataDetail.MaintenanceDataId);
            return View(maintenanceDataDetail);
        }

        // GET: MaintenanceDataDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaintenanceDataDetail maintenanceDataDetail = db.MaintenanceDataDetails.Find(id);
            if (maintenanceDataDetail == null)
            {
                return HttpNotFound();
            }
            return View(maintenanceDataDetail);
        }

        // POST: MaintenanceDataDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MaintenanceDataDetail maintenanceDataDetail = db.MaintenanceDataDetails.Find(id);
            db.MaintenanceDataDetails.Remove(maintenanceDataDetail);
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
