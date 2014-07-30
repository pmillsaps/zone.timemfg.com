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
            var orders = db.Orders.Include(o => o.Dealer).Include(t => t.Territory).ToList();
            return View(orders);
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
            ViewBag.PONum = order.PO;
            return View(order);
        }

        // GET: /OrderLog/Create
        public ActionResult Create()
        {
            getOrderDropDowns();
            return View();
        }

        // POST: /OrderLog/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderId,PO,DealerId,TerritoryID,Date")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            getOrderDropDowns();
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

            ViewBag.PONum = order.PO;
            getOrderDropDowns(order);
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
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = order.OrderId });
            }

            return View(order);
        }

        private void getOrderDropDowns()
        {
            ViewBag.DealerId = new SelectList(db.Dealers, "DealerId", "DealerName");
            ViewBag.TerritoryId = new SelectList(db.Territories, "TerritoryId", "TerritoryName");
        }

        private void getOrderDropDowns(Order order)
        {
            ViewBag.DealerId = new SelectList(db.Dealers, "DealerId", "DealerName", order.DealerId);
            ViewBag.TerritoryId = new SelectList(db.Territories, "TerritoryId", "TerritoryName", order.TerritoryId);
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

        // #########################################################| ORDER LINES |######################################################### \\

        //Partial View displays order lines on Order Details page
        public ActionResult _OrderLines(int id = 0)
        {
            if (id != 0)
            {
                var lines = db.OrderLines.Where(x => x.OrderId == id).Include(o => o.LiftModel).ToList();
                return PartialView(lines);
            }
            return new EmptyResult();
        }

        // GET: /OrderLog/OrderLineCreate
        public ActionResult OrderLineCreate(int id)
        {
            var orderline = new OrderLine { OrderId = id }; // <-- Required to pass OrderId to POST method
            getOrderLineDropDowns();
            return View(orderline);
        }

        // POST: /OrderLog/OrderLineCreate
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OrderLineCreate([Bind(Include = "OrderId,LiftModelId,NewQty,CancelQty,Special,InstallId,Stock,RTG,Demo,Customer,City,State,Zip,Comment")] OrderLine orderline)
        {
            if (ModelState.IsValid)
            {
                db.OrderLines.Add(orderline);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = orderline.OrderId });
            }
            getOrderLineDropDowns(orderline);
            return View(orderline);
        }

        // GET: /OrderLog/OrderLineEdit/5
        public ActionResult OrderLineEdit(int? id)
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
            getOrderLineDropDowns(orderline);
            return View(orderline);
        }

        // POST: /OrderLog/OrderLineEdit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OrderLineEdit([Bind(Include = "OrderLineId,OrderId,LiftModelId,NewQty,CancelQty,Special,InstallId,Stock,RTG,Demo,Customer,City,State,Zip,Comment")] OrderLine orderline)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderline).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = orderline.OrderId });
            }
            return RedirectToAction("Details", new { id = orderline.OrderId });
        }

        private void getOrderLineDropDowns()
        {
            ViewBag.LiftModelId = new SelectList(db.LiftModels, "LiftModelId", "LiftModelName");
            ViewBag.InstallId = new SelectList(db.Installs, "InstallId", "InstallName");
        }

        private void getOrderLineDropDowns(OrderLine orderline)
        {
            ViewBag.LiftModelId = new SelectList(db.LiftModels, "LiftModelId", "LiftModelName", orderline.LiftModelId);
            ViewBag.InstallId = new SelectList(db.Installs, "InstallId", "InstallName", orderline.InstallId);
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