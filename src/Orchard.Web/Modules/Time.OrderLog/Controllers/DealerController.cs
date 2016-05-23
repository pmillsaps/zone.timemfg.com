using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Time.Data.EntityModels.OrderLog;
using Time.OrderLog.Models;

namespace Time.OrderLog.Controllers
{
    [Themed]
    public class DealerController : Controller
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        private OrderLogEntities db;

        public DealerController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
            db = new OrderLogEntities();
            Setup();
        }

        public DealerController(IOrchardServices services, OrderLogEntities _db)
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

        // GET: /Dealer/
        public ActionResult Index()
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            var dealers = db.Dealers.OrderBy(x => x.DealerName);
            return View(dealers.ToList());
        }

        // GET: /Dealer/Create
        public ActionResult Create()
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            getDropDowns();
            return View();
        }

        // POST: /Dealer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DealerName,TerritoryId")] Dealer dealer)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();


            if (ModelState.IsValid)
            {
                db.Dealers.Add(dealer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            getDropDowns(dealer);
            //ViewBag.TerritoryId = new SelectList(db.Territories, "TerritoryId", "TerritoryName", dealer.TerritoryId);
            return View(dealer);
        }

        // GET: /Dealer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            Dealer dealer = db.Dealers.Find(id);
            if (dealer == null)
            {
                return HttpNotFound();
            }
            getDropDowns(dealer);
            return View(dealer);
        }

        // POST: /Dealer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DealerId,DealerName,TerritoryId")] Dealer dealer)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            if (ModelState.IsValid)
            {
                db.Entry(dealer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.TerritoryId = new SelectList(db.Territories, "TerritoryId", "TerritoryName", dealer.TerritoryId);
            return View(dealer);
        }

        // GET: /Dealer/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            Dealer dealer = db.Dealers.Find(id);
            if (dealer == null)
            {
                return HttpNotFound();
            }
            return View(dealer);
        }

        // POST: /Dealer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            Dealer dealer = db.Dealers.Find(id);
            db.Dealers.Remove(dealer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void getDropDowns()
        {
            //ViewBag.DealerId = new SelectList(db.Dealers.OrderBy(x => x.DealerName), "DealerId", "DealerName");
            ViewBag.TerritoryId = new SelectList(db.Territories.OrderBy(x => x.TerritoryName), "TerritoryId", "TerritoryName");
        }

        private void getDropDowns(Dealer dealer)
        {
            //ViewBag.DealerId = new SelectList(db.Dealers.OrderBy(x => x.DealerName), "DealerId", "DealerName", order.DealerId);
            ViewBag.TerritoryId = new SelectList(db.Territories.OrderBy(x => x.TerritoryName), "TerritoryId", "TerritoryName", dealer.TerritoryId);
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