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
using Time.Data.EntityModels.TimeMFG;

namespace Time.IT.Controllers
{
    [Themed]
    [Authorize]
    public class SchedulesController : Controller
    {
        private TimeMFGEntities db = new TimeMFGEntities();
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public SchedulesController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
        }

        // GET: Schedules
        public ActionResult Index()
        {
            return View(db.SysTaskSchedules.ToList());
        }

        // GET: Schedules/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SysTaskSchedule sysTaskSchedule = db.SysTaskSchedules.Find(id);
            if (sysTaskSchedule == null)
            {
                return HttpNotFound();
            }
            return View(sysTaskSchedule);
        }

        // GET: Schedules/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Schedules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] SysTaskSchedule sysTaskSchedule)
        {
            if (ModelState.IsValid)
            {
                db.SysTaskSchedules.Add(sysTaskSchedule);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sysTaskSchedule);
        }

        // GET: Schedules/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SysTaskSchedule sysTaskSchedule = db.SysTaskSchedules.Find(id);
            if (sysTaskSchedule == null)
            {
                return HttpNotFound();
            }
            return View(sysTaskSchedule);
        }

        // POST: Schedules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SysTaskSchedule sysTaskSchedule)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sysTaskSchedule).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sysTaskSchedule);
        }

        // GET: Schedules/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SysTaskSchedule sysTaskSchedule = db.SysTaskSchedules.Find(id);
            if (sysTaskSchedule == null)
            {
                return HttpNotFound();
            }
            return View(sysTaskSchedule);
        }

        // POST: Schedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SysTaskSchedule sysTaskSchedule = db.SysTaskSchedules.Find(id);
            db.SysTaskSchedules.Remove(sysTaskSchedule);
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