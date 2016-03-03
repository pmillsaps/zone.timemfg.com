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
    public class EP_SS_OptionsController : Controller
    {
        private DataPlateEntities db = new DataPlateEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public EP_SS_OptionsController(IOrchardServices services)
        {
            Services = services;
            db = new DataPlateEntities();
        }

        public EP_SS_OptionsController(IOrchardServices services, DataPlateEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: EP_SS_Options
        public ActionResult Index(string search)
        {
            if (String.IsNullOrEmpty(search))
            {
                return View(db.EP_SS_Options.ToList());
            }
            else
            {
                return View(db.EP_SS_Options.Where(x => x.Option.Contains(search)).ToList());
            }
        }

        // GET: EP_SS_Options/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EP_SS_Options eP_SS_Options = db.EP_SS_Options.Find(id);
            if (eP_SS_Options == null)
            {
                return HttpNotFound();
            }
            return View(eP_SS_Options);
        }

        // GET: EP_SS_Options/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EP_SS_Options/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Option,Voltage")] EP_SS_Options eP_SS_Options)
        {
            // Alerting the user about inserting a duplicate
            var option = db.EP_SS_Options.FirstOrDefault(x => x.Option == eP_SS_Options.Option);
            if (option != null) ModelState.AddModelError("", "Option already exists!");

            if (ModelState.IsValid)
            {
                db.EP_SS_Options.Add(eP_SS_Options);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(eP_SS_Options);
        }

        // GET: EP_SS_Options/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EP_SS_Options eP_SS_Options = db.EP_SS_Options.Find(id);
            if (eP_SS_Options == null)
            {
                return HttpNotFound();
            }
            return View(eP_SS_Options);
        }

        // POST: EP_SS_Options/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Option,Voltage")] EP_SS_Options eP_SS_Options)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eP_SS_Options).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(eP_SS_Options);
        }

        // GET: EP_SS_Options/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EP_SS_Options eP_SS_Options = db.EP_SS_Options.Find(id);
            if (eP_SS_Options == null)
            {
                return HttpNotFound();
            }
            return View(eP_SS_Options);
        }

        // POST: EP_SS_Options/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            EP_SS_Options eP_SS_Options = db.EP_SS_Options.Find(id);
            db.EP_SS_Options.Remove(eP_SS_Options);
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
