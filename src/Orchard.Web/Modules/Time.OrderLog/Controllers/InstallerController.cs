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
using Time.Data.EntityModels.OrderLog;
using Time.OrderLog.Models;

namespace Time.OrderLog.Controllers
{
    [Authorize]
    [Themed]
    public class InstallerController : Controller
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        private OrderLogEntities db;

        public InstallerController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
            db = new OrderLogEntities();
            Setup();
        }

        public InstallerController(IOrchardServices services, OrderLogEntities _db)
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

        // GET: Installer
        public ActionResult Index()
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            return View(db.Installers.OrderBy(x => x.InstallerName).ToList());
        }

        // GET: Installer/Create
        public ActionResult Create()
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            return View();
        }

        // POST: Installer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "InstallerId,InstallerName")] Installer installer)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            if (ModelState.IsValid)
            {
                db.Installers.Add(installer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(installer);
        }

        // GET: Installer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Installer installer = db.Installers.Find(id);
            if (installer == null)
            {
                return HttpNotFound();
            }
            return View(installer);
        }

        // POST: Installer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "InstallerId,InstallerName")] Installer installer)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            if (ModelState.IsValid)
            {
                db.Entry(installer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(installer);
        }

        // GET: Installer/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Installer installer = db.Installers.Find(id);
            if (installer == null)
            {
                return HttpNotFound();
            }
            return View(installer);
        }

        // POST: Installer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditOrders, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            Installer installer = db.Installers.Find(id);
            db.Installers.Remove(installer);
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