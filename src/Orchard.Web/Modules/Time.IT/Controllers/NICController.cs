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
    public class NICController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: Ref_NIC
        public ActionResult Index()
        {
            var ref_NIC = db.Ref_NIC.Include(r => r.Ref_CableNo).Include(r => r.Ref_NICSpeed).Include(r => r.Ref_SwitchPort);
            return View(ref_NIC.ToList());
        }

        // GET: Ref_NIC/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_NIC ref_NIC = db.Ref_NIC.Find(id);
            if (ref_NIC == null)
            {
                return HttpNotFound();
            }
            return View(ref_NIC);
        }

        // GET: Ref_NIC/Create
        public ActionResult Create(int id)
        {
            ViewBag.Id = id;
            ViewBag.CableId = new SelectList(db.Ref_CableNo, "Id", "Name");
            ViewBag.SpeedId = new SelectList(db.Ref_NICSpeed, "Id", "NIC_Speed");
            ViewBag.SwitchPortId = new SelectList(db.Ref_SwitchPort, "Id", "SwitchPort");
            return View();
        }

        // POST: Ref_NIC/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MAC,IP,SpeedId,Type,SwitchPortId,CableId")] Ref_NIC ref_NIC)
        {
            if (ModelState.IsValid)
            {
                db.Ref_NIC.Add(ref_NIC);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CableId = new SelectList(db.Ref_CableNo, "Id", "Name", ref_NIC.CableId);
            ViewBag.SpeedId = new SelectList(db.Ref_NICSpeed, "Id", "NIC_Speed", ref_NIC.SpeedId);
            ViewBag.SwitchPortId = new SelectList(db.Ref_SwitchPort, "Id", "SwitchPort", ref_NIC.SwitchPortId);
            return View(ref_NIC);
        }

        // GET: Ref_NIC/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_NIC ref_NIC = db.Ref_NIC.Find(id);
            if (ref_NIC == null)
            {
                return HttpNotFound();
            }
            ViewBag.CableId = new SelectList(db.Ref_CableNo, "Id", "Name", ref_NIC.CableId);
            ViewBag.SpeedId = new SelectList(db.Ref_NICSpeed, "Id", "NIC_Speed", ref_NIC.SpeedId);
            ViewBag.SwitchPortId = new SelectList(db.Ref_SwitchPort, "Id", "SwitchPort", ref_NIC.SwitchPortId);
            return View(ref_NIC);
        }

        // POST: Ref_NIC/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,MAC,IP,SpeedId,Type,SwitchPortId,CableId")] Ref_NIC ref_NIC)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ref_NIC).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CableId = new SelectList(db.Ref_CableNo, "Id", "Name", ref_NIC.CableId);
            ViewBag.SpeedId = new SelectList(db.Ref_NICSpeed, "Id", "NIC_Speed", ref_NIC.SpeedId);
            ViewBag.SwitchPortId = new SelectList(db.Ref_SwitchPort, "Id", "SwitchPort", ref_NIC.SwitchPortId);
            return View(ref_NIC);
        }

        // GET: Ref_NIC/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_NIC ref_NIC = db.Ref_NIC.Find(id);
            if (ref_NIC == null)
            {
                return HttpNotFound();
            }
            return View(ref_NIC);
        }

        // POST: Ref_NIC/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ref_NIC ref_NIC = db.Ref_NIC.Find(id);
            db.Ref_NIC.Remove(ref_NIC);
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
