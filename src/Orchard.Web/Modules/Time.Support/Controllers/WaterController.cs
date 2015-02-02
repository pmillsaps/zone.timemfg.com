using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.TimeMFG;


namespace Time.Support.Controllers
{
    [Authorize]
    [Themed]
    public class WaterController : Controller
    {
        private TimeMFGEntities db = new TimeMFGEntities();

        // GET: Water
        public ActionResult Index()
        {
            return View(db.WaterTests.OrderByDescending(x => x.Date).ToList());
        }

        // GET: Water/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WaterTest waterTest = db.WaterTests.Find(id);
            if (waterTest == null)
            {
                return HttpNotFound();
            }
            return View(waterTest);
        }

        // GET: Water/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Water/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserName,Date,GallonsUsed,PH,Temp")] WaterTest waterTest)
        {
            waterTest.SysDateTime = DateTime.Now;
            waterTest.SysUser = HttpContext.User.Identity.Name;
            // waterTest.SysComputer = Environment.MachineName;
            waterTest.SysComputer = Request.UserHostAddress;

            if (ModelState.IsValid)
            {
                db.WaterTests.Add(waterTest);
                db.SaveChanges();
                return RedirectToAction("Index", "Ticket");
            }

            return View(waterTest);
        }

        // GET: Water/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WaterTest waterTest = db.WaterTests.Find(id);
            if (waterTest == null)
            {
                return HttpNotFound();
            }
            return View(waterTest);
        }

        // POST: Water/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserName,Date,GallonsUsed,PH,Temp,SysDateTime,SysUser,SysComputer")] WaterTest waterTest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(waterTest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(waterTest);
        }

        // GET: Water/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WaterTest waterTest = db.WaterTests.Find(id);
            if (waterTest == null)
            {
                return HttpNotFound();
            }
            return View(waterTest);
        }

        // POST: Water/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WaterTest waterTest = db.WaterTests.Find(id);
            db.WaterTests.Remove(waterTest);
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
