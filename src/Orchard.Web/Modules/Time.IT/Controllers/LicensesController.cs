﻿using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.ITInventory;
using Time.IT.Models;

namespace Time.IT.Controllers
{
    [Themed]
    [Authorize]
    public class LicensesController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: Licenses
        public ActionResult Index(string search = "")
        {
            return View();
            //var licenses = db.Licenses.OrderBy(x => x.Name).ThenBy(x => x.LicenseKey).ThenBy(x => x.Quantity).Include(l => l.Ref_LicenseType);
            //if (!String.IsNullOrEmpty(search))
            //    licenses = licenses.Where(x => x.Name.Contains(search) || x.Note.Contains(search) ||
            //    x.LicenseKey.Contains(search) || x.PO.Contains(search));
            //return View(licenses.ToList());
        }

        // Load the data for the Index table
        public ActionResult LoadLicenses()
        {
            List<LicensesViewModel> model = new List<LicensesViewModel>();
            var licenses = db.Licenses.OrderBy(x => x.Name).ThenBy(x => x.LicenseKey).ThenBy(x => x.Quantity).Include(l => l.Ref_LicenseType);
            foreach (var item in licenses)
            {
                LicensesViewModel lvm = new LicensesViewModel();
                lvm.Id = item.Id;
                lvm.Name = item.Name;
                lvm.Quantity = item.Quantity;
                lvm.LicenseKey = item.LicenseKey;
                lvm.QuantityAssigned = item.QuantityAssigned;
                lvm.LicenseType = (item.Ref_LicenseType == null)? "" : item.Ref_LicenseType.LicenseType;
                lvm.PO = item.PO;
                lvm.PurchaseDate = (item.PurchaseDate == null) ? "" : item.PurchaseDate.Value.ToShortDateString();
                lvm.Note = (item.Note == null)? "" : item.Note;
                if (item.Computers.Count() == 1)
                {
                    var cmp = item.Computers.First();
                    lvm.CompOrUserName = cmp.Name;
                }
                else if (item.Users.Count() == 1)
                {
                    var user = item.Users.First();
                    lvm.CompOrUserName = user.Name;
                }
                else
                {
                    lvm.CompOrUserName = "";
                }
                model.Add(lvm);
            }
            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }

        // GET: Licenses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            License license = db.Licenses.Find(id);
            if (license == null)
            {
                return HttpNotFound();
            }
            return View(license);
        }

        // GET: Licenses/Create
        public ActionResult Create()
        {
            ViewBag.LicenseTypeId = new SelectList(db.Ref_LicenseType, "Id", "LicenseType");
            return View();
        }

        // POST: Licenses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] License license)
        {
            ValidateLicenseNumber(license);

            if (ModelState.IsValid)
            {
                db.Licenses.Add(license);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.LicenseTypeId = new SelectList(db.Ref_LicenseType, "Id", "LicenseType", license.LicenseTypeId);
            return View(license);
        }

        private void ValidateLicenseNumber(License license)
        {
            var lic = db.Licenses.Where(x => x.LicenseKey == license.LicenseKey && x.Id != license.Id);
            if (lic.Count() > 0) ModelState.AddModelError("LicenseKey", "License Key is a Duplicate Entry");
        }

        // GET: Licenses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            License license = db.Licenses.Find(id);
            if (license == null)
            {
                return HttpNotFound();
            }
            ViewBag.LicenseTypeId = new SelectList(db.Ref_LicenseType, "Id", "LicenseType", license.LicenseTypeId);
            return View(license);
        }

        // POST: Licenses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(License license)
        {
            ValidateLicenseNumber(license);
            if (ModelState.IsValid)
            {
                db.Entry(license).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LicenseTypeId = new SelectList(db.Ref_LicenseType, "Id", "LicenseType", license.LicenseTypeId);
            return View(license);
        }

        // GET: Licenses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            License license = db.Licenses.Find(id);
            if (license == null)
            {
                return HttpNotFound();
            }
            return View(license);
        }

        public ActionResult RecountLicenses()
        {
            var licenses = db.Licenses.ToList();
            foreach (var item in licenses)
            {
                item.QuantityAssigned = item.Computers.Count + item.Users.Count;
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Licenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            License license = db.Licenses.Find(id);
            db.Licenses.Remove(license);
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