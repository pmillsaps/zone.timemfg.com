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
using Time.Data.EntityModels.Configurator;

namespace Time.Configurator.Controllers
{
    [Themed]
    [Authorize]
    public class SpecialDatasController : Controller
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public SpecialDatasController(IOrchardServices services)
        {
            Services = services;
            db = new ConfiguratorEntities();
        }

        public SpecialDatasController(IOrchardServices services, ConfiguratorEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: SpecialDatas
        public ActionResult Index()
        {
            var specialDatas = db.SpecialDatas.Include(s => s.SpecialConfig).Include(s => s.SpecialDataType).Include(s => s.SpecialRelatedOp);
            return View(specialDatas.ToList());
        }

        // GET: SpecialDatas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpecialData specialData = db.SpecialDatas.Find(id);
            if (specialData == null)
            {
                return HttpNotFound();
            }
            return View(specialData);
        }

        // GET: SpecialDatas/Create
        public ActionResult Create()
        {
            ViewBag.SpecialConfigId = new SelectList(db.SpecialConfigs, "Id", "Name");
            ViewBag.SpecialDataTypeId = new SelectList(db.SpecialDataTypes, "Id", "Name");
            ViewBag.RelatedOpId = new SelectList(db.SpecialRelatedOps, "Id", "Operation");
            return View();
        }

        // POST: SpecialDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] SpecialData specialData)
        {
            //prevents duplicates when creating a new customer
            var datas = db.SpecialDatas.FirstOrDefault(x => x.Part == specialData.Part);

            //displays if previous code found a duplicate
            if (datas != null) ModelState.AddModelError("", "Duplicate Part Created---Please Check Data");

            if (ModelState.IsValid)
            {
                db.SpecialDatas.Add(specialData);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SpecialConfigId = new SelectList(db.SpecialConfigs, "Id", "Name", specialData.SpecialConfigId);
            ViewBag.SpecialDataTypeId = new SelectList(db.SpecialDataTypes, "Id", "Name", specialData.SpecialDataTypeId);
            ViewBag.RelatedOpId = new SelectList(db.SpecialRelatedOps, "Id", "Operation", specialData.RelatedOpId);
            return View(specialData);
        }

        // GET: SpecialDatas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpecialData specialData = db.SpecialDatas.Find(id);
            if (specialData == null)
            {
                return HttpNotFound();
            }
            ViewBag.SpecialConfigId = new SelectList(db.SpecialConfigs, "Id", "Name", specialData.SpecialConfigId);
            ViewBag.SpecialDataTypeId = new SelectList(db.SpecialDataTypes, "Id", "Name", specialData.SpecialDataTypeId);
            ViewBag.RelatedOpId = new SelectList(db.SpecialRelatedOps, "Id", "Operation", specialData.RelatedOpId);
            return View(specialData);
        }

        // POST: SpecialDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,SpecialConfigId,SpecialDataTypeId,Part,Quantity,Price,RelatedOpId")] SpecialData specialData)
        {
            //prevents a duplicate from being saved when editing
            var Datas = db.SpecialDatas.FirstOrDefault(x => x.SpecialConfigId == specialData.SpecialConfigId && x.SpecialDataTypeId == specialData.SpecialDataTypeId && x.Part == specialData.Part
                && x.Quantity == specialData.Quantity && x.Price == specialData.Price && x.Id != specialData.Id);

            //displays if previous code found a duplicate
            if (Datas != null) ModelState.AddModelError("", "Duplicate Lookup Created---Please Recheck Data");

            if (ModelState.IsValid)
            {
                db.Entry(specialData).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SpecialConfigId = new SelectList(db.SpecialConfigs, "Id", "Name", specialData.SpecialConfigId);
            ViewBag.SpecialDataTypeId = new SelectList(db.SpecialDataTypes, "Id", "Name", specialData.SpecialDataTypeId);
            ViewBag.RelatedOpId = new SelectList(db.SpecialRelatedOps, "Id", "Operation", specialData.RelatedOpId);
            return View(specialData);
        }

        // GET: SpecialDatas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpecialData specialData = db.SpecialDatas.Find(id);
            if (specialData == null)
            {
                return HttpNotFound();
            }
            return View(specialData);
        }

        // POST: SpecialDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SpecialData specialData = db.SpecialDatas.Find(id);
            db.SpecialDatas.Remove(specialData);
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
