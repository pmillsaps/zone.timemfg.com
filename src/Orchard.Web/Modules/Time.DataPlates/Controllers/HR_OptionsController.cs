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
    public class HR_OptionsController : Controller
    {
        private DataPlateEntities db = new DataPlateEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public HR_OptionsController(IOrchardServices services)
        {
            Services = services;
            db = new DataPlateEntities();
        }

        public HR_OptionsController(IOrchardServices services, DataPlateEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: HR_Options
        public ActionResult Index(string search)
        {
            if (String.IsNullOrEmpty(search))
            {
                return View(db.HR_Options.ToList());
            }
            else
            {
                return View(db.HR_Options.Where(x => x.Option.Contains(search)).ToList());
            }
        }

        // GET: HR_Options/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_Options hR_Options = db.HR_Options.Find(id);
            if (hR_Options == null)
            {
                return HttpNotFound();
            }
            return View(hR_Options);
        }

        // GET: HR_Options/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HR_Options/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Option,HRUpperControls")] HR_Options hR_Options)
        {
            // Alerting the user about inserting a duplicate
            var option = db.HR_Options.FirstOrDefault(x => x.Option == hR_Options.Option);
            if (option != null) ModelState.AddModelError("", "Option already exists!");

            if (ModelState.IsValid)
            {
                db.HR_Options.Add(hR_Options);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hR_Options);
        }

        // GET: HR_Options/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_Options hR_Options = db.HR_Options.Find(id);
            if (hR_Options == null)
            {
                return HttpNotFound();
            }
            return View(hR_Options);
        }

        // POST: HR_Options/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Option,HRUpperControls")] HR_Options hR_Options)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hR_Options).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hR_Options);
        }

        // GET: HR_Options/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_Options hR_Options = db.HR_Options.Find(id);
            if (hR_Options == null)
            {
                return HttpNotFound();
            }
            return View(hR_Options);
        }

        // POST: HR_Options/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            HR_Options hR_Options = db.HR_Options.Find(id);
            db.HR_Options.Remove(hR_Options);
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
