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
    public class LiftDatasController : Controller
    {
        private DataPlateEntities db = new DataPlateEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public LiftDatasController(IOrchardServices services)
        {
            Services = services;
            db = new DataPlateEntities();
        }

        public LiftDatasController(IOrchardServices services, DataPlateEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: LiftDatas
        public ActionResult Index(string search)
        {
            if (String.IsNullOrEmpty(search))
            {
                return View(db.LiftDatas.ToList());
            }
            else
            {
                return View(db.LiftDatas.Where(x => x.Lift.Contains(search)).ToList());
            }
        }

        // GET: LiftDatas/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiftData liftData = db.LiftDatas.Find(id);
            if (liftData == null)
            {
                return HttpNotFound();
            }
            return View(liftData);
        }

        // GET: LiftDatas/Create
        public ActionResult Create()
        {
            if (!Services.Authorizer.Authorize(Permissions.DataPlateEditor, T("You Do Not Have Permission to Edit Data Plates")))
                return new HttpUnauthorizedResult();

            return View();
        }

        // POST: LiftDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LiftData liftData)
        {
            if (!Services.Authorizer.Authorize(Permissions.DataPlateEditor, T("You Do Not Have Permission to Edit Data Plates")))
                return new HttpUnauthorizedResult();

            // Alerting the user about inserting a duplicate
            var option = db.LiftDatas.FirstOrDefault(x => x.Lift == liftData.Lift);
            if (option != null) ModelState.AddModelError("", "Lift Model already exists!");

            if (ModelState.IsValid)
            {
                db.LiftDatas.Add(liftData);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(liftData);
        }

        // GET: LiftDatas/Edit/5
        public ActionResult Edit(string id)
        {
            if (!Services.Authorizer.Authorize(Permissions.DataPlateEditor, T("You Do Not Have Permission to Edit Data Plates")))
                return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiftData liftData = db.LiftDatas.Find(id);
            if (liftData == null)
            {
                return HttpNotFound();
            }
            return View(liftData);
        }

        // POST: LiftDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LiftData liftData)
        {
            if (!Services.Authorizer.Authorize(Permissions.DataPlateEditor, T("You Do Not Have Permission to Edit Data Plates")))
                return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
                db.Entry(liftData).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(liftData);
        }

        // GET: LiftDatas/Delete/5
        public ActionResult Delete(string id)
        {
            if (!Services.Authorizer.Authorize(Permissions.DataPlateEditor, T("You Do Not Have Permission to Edit Data Plates")))
                return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiftData liftData = db.LiftDatas.Find(id);
            if (liftData == null)
            {
                return HttpNotFound();
            }
            return View(liftData);
        }

        // POST: LiftDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            if (!Services.Authorizer.Authorize(Permissions.DataPlateEditor, T("You Do Not Have Permission to Edit Data Plates")))
                return new HttpUnauthorizedResult();

            LiftData liftData = db.LiftDatas.Find(id);
            db.LiftDatas.Remove(liftData);
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