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
using Time.Data.EntityModels.DataPlates;

namespace Time.DataPlates.Controllers
{
    [Themed]
    public class DielectricRatingsController : Controller
    {
        private DataPlateEntities db = new DataPlateEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public DielectricRatingsController(IOrchardServices services)
        {
            Services = services;
            db = new DataPlateEntities();
        }

        public DielectricRatingsController(IOrchardServices services, DataPlateEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: DielectricRatings
        public ActionResult Index()
        {
            return View(db.DielectricRatings.ToList());
        }

        // GET: DielectricRatings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DielectricRating dielectricRating = db.DielectricRatings.Find(id);
            if (dielectricRating == null)
            {
                return HttpNotFound();
            }
            return View(dielectricRating);
        }

        // GET: DielectricRatings/Create
        public ActionResult Create()
        {
            if (!Services.Authorizer.Authorize(Permissions.DataPlateEditor, T("You Do Not Have Permission to Edit Data Plates")))
                return new HttpUnauthorizedResult();

            return View();
        }

        // POST: DielectricRatings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SearchOrder,Name,SearchString,Rating,LineVoltage")] DielectricRating dielectricRating)
        {
            if (!Services.Authorizer.Authorize(Permissions.DataPlateEditor, T("You Do Not Have Permission to Edit Data Plates")))
                return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
                db.DielectricRatings.Add(dielectricRating);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dielectricRating);
        }

        // GET: DielectricRatings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.DataPlateEditor, T("You Do Not Have Permission to Edit Data Plates")))
                return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DielectricRating dielectricRating = db.DielectricRatings.Find(id);
            if (dielectricRating == null)
            {
                return HttpNotFound();
            }
            return View(dielectricRating);
        }

        // POST: DielectricRatings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SearchOrder,Name,SearchString,Rating,LineVoltage")] DielectricRating dielectricRating)
        {
            if (!Services.Authorizer.Authorize(Permissions.DataPlateEditor, T("You Do Not Have Permission to Edit Data Plates")))
                return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
                db.Entry(dielectricRating).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dielectricRating);
        }

        // GET: DielectricRatings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.DataPlateEditor, T("You Do Not Have Permission to Edit Data Plates")))
                return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DielectricRating dielectricRating = db.DielectricRatings.Find(id);
            if (dielectricRating == null)
            {
                return HttpNotFound();
            }
            return View(dielectricRating);
        }

        // POST: DielectricRatings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.DataPlateEditor, T("You Do Not Have Permission to Edit Data Plates")))
                return new HttpUnauthorizedResult();

            DielectricRating dielectricRating = db.DielectricRatings.Find(id);
            db.DielectricRatings.Remove(dielectricRating);
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