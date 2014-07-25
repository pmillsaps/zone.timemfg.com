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
using Time.OrderLog.EntityModels;

namespace Time.OrderLog.Controllers
{
    [Themed]
    public class HomeController : Controller
    {
        public IOrchardServices Services { get; set; }

        public Localizer T { get; set; }

        private OrderLogEntities db = new OrderLogEntities();

        public HomeController(IOrchardServices services)
        {
            Services = services;

            T = NullLocalizer.Instance;
        }

        // GET: /OrderLog/
        public ActionResult Index()
        {
            if (!Services.Authorizer.Authorize(Permissions.ViewOrders, T("Couldn't View Orders")))
                return new HttpUnauthorizedResult();
            var orders = db.Orders.Include(o => o.Dealer).Include(t => t.Territory);
            return View(orders.ToList());
        }

        // GET: /OrderLog/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: /OrderLog/Create
        public ActionResult Create()
        {
            ViewBag.DealerId = new SelectList(db.Dealers, "DealerId", "DealerName");
            return View();
        }

        // POST: /OrderLog/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderId,PO,DealerId,Date")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DealerId = new SelectList(db.Dealers, "DealerId", "DealerName", order.DealerId);
            return View(order);
        }

        // GET: /OrderLog/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.DealerId = new SelectList(db.Dealers, "DealerId", "DealerName", order.DealerId);
            return View(order);
        }

        // POST: /OrderLog/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderId,PO,DealerId,Date")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DealerId = new SelectList(db.Dealers, "DealerId", "DealerName", order.DealerId);
            return View(order);
        }

        // GET: /OrderLog/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
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
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
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