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
using Time.Data.EntityModels.Configurator;

namespace Time.Configurator.Controllers
{
    //sets theme and requires you to log in to go to the page
    [Themed]
    [Authorize]
    public class SpecialCustomersController : Controller
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        //private string[] tokens;

        public SpecialCustomersController(IOrchardServices services)
        {
            Services = services;
            db = new ConfiguratorEntities();
        }

        public SpecialCustomersController(IOrchardServices services, ConfiguratorEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: SpecialCustomers
        public ActionResult Index()
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to View this Page")))
                return new HttpUnauthorizedResult();

            return View(db.SpecialCustomers.OrderBy(x => x.Name).ToList());
        }

        // GET: SpecialCustomers/Details/5
        public ActionResult Details(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to View this Page")))
                return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpecialCustomer specialCustomer = db.SpecialCustomers.Find(id);
            if (specialCustomer == null)
            {
                return HttpNotFound();
            }

            var specialConfig = db.SpecialConfigs.Where(x => x.SpecialCustomerId == specialCustomer.Id).OrderBy(x => x.Name).ToList();
            ViewBag.SpecialConfig = specialConfig;

            return View(specialCustomer);
        }

        // GET: SpecialCustomers/Create
        public ActionResult Create()
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to View this Page")))
                return new HttpUnauthorizedResult();

            return View();
        }

        // POST: SpecialCustomers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] SpecialCustomer specialCustomer)
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to View this Page")))
                return new HttpUnauthorizedResult();

            //prevents duplicates when creating a new customer
            var Cust = db.SpecialCustomers.FirstOrDefault(x => x.Name == specialCustomer.Name);

            //displays if previous code found a duplicate
            if (Cust != null) ModelState.AddModelError("", "Duplicate Customer Created---Please Check Data");

            if (ModelState.IsValid)
            {
                db.SpecialCustomers.Add(specialCustomer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(specialCustomer);
        }

        // GET: SpecialCustomers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to View this Page")))
                return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpecialCustomer specialCustomer = db.SpecialCustomers.Find(id);
            if (specialCustomer == null)
            {
                return HttpNotFound();
            }
            return View(specialCustomer);
        }

        // POST: SpecialCustomers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] SpecialCustomer specialCustomer)
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to View this Page")))
                return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
                db.Entry(specialCustomer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(specialCustomer);
        }

        // GET: SpecialCustomers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to View this Page")))
                return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpecialCustomer specialCustomer = db.SpecialCustomers.Find(id);
            if (specialCustomer == null)
            {
                return HttpNotFound();
            }
            return View(specialCustomer);
        }

        // POST: SpecialCustomers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to View this Page")))
                return new HttpUnauthorizedResult();

            SpecialCustomer specialCustomer = db.SpecialCustomers.Find(id);
            db.SpecialCustomers.Remove(specialCustomer);
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