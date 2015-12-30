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
    public class CA_OptionsController : Controller
    {
        private DataPlateEntities db = new DataPlateEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public CA_OptionsController(IOrchardServices services)
        {
            Services = services;
            db = new DataPlateEntities();
        }

        public CA_OptionsController(IOrchardServices services, DataPlateEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: CA_Options
        public ActionResult Index(string search)
        {
            if (String.IsNullOrEmpty(search))
            {
                return View(db.CA_Options.ToList());
            }
            else
            {
                return View(db.CA_Options.Where(x => x.Option.StartsWith(search)).ToList());
            }            
        }

        // GET: CA_Options/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CA_Options cA_Options = db.CA_Options.Find(id);
            if (cA_Options == null)
            {
                return HttpNotFound();
            }
            return View(cA_Options);
        }

        // GET: CA_Options/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CA_Options/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Option,Cap_LBS,Cap_KG,NonPlatformCap")] CA_Options cA_Options)
        {
            // Alerting the user about inserting a duplicate
            var option = db.CA_Options.FirstOrDefault(x => x.Option == cA_Options.Option);
            if (option != null) ModelState.AddModelError("", "Option already exists!");

            if (ModelState.IsValid)
            {
                db.CA_Options.Add(cA_Options);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cA_Options);
        }

        // GET: CA_Options/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CA_Options cA_Options = db.CA_Options.Find(id);
            if (cA_Options == null)
            {
                return HttpNotFound();
            }
            return View(cA_Options);
        }

        // POST: CA_Options/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Option,Cap_LBS,Cap_KG,NonPlatformCap")] CA_Options cA_Options)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cA_Options).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cA_Options);
        }

        // GET: CA_Options/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CA_Options cA_Options = db.CA_Options.Find(id);
            if (cA_Options == null)
            {
                return HttpNotFound();
            }
            return View(cA_Options);
        }

        // POST: CA_Options/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            CA_Options cA_Options = db.CA_Options.Find(id);
            db.CA_Options.Remove(cA_Options);
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
