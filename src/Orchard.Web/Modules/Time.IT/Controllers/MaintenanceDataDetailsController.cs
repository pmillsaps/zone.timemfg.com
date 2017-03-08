﻿using Orchard.Themes;
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
            ViewBag.Expiration = new List<SelectListItem>
            {
                new SelectListItem {Text="Non-Expired", Value= "Non-Expired", Selected=true },
                new SelectListItem {Text="Expired", Value= "Expired", Selected=false }
            };
            return View();
        }

        // Load the data for the Index table
        public ActionResult LoadMaintenanceDataDetails(string licenseStatus)
        {
            List<MaintenanceDataDetailViewModel> model = new List<MaintenanceDataDetailViewModel>();
            var maintData = db.MaintenanceDataDetails.Include(m => m.MaintenanceData).AsQueryable();
            if (licenseStatus == "Non-Expired")
            {
                maintData = maintData.Where(x => x.ExpirationDate > DateTime.Now);
            }
            else
            {
                maintData = maintData.Where(x => x.ExpirationDate < DateTime.Now);
            }
            foreach (var item in maintData)
            {
                MaintenanceDataDetailViewModel mvm = new MaintenanceDataDetailViewModel();
                mvm.Id = item.Id;
                mvm.CompanyName = item.MaintenanceData.MaintDataCompany.CompanyName;
                mvm.BudgetItem = item.MaintenanceData.BudgetItem;
                mvm.Supplier = item.MaintenanceData.Supplier;
                mvm.OriginalPurchDate = (item.MaintenanceData.OriginalPurchDate != null) ? item.MaintenanceData.OriginalPurchDate.Value.ToShortDateString() : "";
                mvm.PurchaseDate = (item.PurchaseDate != null) ? item.PurchaseDate.ToShortDateString() : "";
                mvm.ExpirationDate = (item.ExpirationDate != null) ? item.ExpirationDate.ToShortDateString() : "";
                mvm.AccountNumber = item.AccountNumber;
                mvm.Duration = item.Duration;
                mvm.Cost = item.Cost.ToString();
                mvm.Monthly = item.Monthly;
                mvm.Explanation = item.Explanation;
                mvm.AlternateInfo = item.AlternateInfo;
                mvm.PO_CC = item.PO_CC;
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
            ViewBag.History = db.MaintenanceDataDetails.Where(x => x.MaintenanceDataId == maintenanceDataDetail.MaintenanceDataId).ToList();
            var maintenanceData = db.MaintenanceDatas.FirstOrDefault(x => x.Id == maintenanceDataDetail.MaintenanceDataId);
            if(maintenanceData.ComputerId != null)
                ViewBag.Computer = db.Computers.FirstOrDefault(x => x.Id == maintenanceData.ComputerId);
            if (maintenanceData.LicenseId != null)
                ViewBag.License = db.Licenses.FirstOrDefault(x => x.Id == maintenanceData.LicenseId);
            return View(maintenanceDataDetail);
        }

        // GET: MaintenanceDataDetails/Create
        public ActionResult Create()
        {
            //ViewBag.NoModal = false;
            ViewBag.Duration = DurationYears("");
            ViewBag.MaintenanceDataId = new SelectList(db.MaintenanceDatas, "Id", "BudgetItem");
            //return PartialView("_Create");
            return View();
        }

        // POST: MaintenanceDataDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] MaintenanceDataDetail maintenanceDataDetail)
        {
            //bool status = false;
            if (ModelState.IsValid)
            {
                db.MaintenanceDataDetails.Add(maintenanceDataDetail);
                db.SaveChanges();
                //status = true;
                return RedirectToAction("Index");
            }
            ViewBag.Duration = DurationYears(maintenanceDataDetail.Duration);
            //ViewBag.NoModal = true;
            ViewBag.MaintenanceDataId = new SelectList(db.MaintenanceDatas, "Id", "BudgetItem", maintenanceDataDetail.MaintenanceDataId);
            return View(maintenanceDataDetail);
            //return new JsonResult { Data = new { status = status } };
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
            //ViewBag.NoModal = false;
            ViewBag.Duration = DurationYears(maintenanceDataDetail.Duration);
            ViewBag.MaintenanceDataId = new SelectList(db.MaintenanceDatas, "Id", "BudgetItem", maintenanceDataDetail.MaintenanceDataId);
            //return PartialView("_Edit", maintenanceDataDetail);
            return View(maintenanceDataDetail);
        }

        // POST: MaintenanceDataDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MaintenanceDataDetail maintenanceDataDetail)
        {
            //bool status = false;
            if (ModelState.IsValid)
            {
                db.Entry(maintenanceDataDetail).State = EntityState.Modified;
                db.SaveChanges();
                //status = true;
                return RedirectToAction("Index");
            }
            //ViewBag.NoModal = true;
            ViewBag.Duration = DurationYears(maintenanceDataDetail.Duration);
            ViewBag.MaintenanceDataId = new SelectList(db.MaintenanceDatas, "Id", "BudgetItem", maintenanceDataDetail.MaintenanceDataId);
            return View(maintenanceDataDetail);
            //return new JsonResult { Data = new { status = status } };
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

        // Loads the years for the Create and Edit pages
        public SelectList DurationYears(string yearSelected)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 1; i < 21; i++)
            {
                if(i < 10){
                    items.Add(new SelectListItem() { Text = i + " Year", Value = i + " Year", Selected = false });
                }
                else{
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
