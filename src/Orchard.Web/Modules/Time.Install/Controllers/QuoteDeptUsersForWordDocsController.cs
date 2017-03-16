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
    public class QuoteDeptUsersForWordDocsController : Controller
    {
        private VSWQuotesEntities db = new VSWQuotesEntities();

        private string ErrorMessage { get; set; }
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public QuoteDeptUsersForWordDocsController(IOrchardServices services)
        {
            Services = services;
        }

        // GET: QuoteDeptUsersForWordDocs
        public ActionResult Index()
        {
            return View(db.QuoteDeptUsersForWordDocs.ToList());
        }

        // GET: QuoteDeptUsersForWordDocs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuoteDeptUsersForWordDoc quoteDeptUsersForWordDoc = db.QuoteDeptUsersForWordDocs.Find(id);
            if (quoteDeptUsersForWordDoc == null)
            {
                return HttpNotFound();
            }
            return View(quoteDeptUsersForWordDoc);
        }

        // GET: QuoteDeptUsersForWordDocs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: QuoteDeptUsersForWordDocs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] QuoteDeptUsersForWordDoc quoteDeptUsersForWordDoc)
        {
            if (ModelState.IsValid)
            {
                db.QuoteDeptUsersForWordDocs.Add(quoteDeptUsersForWordDoc);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(quoteDeptUsersForWordDoc);
        }

        // GET: QuoteDeptUsersForWordDocs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuoteDeptUsersForWordDoc quoteDeptUsersForWordDoc = db.QuoteDeptUsersForWordDocs.Find(id);
            if (quoteDeptUsersForWordDoc == null)
            {
                return HttpNotFound();
            }
            return View(quoteDeptUsersForWordDoc);
        }

        // POST: QuoteDeptUsersForWordDocs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(QuoteDeptUsersForWordDoc quoteDeptUsersForWordDoc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(quoteDeptUsersForWordDoc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(quoteDeptUsersForWordDoc);
        }

        // GET: QuoteDeptUsersForWordDocs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuoteDeptUsersForWordDoc quoteDeptUsersForWordDoc = db.QuoteDeptUsersForWordDocs.Find(id);
            if (quoteDeptUsersForWordDoc == null)
            {
                return HttpNotFound();
            }
            return View(quoteDeptUsersForWordDoc);
        }

        // POST: QuoteDeptUsersForWordDocs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QuoteDeptUsersForWordDoc quoteDeptUsersForWordDoc = db.QuoteDeptUsersForWordDocs.Find(id);
            db.QuoteDeptUsersForWordDocs.Remove(quoteDeptUsersForWordDoc);
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
