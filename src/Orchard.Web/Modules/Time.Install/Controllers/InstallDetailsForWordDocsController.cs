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
    public class InstallDetailsForWordDocsController : Controller
    {
        private VSWQuotesEntities db = new VSWQuotesEntities();

        private string ErrorMessage { get; set; }
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public InstallDetailsForWordDocsController(IOrchardServices services)
        {
            Services = services;
        }

        // GET: InstallDetailsForWordDocs
        public ActionResult Index()
        {
            return View(db.InstallDetailsForWordDocs.ToList());
        }

        // GET: InstallDetailsForWordDocs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstallDetailsForWordDoc installDetailsForWordDoc = db.InstallDetailsForWordDocs.Find(id);
            if (installDetailsForWordDoc == null)
            {
                return HttpNotFound();
            }
            return View(installDetailsForWordDoc);
        }

        // GET: InstallDetailsForWordDocs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InstallDetailsForWordDocs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] InstallDetailsForWordDoc installDetailsForWordDoc)
        {
            if (ModelState.IsValid)
            {
                db.InstallDetailsForWordDocs.Add(installDetailsForWordDoc);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(installDetailsForWordDoc);
        }

        // GET: InstallDetailsForWordDocs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstallDetailsForWordDoc installDetailsForWordDoc = db.InstallDetailsForWordDocs.Find(id);
            if (installDetailsForWordDoc == null)
            {
                return HttpNotFound();
            }
            return View(installDetailsForWordDoc);
        }

        // POST: InstallDetailsForWordDocs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(InstallDetailsForWordDoc installDetailsForWordDoc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(installDetailsForWordDoc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(installDetailsForWordDoc);
        }

        // GET: InstallDetailsForWordDocs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstallDetailsForWordDoc installDetailsForWordDoc = db.InstallDetailsForWordDocs.Find(id);
            if (installDetailsForWordDoc == null)
            {
                return HttpNotFound();
            }
            return View(installDetailsForWordDoc);
        }

        // POST: InstallDetailsForWordDocs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InstallDetailsForWordDoc installDetailsForWordDoc = db.InstallDetailsForWordDocs.Find(id);
            db.InstallDetailsForWordDocs.Remove(installDetailsForWordDoc);
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
