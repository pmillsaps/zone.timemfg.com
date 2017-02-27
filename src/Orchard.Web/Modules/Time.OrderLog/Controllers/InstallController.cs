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
    [Authorize]
    [Themed]
    public class InstallController : Controller
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        private OrderLogEntities db;

        public InstallController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
            db = new OrderLogEntities();
            Setup();
        }

        public InstallController(IOrchardServices services, OrderLogEntities _db)
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

        // GET: Install
        public ActionResult Index()
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            return View(db.Installs.ToList());
        }

        // GET: Install/Create
        public ActionResult Create()
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            return View();
        }

        // POST: Install/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "InstallId,InstallName")] Install install)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            if (ModelState.IsValid)
            {
                db.Installs.Add(install);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(install);
        }

        // GET: Install/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Install install = db.Installs.Find(id);
            if (install == null)
            {
                return HttpNotFound();
            }
            return View(install);
        }

        // POST: Install/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "InstallId,InstallName")] Install install)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            if (ModelState.IsValid)
            {
                db.Entry(install).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(install);
        }

        // GET: Install/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Install install = db.Installs.Find(id);
            if (install == null)
            {
                return HttpNotFound();
            }
            return View(install);
        }

        // POST: Install/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            Install install = db.Installs.Find(id);
            db.Installs.Remove(install);
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