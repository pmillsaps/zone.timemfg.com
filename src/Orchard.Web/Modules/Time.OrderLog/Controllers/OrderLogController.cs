﻿using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Time.Data.EntityModels.OrderLog;
using Time.Epicor.Helpers;
using Time.OrderLog.Models;

namespace Time.OrderLog.Controllers
{
    [Authorize]
    [Themed]
    public class OrderLogController : Controller
    {
        public IOrchardServices Services { get; set; }

        public Localizer T { get; set; }

        private OrderLogEntities db;

        public OrderLogController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
            db = new OrderLogEntities();
            Setup();
        }

        public OrderLogController(IOrchardServices services, OrderLogEntities _db)
        {
            Services = services;
            T = NullLocalizer.Instance;
            db = _db;
            Setup();
        }

        public void Setup()
        {
            var vm = new MenuViewModel();
            if (Services.Authorizer.Authorize(Permissions.ViewOrders)) vm.ViewOrders = true;
            if (Services.Authorizer.Authorize(Permissions.EditOrders)) vm.EditOrders = true;
            if (Services.Authorizer.Authorize(Permissions.OrderLogReporting)) vm.OrderLogReporting = true;
            ViewBag.Permissions = vm;
        }

        // GET: /OrderLog/
        public ActionResult Index()
        {
            if (!Services.Authorizer.Authorize(Permissions.ViewOrders, T("You Do Not Have Permission to View Orders")))
                return new HttpUnauthorizedResult();
            var orders = db.Orders.OrderByDescending(x => x.Date).ToList();
            var yearCurrent = DateTime.Now.Year;
            var yearPrevious = DateTime.Now.AddYears(-1).Year;
            try
            {
                var ytd = db.OrderTrans.Where(x => x.AsOfDate.Value.Year == yearCurrent).Sum(x => x.NewQty) - db.OrderTrans.Where(x => x.AsOfDate.Value.Year == yearCurrent).Sum(x => x.CancelQty);
                ViewBag.YTD = ytd;
            }
            catch
            {
                ViewBag.YTD = "Error";
            }

            try
            {
                var pytd = db.OrderTrans.Where(x => x.AsOfDate.Value.Year == yearPrevious).Sum(x => x.NewQty) - db.OrderTrans.Where(x => x.AsOfDate.Value.Year == yearPrevious).Sum(x => x.CancelQty);
                ViewBag.PYTD = pytd;
            }
            catch
            {
                ViewBag.PYTD = "Error";
            }

            //var orders = db.Orders.Include(o => o.Dealer).Include(t => t.Territory).ToList();
            return View(orders);
        }

        public ActionResult Search(string search)
        {
            if (!Services.Authorizer.Authorize(Permissions.ViewOrders, T("You Do Not Have Permission to View Orders")))
                return new HttpUnauthorizedResult();
            var orders = db.Orders.Where(x => x.PO.Contains(search) || x.Customer.Contains(search) || x.Dealer.DealerName.Contains(search)).OrderByDescending(x => x.Date).ToList();
            return View("Index", orders);
        }

        // Export Order Log to Excel
        public ActionResult ExportOrderLog(DatePickerVM dpVM, string command)
        {
            if (dpVM.StartDate == null || dpVM.EndDate == null)
                return View(); // Returning an empty view if the dates are empty
            else
            {
                // Retrieving the data for the report from the different tables
                var report = db.Orders.Where(x => x.Date >= dpVM.StartDate && x.Date <= dpVM.EndDate)
                                      .Include(d => d.Dealer).Include(i => i.Install)
                                      .Include(ir => ir.Installer).Include(t => t.Territory)
                                      .Include(ot => ot.OrderTrans).ToList();

                // Generating the list of Orders
                OrderDetails o;
                List<OrderDetails> orderD = new List<OrderDetails>();
                // Generating the list of Orders Transactions
                OrderTransactions oT;
                List<OrderTransactions> orderT = new List<OrderTransactions>();
                int sum = 0;

                foreach (var item in report)
                {
                    o = new OrderDetails();

                    o.PONum = item.PO;
                    o.OrderDate = item.Date;
                    o.DealerName = item.Dealer.DealerName;
                    if (item.Install == null) o.InstallType = "";
                    else o.InstallType = item.Install.InstallName;
                    if (item.Installer == null) o.InstallerName = "";
                    else o.InstallerName = item.Installer.InstallerName;
                    foreach (var trans in item.OrderTrans)
                    {
                        oT = new OrderTransactions();

                        oT.PO = o.PONum;
                        oT.Date = trans.Date;
                        oT.AsOfDate = trans.AsOfDate;
                        var lift = db.LiftModels.FirstOrDefault(l => l.LiftModelId == trans.LiftModelId);
                        oT.LiftModel = lift.LiftModelName.ToString();
                        oT.NewQty = trans.NewQty;
                        oT.CancelQty = trans.CancelQty;
                        oT.Special = trans.Special;
                        oT.Stock = trans.Stock;
                        oT.Demo = trans.Demo;
                        oT.RTG = trans.RTG;
                        oT.TruGuard = trans.TruGuard;
                        oT.Comment = trans.Comment;

                        orderT.Add(oT);

                        sum += trans.NewQty - trans.CancelQty;
                    }
                    o.OrderQty = sum;
                    o.Special = item.Special;
                    o.Stock = item.Stock;
                    o.Demo = item.Demo;
                    o.RTG = item.RTG;
                    o.TruGuard = item.TruGuard;
                    if (item.Customer == null) o.Customer = "";
                    else o.Customer = item.Customer;
                    if (item.CityStateZip == null) o.CityStateZip = "";
                    else o.CityStateZip = item.CityStateZip;

                    sum = 0;
                    orderD.Add(o);
                }
                if (command == "Export Order Details Report")
                {
                    return new ExporttoExcelResult("OrderLogReport", orderD.Cast<object>().ToList());
                }
                else
                {
                    return new ExporttoExcelResult("OrderTransactionReport", orderT.Cast<object>().ToList());
                }
            }
        }

        // GET: /OrderLog/Details/5
        public ActionResult Details(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!Services.Authorizer.Authorize(Permissions.ViewOrders, T("You Do Not Have Permission to View Orders")))
                return new HttpUnauthorizedResult();
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            getDropDowns(order);
            return View(order);
        }

        // GET: /OrderLog/Create
        public ActionResult Create()
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit Orders")))
                return new HttpUnauthorizedResult();
            getDropDowns();
            return View();
        }

        // POST: /OrderLog/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "OrderId")] Order order)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit Orders")))
                return new HttpUnauthorizedResult();
            var found = db.Orders.FirstOrDefault(x => x.DealerId == order.DealerId && x.PO == order.PO);
            if (found != null) ModelState.AddModelError("PO", "Duplicate PO Numbers for the same customer are not allowed");
            var dealer = db.Dealers.FirstOrDefault(x => x.DealerId == order.DealerId);
            if ((order.TerritoryId == 0) && dealer != null) order.TerritoryId = dealer.TerritoryId;
            order.Date = DateTime.Now;
            if (ModelState.IsValid)
            {
                //var dealer = db.Dealers.FirstOrDefault(x => x.DealerId == order.DealerId);
                //if ((order.TerritoryId == 0) && dealer != null) order.TerritoryId = dealer.TerritoryId;
                order.Date = DateTime.Now;
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Create", "OrderTran", new { id = order.OrderId });
            }

            getDropDowns();
            return View(order);
        }

        // GET: /OrderLog/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit Orders")))
                return new HttpUnauthorizedResult();
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            getDropDowns(order);
            return View(order);
        }

        // POST: /OrderLog/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "OrderId,PO,DealerId,TerritoryID,Date")] Order order)
        public ActionResult Edit(Order order, string returnUrl)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit Orders")))
                return new HttpUnauthorizedResult();
            if (ModelState.IsValid)
            {
                var dealer = db.Dealers.FirstOrDefault(x => x.DealerId == order.DealerId);
                if (order.TerritoryId == 0 && dealer != null) order.TerritoryId = dealer.TerritoryId;
                var foo = ViewBag.returnUrl;
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = order.OrderId });
                //Redirect to Details page instead of Index -- see: http://stackoverflow.com/questions/9772947/c-sharp-asp-net-mvc-return-to-previous-page
                //return Redirect(returnUrl);
            }
            getDropDowns(order);
            return View(order);
        }

        private void getDropDowns()
        {
            ViewBag.DealerId = new SelectList(db.Dealers.OrderBy(x => x.DealerName), "DealerId", "DealerName");
            ViewBag.TerritoryId = new SelectList(db.Territories.OrderBy(x => x.TerritoryName), "TerritoryId", "TerritoryName");
            ViewBag.InstallId = new SelectList(db.Installs.OrderBy(x => x.InstallName), "InstallId", "InstallName");
            ViewBag.InstallerId = new SelectList(db.Installers.OrderBy(x => x.InstallerName), "InstallerId", "InstallerName");
            ViewBag.OrderQty = 0;
        }

        private void getDropDowns(Order order)
        {
            ViewBag.DealerId = new SelectList(db.Dealers.OrderBy(x => x.DealerName), "DealerId", "DealerName", order.DealerId);
            ViewBag.TerritoryId = new SelectList(db.Territories.OrderBy(x => x.TerritoryName), "TerritoryId", "TerritoryName", order.TerritoryId);
            ViewBag.InstallId = new SelectList(db.Installs.OrderBy(x => x.InstallName), "InstallId", "InstallName", order.InstallId);
            ViewBag.InstallerId = new SelectList(db.Installers.OrderBy(x => x.InstallerName), "InstallerId", "InstallerName", order.InstallerId);
            var qty = order.OrderTrans.Sum(x => x.NewQty) - order.OrderTrans.Sum(x => x.CancelQty);
            ViewBag.OrderQty = qty;
        }

        // GET: /OrderLog/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit Orders")))
                return new HttpUnauthorizedResult();
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: /OrderLog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit Orders")))
                return new HttpUnauthorizedResult();
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult _OrderTrans(int id = 0)
        {
            if (id != 0)
            {
                var trans = db.OrderTrans.Where(x => x.OrderId == id).Include(o => o.LiftModel).ToList();
                return PartialView(trans);
            }

            return new EmptyResult();
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