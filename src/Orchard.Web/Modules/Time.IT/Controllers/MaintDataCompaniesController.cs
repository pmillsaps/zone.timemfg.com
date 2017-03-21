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
    public class MaintDataCompaniesController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: MaintDataCompanies
        public ActionResult Index()
        {
            return View(db.MaintDataCompanies.ToList());
        }

        // GET: MaintDataCompanies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaintDataCompany maintDataCompany = db.MaintDataCompanies.Find(id);
            if (maintDataCompany == null)
            {
                return HttpNotFound();
            }
            return View(maintDataCompany);
        }

        // GET: MaintDataCompanies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MaintDataCompanies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] MaintDataCompany maintDataCompany)
        {
            if (ModelState.IsValid)
            {
                db.MaintDataCompanies.Add(maintDataCompany);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(maintDataCompany);
        }

        // GET: MaintDataCompanies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaintDataCompany maintDataCompany = db.MaintDataCompanies.Find(id);
            if (maintDataCompany == null)
            {
                return HttpNotFound();
            }
            return View(maintDataCompany);
        }

        // POST: MaintDataCompanies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CompanyName")] MaintDataCompany maintDataCompany)
        {
            if (ModelState.IsValid)
            {
                db.Entry(maintDataCompany).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(maintDataCompany);
        }

        // GET: MaintDataCompanies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaintDataCompany maintDataCompany = db.MaintDataCompanies.Find(id);
            if (maintDataCompany == null)
            {
                return HttpNotFound();
            }
            return View(maintDataCompany);
        }

        // POST: MaintDataCompanies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MaintDataCompany maintDataCompany = db.MaintDataCompanies.Find(id);
            db.MaintDataCompanies.Remove(maintDataCompany);
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
