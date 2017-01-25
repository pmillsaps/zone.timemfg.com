using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Time.Invoicing.EntityModels.PcInvoice;
using Time.Invoicing.Logging;

namespace Time.Invoicing.Controllers {
    [Authorize]
    [Themed]
    public class ChassisController : Controller {

        private readonly PCInvoiceEntities _db;
        private readonly ILogger _logger;
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ChassisController(IOrchardServices services) {
            Services = services;
            T = NullLocalizer.Instance;

            _db = new PCInvoiceEntities();
            _logger = new NLogger();
        }

        public ChassisController(IOrchardServices services, PCInvoiceEntities db)
        {
            Services = services;
            T = NullLocalizer.Instance;

            _db = db;
            _logger = new NLogger();
        }

        //
        // GET: /Chassis/

        public ActionResult Index()
        {
            if (TempData["ErrorMessage"] != null) ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();

            //var chassis = _db.Chassis.Include("CALookup").Include("ChassisModel").Include("GVWLookup").Include("ModelYearLookup").Include("Transmission").OrderBy(x => x.FullName);
            var chassis = _db.Chassis.OrderBy(x => x.FullName);

            return View(chassis.ToList());
            //return View();
        }

        //
        // GET: /Chassis/Details/5

        public ActionResult Details(int id = 0)
        {
            Chassis chassis = _db.Chassis.SingleOrDefault(c => c.Id == id);
            if (chassis == null) return HttpNotFound();
            return View(chassis);
        }

        //
        // GET: /Chassis/Create

        //[Authorize(Roles = "InvoicingAdmin")]
        public ActionResult Create()
        {
            if (!Services.Authorizer.Authorize(Permissions.InvoicingAdmin, T("Couldn't Edit Chassis")))
                return new HttpUnauthorizedResult();

            GenerateDropDownLists(new Chassis());

            return View();
        }

        //
        // POST: /Chassis/Create

        //[Authorize(Roles = "InvoicingAdmin")]
        [HttpPost]
        public ActionResult Create(Chassis chassis)
        {
            if (!Services.Authorizer.Authorize(Permissions.InvoicingAdmin, T("Couldn't Edit Chassis")))
                return new HttpUnauthorizedResult();

            CheckIfExists(chassis);

            if (ModelState.IsValid)
            {
                chassis = UpdateFullName(chassis);
                _db.Chassis.Add(chassis);
                _db.SaveChanges();
                _logger.Info(string.Format("User '{0}' created Chassis '{1}'", HttpContext.User.Identity.Name, chassis.FullName));
                return RedirectToAction("Index");
            }

            GenerateDropDownLists(chassis);

            return View(chassis);
        }

        //
        // GET: /Chassis/Edit/5

        //[Authorize(Roles = "InvoicingAdmin")]
        public ActionResult Edit(int id = 0)
        {
            if (!Services.Authorizer.Authorize(Permissions.InvoicingAdmin, T("Couldn't Edit Chassis")))
                return new HttpUnauthorizedResult();

            Chassis chassis = _db.Chassis.SingleOrDefault(c => c.Id == id);
            if (chassis == null) return HttpNotFound();
            if (CheckIfUsed(chassis))
            {
                TempData["ErrorMessage"] = String.Format("Chassis '{0}' is being used - It cannot be modified", chassis.FullName);
                return RedirectToAction("Index");
            }

            GenerateDropDownLists(chassis);

            return View(chassis);
        }

        //
        // POST: /Chassis/Edit/5

        [Authorize(Roles = "InvoicingAdmin")]
        [HttpPost]
        public ActionResult Edit(Chassis chassis)
        {
            CheckIfExists(chassis);

            if (ModelState.IsValid)
            {
                chassis = UpdateFullName(chassis);

                _db.Chassis.Attach(chassis);
                // _db.ObjectStateManager.ChangeObjectState(chassis, EntityState.Modified);
                _db.SaveChanges();
                _logger.Info(string.Format("User '{0}' edited Chassis '{1}'", HttpContext.User.Identity.Name, chassis.FullName));
                return RedirectToAction("Index");
            }
            GenerateDropDownLists(chassis);

            return View(chassis);
        }

        //
        // GET: /Chassis/Delete/5

        [Authorize(Roles = "InvoicingAdmin")]
        public ActionResult Delete(int id = 0)
        {
            Chassis chassis = _db.Chassis.SingleOrDefault(c => c.Id == id);
            if (chassis == null) return HttpNotFound();

            if (CheckIfUsed(chassis))
            {
                TempData["ErrorMessage"] = String.Format("Chassis '{0}' is being used - It cannot be deleted", chassis.FullName);

                return RedirectToAction("Index");
            }

            return View(chassis);
        }

        //
        // POST: /Chassis/Delete/5

        [Authorize(Roles = "InvoicingAdmin")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Chassis chassis = _db.Chassis.Single(c => c.Id == id);
            if (chassis != null)
            {
                _db.Chassis.Remove(chassis);
                _db.SaveChanges();
                _logger.Info(string.Format("User '{0}' deleted Chassis '{1}'", HttpContext.User.Identity.Name, chassis.FullName));
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        private void GenerateDropDownLists(Chassis chassis)
        {
            ViewBag.CALookupId = new SelectList(_db.CALookups, "Id", "Name", chassis.CALookupId);
            ViewBag.ChassisModelId = new SelectList(_db.ChassisModels, "Id", "Name", chassis.ChassisModelId);
            ViewBag.GVWLookupId = new SelectList(_db.GVWLookups, "Id", "Name", chassis.GVWLookupId);
            ViewBag.ModelYearId = new SelectList(_db.ModelYearLookups, "Id", "Name", chassis.ModelYearId);
            ViewBag.TransmissionId = new SelectList(_db.Transmissions, "Id", "Name", chassis.TransmissionId);
        }

        private Chassis UpdateFullName(Chassis chassis)
        {
            //if (!chassis.ModelYearLookupReference.IsLoaded) chassis.ModelYearLookupReference.Load();
            //if (!chassis.ModelYearLookupReference.IsLoaded) chassis.ModelYearLookupReference.Load();
            //if (!chassis.ModelYearLookupReference.IsLoaded) chassis.ModelYearLookupReference.Load();
            //if (!chassis.ModelYearLookupReference.IsLoaded) chassis.ModelYearLookupReference.Load();
            //if (!chassis.ModelYearLookupReference.IsLoaded) chassis.ModelYearLookupReference.Load();

            chassis.ModelYearLookup = _db.ModelYearLookups.Single(x => x.Id == chassis.ModelYearId);
            chassis.ChassisModel = _db.ChassisModels.Single(x => x.Id == chassis.ChassisModelId);
            chassis.Transmission = _db.Transmissions.Single(x => x.Id == chassis.TransmissionId);
            chassis.CALookup = _db.CALookups.Single(x => x.Id == chassis.CALookupId);
            chassis.GVWLookup = _db.GVWLookups.Single(x => x.Id == chassis.GVWLookupId);

            // chassis.ChassisModel.ChassisMake = _db.ChassisMakes.Single(x => x.Id == chassis.ChassisModel.ChassisMakeId);

            chassis.FullName = String.Format("{0} {1} {2} {3} {4} CA {5} GVW",
                                    chassis.ModelYearLookup.Name,
                                    chassis.ChassisModel.ChassisMake.Name,
                                    chassis.ChassisModel.Name,
                                    chassis.Transmission.Name,
                                    chassis.CALookup.Name,
                                    chassis.GVWLookup.Name);

            return chassis;
        }

        private bool CheckIfUsed(Chassis chassis)
        {
            if (_db.OrderHeaders.Count(c => c.ChassisId == chassis.Id) > 0) return true;
            if (_db.OrderDetails.Count(c => c.ChassisId == chassis.Id) > 0) return true;

            return false;
        }

        private void CheckIfExists(Chassis chassis)
        {
            var uniqueCheck =
                _db.Chassis.SingleOrDefault(
                        x => x.CALookupId == chassis.CALookupId &&
                        x.ChassisModelId == chassis.ChassisModelId &&
                        x.GVWLookupId == chassis.GVWLookupId &&
                        x.ModelYearId == chassis.ModelYearId &&
                        x.TransmissionId == chassis.TransmissionId &&
                        x.Id != chassis.Id
                    );

            if (uniqueCheck != null)
                ModelState.AddModelError("", "This combination already exists in the list of Chassis'");
        }
        
    }
}
