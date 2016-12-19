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
    [Authorize]
    [Themed]
    public class PS_OptionController : Controller
    {
        private DataPlateEntities db = new DataPlateEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public PS_OptionController(IOrchardServices services)
        {
            Services = services;
            db = new DataPlateEntities();
        }

        public PS_OptionController(IOrchardServices services, DataPlateEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: PS_Option
        public ActionResult Index(string search)
        {
            if (String.IsNullOrEmpty(search))
            {
                return View(db.PS_Option.ToList());
            }
            else
            {
                return View(db.PS_Option.Where(x => x.Option.Contains(search)).ToList());
            }
        }

        // GET: PS_Option/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PS_Option pS_Option = db.PS_Option.Find(id);
            if (pS_Option == null)
            {
                return HttpNotFound();
            }
            return View(pS_Option);
        }

        // GET: PS_Option/Create
        public ActionResult Create()
        {
            if (!Services.Authorizer.Authorize(Permissions.DataPlateEditor, T("You Do Not Have Permission to Edit Data Plates")))
                return new HttpUnauthorizedResult();

            return View();
        }

        // POST: PS_Option/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PS_Option pS_Option)
        {
            if (!Services.Authorizer.Authorize(Permissions.DataPlateEditor, T("You Do Not Have Permission to Edit Data Plates")))
                return new HttpUnauthorizedResult();

            // Alerting the user about inserting a duplicate
            var option = db.PS_Option.FirstOrDefault(x => x.Option == pS_Option.Option);
            if (option != null) ModelState.AddModelError("", "Option already exists!");

            if (ModelState.IsValid)
            {
                db.PS_Option.Add(pS_Option);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pS_Option);
        }

        // GET: PS_Option/Edit/5
        public ActionResult Edit(string id)
        {
            if (!Services.Authorizer.Authorize(Permissions.DataPlateEditor, T("You Do Not Have Permission to Edit Data Plates")))
                return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PS_Option pS_Option = db.PS_Option.Find(id);
            if (pS_Option == null)
            {
                return HttpNotFound();
            }
            return View(pS_Option);
        }

        // POST: PS_Option/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PS_Option pS_Option)
        {
            if (!Services.Authorizer.Authorize(Permissions.DataPlateEditor, T("You Do Not Have Permission to Edit Data Plates")))
                return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
                db.Entry(pS_Option).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pS_Option);
        }

        // GET: PS_Option/Delete/5
        public ActionResult Delete(string id)
        {
            if (!Services.Authorizer.Authorize(Permissions.DataPlateEditor, T("You Do Not Have Permission to Edit Data Plates")))
                return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PS_Option pS_Option = db.PS_Option.Find(id);
            if (pS_Option == null)
            {
                return HttpNotFound();
            }
            return View(pS_Option);
        }

        // POST: PS_Option/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            if (!Services.Authorizer.Authorize(Permissions.DataPlateEditor, T("You Do Not Have Permission to Edit Data Plates")))
                return new HttpUnauthorizedResult();

            PS_Option pS_Option = db.PS_Option.Find(id);
            db.PS_Option.Remove(pS_Option);
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