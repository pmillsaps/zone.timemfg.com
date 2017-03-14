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
using Time.Data.EntityModels.Install;
using Time.Install.ViewModels;

namespace Time.Install.Controllers
{
    [Themed]
    [Authorize]
    public class LiftFamiliesController : Controller
    {
        private string ErrorMessage { get; set; }
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public LiftFamiliesController(IOrchardServices services)
        {
            Services = services;
        }

        private VSWQuotesEntities db = new VSWQuotesEntities();

        // GET: LiftFamilies
        public ActionResult Index(string Search)
        {
            var liftFmls = db.LiftFamilies.Include(c => c.ChassisSpecsForWordDocs).AsQueryable();
            if (String.IsNullOrEmpty(Search))
            {
                return View(liftFmls.ToList());
            }
            else
            {
                return View(liftFmls.Where(x => x.FamilyName.Contains(Search)).ToList());
            }
            
        }

        // GET: LiftFamilies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiftFamily liftFamily = db.LiftFamilies.Find(id);
            if (liftFamily == null)
            {
                return HttpNotFound();
            }
            return View(liftFamily);
        }

        // GET: LiftFamilies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LiftFamilies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LiftFamilyViewModel vm)
        {
            // Prevents a duplicate from being created
            var lFmly = db.LiftFamilies.FirstOrDefault(x => x.FamilyName == vm.Lift.FamilyName);

            // Displays if previous code found a duplicate
            if (lFmly != null) ModelState.AddModelError("", "Lift Family Name Already Exists");

            if (ModelState.IsValid)
            {
                // Adding the Lift Family
                db.LiftFamilies.Add(vm.Lift);
                db.SaveChanges();
                // Adding the Installation Specs
                var liftF = db.LiftFamilies.OrderByDescending(x => x.Id).FirstOrDefault();
                vm.ChassisSpecs.LiftFamilyId = liftF.Id;
                db.ChassisSpecsForWordDocs.Add(vm.ChassisSpecs);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(vm);
        }

        // GET: LiftFamilies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiftFamily liftFamily = db.LiftFamilies.Find(id);
            if (liftFamily == null)
            {
                return HttpNotFound();
            }
            LiftFamilyViewModel vm = new LiftFamilyViewModel();
            vm.Lift = liftFamily;
            vm.ChassisSpecs = db.ChassisSpecsForWordDocs.FirstOrDefault(x => x.LiftFamilyId == id);
            return View(vm);
        }

        // POST: LiftFamilies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LiftFamilyViewModel vm)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vm.Lift).State = EntityState.Modified;
                //db.SaveChanges();
                db.Entry(vm.ChassisSpecs).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vm);
        }

        // GET: LiftFamilies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiftFamily liftFamily = db.LiftFamilies.Find(id);
            if (liftFamily == null)
            {
                return HttpNotFound();
            }
            return View(liftFamily);
        }

        // POST: LiftFamilies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            
            ChassisSpecsForWordDoc chassisSpec = db.ChassisSpecsForWordDocs.FirstOrDefault(x => x.LiftFamilyId == id);
            LiftFamily liftFamily = db.LiftFamilies.Find(id);
            db.ChassisSpecsForWordDocs.Remove(chassisSpec);
            db.LiftFamilies.Remove(liftFamily);
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
