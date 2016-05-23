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
    public class TerritoryController : Controller
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        private OrderLogEntities db;

        public TerritoryController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
            db = new OrderLogEntities();
            Setup();
        }

        public TerritoryController(IOrchardServices services, OrderLogEntities _db)
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

        // GET: Territory
        public ActionResult Index()
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            var territories = db.Territories.Include(t => t.Region).OrderBy(x => x.TerritoryName);
            return View(territories.ToList());
        }

        // GET: Territory/Create
        public ActionResult Create()
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            ViewBag.RegionId = new SelectList(db.Regions, "RegionId", "RegionName");
            return View();
        }

        // POST: Territory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TerritoryId,RegionId,TerritoryName")] Territory territory)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            if (ModelState.IsValid)
            {
                db.Territories.Add(territory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RegionId = new SelectList(db.Regions, "RegionId", "RegionName", territory.RegionId);
            return View(territory);
        }

        // GET: Territory/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            Territory territory = db.Territories.Find(id);
            if (territory == null)
            {
                return HttpNotFound();
            }
            ViewBag.RegionId = new SelectList(db.Regions, "RegionId", "RegionName", territory.RegionId);
            return View(territory);
        }

        // POST: Territory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TerritoryId,RegionId,TerritoryName")] Territory territory)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            if (ModelState.IsValid)
            {
                db.Entry(territory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RegionId = new SelectList(db.Regions, "RegionId", "RegionName", territory.RegionId);
            return View(territory);
        }

        // GET: Territory/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            Territory territory = db.Territories.Find(id);
            if (territory == null)
            {
                return HttpNotFound();
            }
            return View(territory);
        }

        // POST: Territory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            Territory territory = db.Territories.Find(id);
            db.Territories.Remove(territory);
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