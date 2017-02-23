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
        public ActionResult Index()
        {
            return View(db.LiftFamilies.ToList());
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
        public ActionResult Create([Bind(Exclude = "Id")] LiftFamily liftFamily)
        {
            if (ModelState.IsValid)
            {
                db.LiftFamilies.Add(liftFamily);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(liftFamily);
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
            return View(liftFamily);
        }

        // POST: LiftFamilies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LiftFamily liftFamily)
        {
            if (ModelState.IsValid)
            {
                db.Entry(liftFamily).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(liftFamily);
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
            LiftFamily liftFamily = db.LiftFamilies.Find(id);
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
