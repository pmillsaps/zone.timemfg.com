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
    public class PortController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: Port
        public ActionResult Index()
        {
            return View(db.Ref_SwitchPort.OrderBy(x => x.SwitchPort).ToList());
        }

        // GET: Port/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_SwitchPort ref_SwitchPort = db.Ref_SwitchPort.Find(id);
            if (ref_SwitchPort == null)
            {
                return HttpNotFound();
            }
            return View(ref_SwitchPort);
        }

        // GET: Port/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Port/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] Ref_SwitchPort ref_SwitchPort)
        {
            if (ModelState.IsValid)
            {
                db.Ref_SwitchPort.Add(ref_SwitchPort);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ref_SwitchPort);
        }

        // GET: Port/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_SwitchPort ref_SwitchPort = db.Ref_SwitchPort.Find(id);
            if (ref_SwitchPort == null)
            {
                return HttpNotFound();
            }
            return View(ref_SwitchPort);
        }

        // POST: Port/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ref_SwitchPort ref_SwitchPort)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ref_SwitchPort).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ref_SwitchPort);
        }

        // GET: Port/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_SwitchPort ref_SwitchPort = db.Ref_SwitchPort.Find(id);
            if (ref_SwitchPort == null)
            {
                return HttpNotFound();
            }
            return View(ref_SwitchPort);
        }

        // POST: Port/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ref_SwitchPort ref_SwitchPort = db.Ref_SwitchPort.Find(id);
            db.Ref_SwitchPort.Remove(ref_SwitchPort);
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