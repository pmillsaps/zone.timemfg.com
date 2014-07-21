using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Time.Legacy.EntityModels.Legacy;

namespace Time.Legacy.Controllers
{
    public class HomeController : Controller
    {
        private LegacyEntities db = new LegacyEntities();

        // GET: Legacy
        public ActionResult Index()
        {
            return View(db.Inventories.Take(1000));
        }

        // GET: Legacy/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory inventory = db.Inventories.Find(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }
            return View(inventory);
        }

        // GET: Legacy/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Legacy/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PartNumber,Description,Vendor,Vendor2,Vendor3,VendorPartNumber,AdditionalDescription,AdditionalDescription2,AggrBuyoutCost,AggrLaborCost,CurrentMaterialCost,DateLastOrdered,DateLastRecieved,DateLastSold,DrawingDate,DrawingRevision,DrawingSize,IncrBuyoutCost,IncrLaborCost,IncrLastPurchaseCost,LiftRequired,LiftRequired2,LiftRequired3,ListPrice,PartType,PartsExpediteFlag,Phantom,UnitPrice2,UnitPrice3,UnitPrice4,UnitPrice5,UnitPrice6")] Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                db.Inventories.Add(inventory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(inventory);
        }

        // GET: Legacy/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory inventory = db.Inventories.Find(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }
            return View(inventory);
        }

        // POST: Legacy/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PartNumber,Description,Vendor,Vendor2,Vendor3,VendorPartNumber,AdditionalDescription,AdditionalDescription2,AggrBuyoutCost,AggrLaborCost,CurrentMaterialCost,DateLastOrdered,DateLastRecieved,DateLastSold,DrawingDate,DrawingRevision,DrawingSize,IncrBuyoutCost,IncrLaborCost,IncrLastPurchaseCost,LiftRequired,LiftRequired2,LiftRequired3,ListPrice,PartType,PartsExpediteFlag,Phantom,UnitPrice2,UnitPrice3,UnitPrice4,UnitPrice5,UnitPrice6")] Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inventory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(inventory);
        }

        // GET: Legacy/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory inventory = db.Inventories.Find(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }
            return View(inventory);
        }

        // POST: Legacy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Inventory inventory = db.Inventories.Find(id);
            db.Inventories.Remove(inventory);
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