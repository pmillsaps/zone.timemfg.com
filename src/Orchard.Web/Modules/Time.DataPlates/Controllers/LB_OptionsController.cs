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
    public class LB_OptionsController : Controller
    {
        private DataPlateEntities db = new DataPlateEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public LB_OptionsController(IOrchardServices services)
        {
            Services = services;
            db = new DataPlateEntities();
        }

        public LB_OptionsController(IOrchardServices services, DataPlateEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: LB_Options
        public ActionResult Index(string search)
        {
            if (String.IsNullOrEmpty(search))
            {
                return View(db.LB_Options.ToList());
            }
            else
            {
                return View(db.LB_Options.Where(x => x.Option.Contains(search)).ToList());
            }
        }

        // GET: LB_Options/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LB_Options lB_Options = db.LB_Options.Find(id);
            if (lB_Options == null)
            {
                return HttpNotFound();
            }
            return View(lB_Options);
        }

        // GET: LB_Options/Create
        public ActionResult Create()
        {
            if (!Services.Authorizer.Authorize(Permissions.DataPlateEditor, T("You Do Not Have Permission to Edit Data Plates")))
                return new HttpUnauthorizedResult();

            return View();
        }

        // POST: LB_Options/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LB_Options lB_Options)
        {
            if (!Services.Authorizer.Authorize(Permissions.DataPlateEditor, T("You Do Not Have Permission to Edit Data Plates")))
                return new HttpUnauthorizedResult();

            // Alerting the user about inserting a duplicate
            var option = db.LB_Options.FirstOrDefault(x => x.Option == lB_Options.Option);
            if (option != null) ModelState.AddModelError("", "Option already exists!");

            if (ModelState.IsValid)
            {
                db.LB_Options.Add(lB_Options);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(lB_Options);
        }

        // GET: LB_Options/Edit/5
        public ActionResult Edit(string id)
        {
            if (!Services.Authorizer.Authorize(Permissions.DataPlateEditor, T("You Do Not Have Permission to Edit Data Plates")))
                return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LB_Options lB_Options = db.LB_Options.Find(id);
            if (lB_Options == null)
            {
                return HttpNotFound();
            }
            return View(lB_Options);
        }

        // POST: LB_Options/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LB_Options lB_Options)
        {
            if (!Services.Authorizer.Authorize(Permissions.DataPlateEditor, T("You Do Not Have Permission to Edit Data Plates")))
                return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
                db.Entry(lB_Options).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(lB_Options);
        }

        // GET: LB_Options/Delete/5
        public ActionResult Delete(string id)
        {
            if (!Services.Authorizer.Authorize(Permissions.DataPlateEditor, T("You Do Not Have Permission to Edit Data Plates")))
                return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LB_Options lB_Options = db.LB_Options.Find(id);
            if (lB_Options == null)
            {
                return HttpNotFound();
            }
            return View(lB_Options);
        }

        // POST: LB_Options/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            if (!Services.Authorizer.Authorize(Permissions.DataPlateEditor, T("You Do Not Have Permission to Edit Data Plates")))
                return new HttpUnauthorizedResult();

            LB_Options lB_Options = db.LB_Options.Find(id);
            db.LB_Options.Remove(lB_Options);
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