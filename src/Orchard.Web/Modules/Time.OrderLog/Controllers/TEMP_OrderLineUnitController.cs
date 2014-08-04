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
    public class TEMP_OrderLineUnitController : Controller
    {
        private OrderLogEntities db = new OrderLogEntities();

        // GET: /OrderLineUnit/
        public ActionResult Index()
        {
            var orderlineunits = db.OrderLineUnits.Include(o => o.OrderLine);
            return View(orderlineunits.ToList());
        }

        // GET: /OrderLineUnit/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderLineUnit orderlineunit = db.OrderLineUnits.Find(id);
            if (orderlineunit == null)
            {
                return HttpNotFound();
            }
            return View(orderlineunit);
        }

        // GET: /OrderLineUnit/Create
        public ActionResult Create()
        {
            ViewBag.OrderLineId = new SelectList(db.OrderLines, "OrderLineId", "Customer");
            return View();
        }

        // POST: /OrderLineUnit/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderLineUnitId,OrderLineId,ATSDate,SalesOrderNum,InvoiceDate,InvoiceAmt")] OrderLineUnit orderlineunit)
        {
            if (ModelState.IsValid)
            {
                db.OrderLineUnits.Add(orderlineunit);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OrderLineId = new SelectList(db.OrderLines, "OrderLineId", "Customer", orderlineunit.OrderLineId);
            return View(orderlineunit);
        }

        // GET: /OrderLineUnit/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderLineUnit orderlineunit = db.OrderLineUnits.Find(id);
            if (orderlineunit == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrderLineId = new SelectList(db.OrderLines, "OrderLineId", "Customer", orderlineunit.OrderLineId);
            return View(orderlineunit);
        }

        // POST: /OrderLineUnit/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderLineUnitId,OrderLineId,ATSDate,SalesOrderNum,InvoiceDate,InvoiceAmt")] OrderLineUnit orderlineunit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderlineunit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OrderLineId = new SelectList(db.OrderLines, "OrderLineId", "Customer", orderlineunit.OrderLineId);
            return View(orderlineunit);
        }

        // GET: /OrderLineUnit/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderLineUnit orderlineunit = db.OrderLineUnits.Find(id);
            if (orderlineunit == null)
            {
                return HttpNotFound();
            }
            return View(orderlineunit);
        }

        // POST: /OrderLineUnit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderLineUnit orderlineunit = db.OrderLineUnits.Find(id);
            db.OrderLineUnits.Remove(orderlineunit);
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