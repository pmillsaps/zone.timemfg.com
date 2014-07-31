using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Time.OrderLog.EntityModels;

namespace Time.OrderLog.Controllers
{
    public class TEMP_OrderLineController : Controller
    {
        private OrderLogEntities db = new OrderLogEntities();

        // GET: /TEMP_OrderLine/
        public ActionResult Index()
        {
            var orderlines = db.OrderLines.Include(o => o.Install).Include(o => o.LiftModel).Include(o => o.Order);
            return View(orderlines.ToList());
        }

        // GET: /TEMP_OrderLine/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderLine orderline = db.OrderLines.Find(id);
            if (orderline == null)
            {
                return HttpNotFound();
            }
            return View(orderline);
        }

        // GET: /TEMP_OrderLine/Create
        public ActionResult Create()
        {
            ViewBag.InstallId = new SelectList(db.Installs, "InstallId", "InstallName");
            ViewBag.LiftModelId = new SelectList(db.LiftModels, "LiftModelId", "LiftModelName");
            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "PO");
            return View();
        }

        // POST: /TEMP_OrderLine/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="OrderLineId,OrderId,LiftModelId,NewQty,CancelQty,Special,InstallId,Stock,RTG,Demo,Customer,City,State,Zip,Comment")] OrderLine orderline)
        {
            if (ModelState.IsValid)
            {
                db.OrderLines.Add(orderline);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.InstallId = new SelectList(db.Installs, "InstallId", "InstallName", orderline.InstallId);
            ViewBag.LiftModelId = new SelectList(db.LiftModels, "LiftModelId", "LiftModelName", orderline.LiftModelId);
            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "PO", orderline.OrderId);
            return View(orderline);
        }

        // GET: /TEMP_OrderLine/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderLine orderline = db.OrderLines.Find(id);
            if (orderline == null)
            {
                return HttpNotFound();
            }
            ViewBag.InstallId = new SelectList(db.Installs, "InstallId", "InstallName", orderline.InstallId);
            ViewBag.LiftModelId = new SelectList(db.LiftModels, "LiftModelId", "LiftModelName", orderline.LiftModelId);
            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "PO", orderline.OrderId);
            return View(orderline);
        }

        // POST: /TEMP_OrderLine/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="OrderLineId,OrderId,LiftModelId,NewQty,CancelQty,Special,InstallId,Stock,RTG,Demo,Customer,City,State,Zip,Comment")] OrderLine orderline)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderline).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.InstallId = new SelectList(db.Installs, "InstallId", "InstallName", orderline.InstallId);
            ViewBag.LiftModelId = new SelectList(db.LiftModels, "LiftModelId", "LiftModelName", orderline.LiftModelId);
            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "PO", orderline.OrderId);
            return View(orderline);
        }

        // GET: /TEMP_OrderLine/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderLine orderline = db.OrderLines.Find(id);
            if (orderline == null)
            {
                return HttpNotFound();
            }
            return View(orderline);
        }

        // POST: /TEMP_OrderLine/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderLine orderline = db.OrderLines.Find(id);
            db.OrderLines.Remove(orderline);
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
