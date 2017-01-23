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
using Time.IT.Models;

namespace Time.IT.Controllers
{
    [Themed]
    [Authorize]
    public class MonitorController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: Monitor
        public ActionResult Index()
        {
            //var monitors = db.Monitors.OrderBy(x => x.Ref_Manufacturer.Name).ThenBy(x => x.SerialNo).Include(m => m.Ref_Manufacturer).Include(m => m.Ref_MonitorSizes).Include(m => m.User);
            //return View(monitors.ToList());
            return View();
        }

        // Load the data for the Index table
        public ActionResult LoadMonitors()
        {

            List<MonitorsViewModel> model = new List<MonitorsViewModel>();
            //db.Configuration.ProxyCreationEnabled = false;
            var monitors = db.Monitors.OrderBy(x => x.Ref_Manufacturer.Name).ThenBy(x => x.SerialNo).Include(m => m.Ref_Manufacturer).Include(m => m.Ref_MonitorSizes).Include(m => m.User);
            foreach (var item in monitors)
            {
                MonitorsViewModel mvm = new MonitorsViewModel();
                mvm.Id = item.Id;
                mvm.UserName = item.User.Name;
                mvm.MFRName = item.Ref_Manufacturer.Name;
                mvm.Model = (item.Model == null) ? "Model not found" : item.Model;
                mvm.SerialNo = item.SerialNo;
                mvm.AssetId = item.AssetId;
                mvm.Size = item.Ref_MonitorSizes.Size;
                mvm.PurchaseDate = item.PurchaseDate;
                mvm.PO = item.PO;
                mvm.PurchasedFrom = item.PurchasedFrom;
                mvm.Cost = item.Cost;
                mvm.Notes = item.Notes;
                model.Add(mvm);
            }
            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }

        // GET: Monitor/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Monitor monitor = db.Monitors.Find(id);
            if (monitor == null)
            {
                return HttpNotFound();
            }
            return View(monitor);
        }

        // GET: Monitor/Create
        public ActionResult Create()
        {
            GetDropDowns(new Monitor());
            return View();
        }

        // POST: Monitor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] Monitor monitor)
        {
            if (ModelState.IsValid)
            {
                db.Monitors.Add(monitor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            GetDropDowns(monitor);
            return View(monitor);
        }

        private void GetDropDowns(Monitor monitor)
        {
            ViewBag.ManufacturerId = new SelectList(db.Ref_Manufacturer.OrderBy(x => x.Name), "Id", "Name", monitor.ManufacturerId);
            ViewBag.SizeId = new SelectList(db.Ref_MonitorSizes.OrderBy(x => x.Size), "Id", "Size", monitor.SizeId);
            ViewBag.UserId = new SelectList(db.Users.OrderBy(x => x.Name), "Id", "Name", monitor.UserId);
        }

        // GET: Monitor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Monitor monitor = db.Monitors.Find(id);
            if (monitor == null)
            {
                return HttpNotFound();
            }
            GetDropDowns(monitor);
            return View(monitor);
        }

        // POST: Monitor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Monitor monitor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(monitor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GetDropDowns(monitor);
            return View(monitor);
        }

        // GET: Monitor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Monitor monitor = db.Monitors.Find(id);
            if (monitor == null)
            {
                return HttpNotFound();
            }
            return View(monitor);
        }

        // POST: Monitor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Monitor monitor = db.Monitors.Find(id);
            db.Monitors.Remove(monitor);
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

        //// The Save action handles both the Create an Edit of Monitor records
        //[HttpGet]
        //public ActionResult Save(int id)
        //{
        //    var monitor = db.Monitors.FirstOrDefault(x => x.Id == id);
        //    if (monitor == null) monitor = new Monitor();
        //    if (id > 0) GetDropDowns(monitor);
        //    else GetDropDowns(new Monitor());
        //    return View(monitor);
        //}
        //// Drop downs for the View
        //private void GetDropDowns(Monitor monitor)
        //{
        //    ViewBag.ManufacturerId = new SelectList(db.Ref_Manufacturer.OrderBy(x => x.Name), "Id", "Name", monitor.ManufacturerId);
        //    ViewBag.SizeId = new SelectList(db.Ref_MonitorSizes.OrderBy(x => x.Size), "Id", "Size", monitor.SizeId);
        //    ViewBag.UserId = new SelectList(db.Users.OrderBy(x => x.Name), "Id", "Name", monitor.UserId);
        //}

        //[HttpPost]// Save POST
        //public ActionResult Save(Monitor mtr)
        //{
        //    bool status = false;
        //    if (ModelState.IsValid)
        //    {
        //        if (mtr.Id > 0)
        //        {
        //            // Edit the Monitor
        //            var monitor = db.Monitors.FirstOrDefault(x => x.Id == mtr.Id);
        //            if (monitor != null)
        //            {
        //                monitor.UserId = mtr.UserId;
        //                monitor.ManufacturerId = mtr.ManufacturerId;
        //                monitor.Model = mtr.Model;
        //                monitor.SerialNo = mtr.SerialNo;
        //                monitor.AssetId = mtr.AssetId;
        //                monitor.SizeId = mtr.SizeId;
        //                monitor.PurchaseDate = mtr.PurchaseDate;
        //                monitor.PurchasedFrom = mtr.PurchasedFrom;
        //                monitor.PO = mtr.PO;
        //                monitor.Cost = mtr.Cost;
        //            }
        //        }
        //        else
        //        {
        //            // Save the Monitor
        //            db.Monitors.Add(mtr);
        //        }
        //        db.SaveChanges();
        //        status = true;
        //    }
        //    return new JsonResult { Data = new { status = status } };
        //}

        //// Delete Monitor
        //[HttpGet]
        //public ActionResult Delete(int id)
        //{
        //    var monitor = db.Monitors.FirstOrDefault(x => x.Id == id);
        //    if (monitor != null)
        //    {
        //        return View(monitor);
        //    }
        //    else
        //    {
        //        return HttpNotFound();
        //    }
        //}
        //// Confirm Delete Monitor
        //[HttpPost]
        //[ActionName("Delete")]
        //public ActionResult DeleteOrder(int id)
        //{
        //    bool status = false;
        //    var monitor = db.Monitors.FirstOrDefault(x => x.Id == id);
        //    if (monitor != null)
        //    {
        //        db.Monitors.Remove(monitor);
        //        db.SaveChanges();
        //        status = true;
        //    }
        //    return new JsonResult { Data = new { status = status } };
        //}

        //// Display Monitor Details
        //[HttpGet]
        //public ActionResult Details(int id)
        //{
        //    var monitor = db.Monitors.FirstOrDefault(x => x.Id == id);
        //    if (monitor != null)
        //    {
        //        return View(monitor);
        //    }
        //    else
        //    {
        //        return HttpNotFound();
        //    }
        //}
    }
}
