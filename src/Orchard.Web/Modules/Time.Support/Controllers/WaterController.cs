using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
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
        private readonly TimeMFGEntities db;
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public WaterController(IOrchardServices services)
        {
            Services = services;
            db = new TimeMFGEntities();
        }

        public WaterController(IOrchardServices services, TimeMFGEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: Water
        public ActionResult Index()
        {
            if (!Services.Authorizer.Authorize(Permissions.ViewWaterReports, T("You do not have access to this report. Please log in.")))
                return new HttpUnauthorizedResult();
            return View(db.WaterTests.OrderByDescending(x => x.Date).ToList());
        }

        // GET: Water/Details/5
        public ActionResult Details(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.ViewWaterReports, T("You do not have access to this report. Please log in.")))
                return new HttpUnauthorizedResult();
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
            if (!Services.Authorizer.Authorize(Permissions.EnterWaterReports, T("You do not have access to enter data. Please log in.")))
                return new HttpUnauthorizedResult();
            return View();
        }

        // POST: Water/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WaterTest waterTest)
        {
            if (!Services.Authorizer.Authorize(Permissions.EnterWaterReports, T("You do not have access to enter data. Please log in.")))
                return new HttpUnauthorizedResult();
            waterTest.SysDateTime = DateTime.Now;
            waterTest.SysUser = HttpContext.User.Identity.Name;
            // waterTest.SysComputer = Environment.MachineName;
            waterTest.SysComputer = Request.UserHostAddress;
            if (waterTest.Date > waterTest.SysDateTime) ModelState.AddModelError("Date", "The Date Entered is in the future - Are You A Time Traveler?");

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
            if (!Services.Authorizer.Authorize(Permissions.EditWaterReports, T("You do not have access to enter data. Please log in.")))
                return new HttpUnauthorizedResult();
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
        public ActionResult Edit(WaterTest waterTest)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditWaterReports, T("You do not have access to enter data. Please log in.")))
                return new HttpUnauthorizedResult();
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
            if (!Services.Authorizer.Authorize(Permissions.EditWaterReports, T("You do not have access to enter data. Please log in.")))
                return new HttpUnauthorizedResult();
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
            if (!Services.Authorizer.Authorize(Permissions.EditWaterReports, T("You do not have access to enter data. Please log in.")))
                return new HttpUnauthorizedResult();
            WaterTest waterTest = db.WaterTests.Find(id);
            db.WaterTests.Remove(waterTest);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public void Export()
        {
            if (Services.Authorizer.Authorize(Permissions.EditWaterReports, T("You do not have access to enter data. Please log in.")))
            {
                StringWriter sw = new StringWriter();

                sw.WriteLine("\"User Name\",\"Date\",\"Gallons Used\",\"PH\",\"Temp\"");

                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=Exported_WaterReport.csv");
                Response.ContentType = "text/csv";

                foreach (var line in db.WaterTests.OrderByDescending(x => x.Date))
                {
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                                               line.UserName,
                                               line.Date.ToShortDateString(),
                                               line.GallonsUsed,
                                               line.PH,
                                               line.Temp));
                }

                Response.Write(sw.ToString());

                Response.End();
            }
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