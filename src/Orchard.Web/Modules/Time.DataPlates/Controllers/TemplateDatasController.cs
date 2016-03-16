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
    public class TemplateDatasController : Controller
    {
        private DataPlateEntities db = new DataPlateEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public TemplateDatasController(IOrchardServices services)
        {
            Services = services;
            db = new DataPlateEntities();
        }

        public TemplateDatasController(IOrchardServices services, DataPlateEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: TemplateDatas
        public ActionResult Index()
        {
            return View(db.TemplateDatas.ToList());
        }

        // GET: TemplateDatas/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TemplateData templateData = db.TemplateDatas.Find(id);
            if (templateData == null)
            {
                return HttpNotFound();
            }
            return View(templateData);
        }

        // GET: TemplateDatas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TemplateDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FieldName,FieldData,TemplateFullFileName")] TemplateData templateData)
        {
            if (ModelState.IsValid)
            {
                db.TemplateDatas.Add(templateData);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(templateData);
        }

        // GET: TemplateDatas/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TemplateData templateData = db.TemplateDatas.Find(id);
            if (templateData == null)
            {
                return HttpNotFound();
            }
            return View(templateData);
        }

        // POST: TemplateDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FieldName,FieldData,TemplateFullFileName")] TemplateData templateData)
        {
            if (ModelState.IsValid)
            {
                db.Entry(templateData).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(templateData);
        }

        // GET: TemplateDatas/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TemplateData templateData = db.TemplateDatas.Find(id);
            if (templateData == null)
            {
                return HttpNotFound();
            }
            return View(templateData);
        }

        // POST: TemplateDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            TemplateData templateData = db.TemplateDatas.Find(id);
            db.TemplateDatas.Remove(templateData);
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
