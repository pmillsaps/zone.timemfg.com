using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.ITInventory;

namespace Time.IT.Controllers
{
    [Themed]
    [Authorize]
    public class ModelController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: Model
        public ActionResult Index()
        {
            var ref_Model = db.Ref_Model.OrderBy(x => x.Model).Include(r => r.Ref_Manufacturer);
            return View(ref_Model.ToList());
        }

        // GET: Model/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_Model ref_Model = db.Ref_Model.Find(id);
            if (ref_Model == null)
            {
                return HttpNotFound();
            }
            return View(ref_Model);
        }

        // GET: Model/Create
        public ActionResult Create()
        {
            GenerateDropDowns(new Ref_Model());
            return View();
        }

        // POST: Model/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "ID")] Ref_Model ref_Model)
        {
            if (ModelState.IsValid)
            {
                db.Ref_Model.Add(ref_Model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            GenerateDropDowns(ref_Model);
            return View(ref_Model);
        }

        // GET: Model/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_Model ref_Model = db.Ref_Model.Find(id);
            if (ref_Model == null)
            {
                return HttpNotFound();
            }
            GenerateDropDowns(ref_Model);
            return View(ref_Model);
        }

        // POST: Model/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ref_Model ref_Model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ref_Model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(ref_Model);
            return View(ref_Model);
        }

        // GET: Model/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_Model ref_Model = db.Ref_Model.Find(id);
            if (ref_Model == null)
            {
                return HttpNotFound();
            }
            return View(ref_Model);
        }

        // POST: Model/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ref_Model ref_Model = db.Ref_Model.Find(id);
            db.Ref_Model.Remove(ref_Model);
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

        private void GenerateDropDowns(Ref_Model ref_Model)
        {
            ViewBag.ManufacturerId = new SelectList(db.Ref_Manufacturer.OrderBy(x => x.Name), "Id", "Name", ref_Model.ManufacturerId);
        }
    }
}