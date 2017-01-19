using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Time.Data.EntityModels.OrderLog;
using Time.OrderLog.Models;

namespace Time.OrderLog.Controllers
{
    [Authorize]
    [Themed]
    public class OrderTranController : Controller
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        private OrderLogEntities db;

        public OrderTranController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
            db = new OrderLogEntities();
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

        public OrderTranController(IOrchardServices services, OrderLogEntities _db)
        {
            Services = services;
            T = NullLocalizer.Instance;
            db = _db;
        }

        // GET: /OrderLine/
        public ActionResult Index()
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            var ordertrans = db.OrderTrans.Include(o => o.LiftModel).Include(o => o.Order);
            return View(ordertrans.ToList());
        }

        // GET: /OrderLine/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            return RedirectToAction("Details", "OrderLog", new { id = id });
        }

        // GET: /OrderLine/Create
        public ActionResult Create(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            var ordertran = new OrderTran() { OrderId = id, Date = DateTime.Now };
            var orderTrans = db.OrderTrans.FirstOrDefault(x => x.OrderId == id);
            var orderHeader = db.Orders.FirstOrDefault(x => x.OrderId == id);
            if (orderHeader != null)
            {
                ordertran.Special = orderHeader.Special;
                ordertran.Stock = orderHeader.Stock;
                ordertran.Demo = orderHeader.Demo;
                ordertran.RTG = orderHeader.RTG;
                ordertran.TruGuard = orderHeader.TruGuard;
            }

            if (orderTrans != null) ordertran.LiftModelId = orderTrans.LiftModelId;
            getDropDowns(ordertran);
            ViewBag.OrderId = id;
            return View(ordertran);
        }

        private void getDropDowns()
        {
            ViewBag.LiftModelId = new SelectList(db.LiftModels.OrderBy(x => x.LiftModelName), "LiftModelId", "LiftModelName");
        }

        private void getDropDowns(OrderTran orderTran)
        {
            ViewBag.LiftModelId = new SelectList(db.LiftModels.OrderBy(x => x.LiftModelName), "LiftModelId", "LiftModelName", orderTran.LiftModelId);
            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "PO", orderTran.OrderId);
        }

        // POST: /OrderLine/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "OrderTranId")] OrderTran ordertran, Order order)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            if (ordertran.NewQty < 0) ModelState.AddModelError("NewQty", "Negative Quantities are Not Allowed");
            if (ordertran.CancelQty < 0) ModelState.AddModelError("CancelQty", "Negative Quantities are Not Allowed");
            if (ordertran.NewQty == 0 && ordertran.CancelQty == 0) ModelState.AddModelError("", "At Least One of the Quantities Must Be Used");

            if (ModelState.IsValid)
            {
                if (ordertran.AsOfDate == null) ordertran.AsOfDate = ordertran.Date;
                db.OrderTrans.Add(ordertran);
                db.SaveChanges();

                order = db.Orders.Where(x => x.OrderId == ordertran.OrderId).Single();
                var ordertrans = db.OrderTrans.Where(x => x.OrderId == ordertran.OrderId).ToList();
                var ordertrantotal = 0;
                foreach (var item in ordertrans)
                {
                    ordertrantotal += (item.NewQty - item.CancelQty) * item.Price;
                }
                order.Price = ordertrantotal;

                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "OrderLog", new { id = ordertran.OrderId });
            }

            getDropDowns(ordertran);

            return View(ordertran);
        }

        // GET: /OrderLine/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            OrderTran ordertran = db.OrderTrans.Find(id);
            if (ordertran == null)
            {
                return HttpNotFound();
            }
            getDropDowns(ordertran);

            // Grab the previous URL and add it to the Model using ViewData or ViewBag
            ViewBag.returnUrl = Request.UrlReferrer;

            return View(ordertran);
        }

        // POST: /OrderLine/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include="OrderLineId,OrderId,LiftModelId,NewQty,CancelQty,Special,Install,Comment")] OrderLine orderline)
        public ActionResult Edit(OrderTran ordertran, string returnUrl)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
                if (ordertran.AsOfDate == null) ordertran.AsOfDate = ordertran.Date;
                db.Entry(ordertran).State = EntityState.Modified;
                db.SaveChanges();
                //return RedirectToAction("Index");
                //Redirect to Details page instead of Index -- see: http://stackoverflow.com/questions/9772947/c-sharp-asp-net-mvc-return-to-previous-page
                // @Html.ActionLink("Back to List", "Details", "OrderLog", new { id = Model.OrderId }, null)
                if (String.IsNullOrEmpty(returnUrl))
                    return RedirectToAction("Details", "OrderLog", new { id = ordertran.OrderId });
                else
                    return Redirect(returnUrl);
            }
            getDropDowns(ordertran);
            return View(ordertran);
        }

        // GET: /OrderLine/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            OrderTran ordertran = db.OrderTrans.Find(id);
            if (ordertran == null)
            {
                return HttpNotFound();
            }
            return View(ordertran);
        }

        // POST: /OrderLine/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            OrderTran ordertran = db.OrderTrans.Find(id);
            db.OrderTrans.Remove(ordertran);
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