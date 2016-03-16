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
using Time.Data.EntityModels.ITInventory;

namespace Time.IT.Controllers
{
    [Themed]
    public class ScheduledTasksController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ScheduledTasksController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
        }

        // GET: ScheduledTasks
        public ActionResult Index()
        {
            var scheduledTasks = db.ScheduledTasks.Include(s => s.Computer).OrderBy(x => x.Computer.Name).ThenBy(x => x.Name);
            return View(scheduledTasks.ToList());
        }

        // GET: ScheduledTasks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduledTask scheduledTask = db.ScheduledTasks.Find(id);
            if (scheduledTask == null)
            {
                return HttpNotFound();
            }
            return View(scheduledTask);
        }

        // GET: ScheduledTasks/Create
        public ActionResult Create(int id)
        {
            ScheduledTask task = new ScheduledTask { ComputerId = id };
            return View(task);
        }

        // POST: ScheduledTasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] ScheduledTask scheduledTask)
        {
            if (ModelState.IsValid)
            {
                scheduledTask.LastUpdatedDate = DateTime.Now;
                scheduledTask.LastUpdatedBy = System.Web.HttpContext.Current.User.Identity.Name;

                db.ScheduledTasks.Add(scheduledTask);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ComputerId = new SelectList(db.Computers, "Id", "Name", scheduledTask.ComputerId);
            return View(scheduledTask);
        }

        // GET: ScheduledTasks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduledTask scheduledTask = db.ScheduledTasks.Find(id);
            if (scheduledTask == null)
            {
                return HttpNotFound();
            }
            ViewBag.ComputerId = new SelectList(db.Computers, "Id", "Name", scheduledTask.ComputerId);
            return View(scheduledTask);
        }

        // POST: ScheduledTasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ScheduledTask scheduledTask)
        {
            if (ModelState.IsValid)
            {
                db.Entry(scheduledTask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ComputerId = new SelectList(db.Computers, "Id", "Name", scheduledTask.ComputerId);
            return View(scheduledTask);
        }

        // GET: ScheduledTasks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduledTask scheduledTask = db.ScheduledTasks.Find(id);
            if (scheduledTask == null)
            {
                return HttpNotFound();
            }
            return View(scheduledTask);
        }

        // POST: ScheduledTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ScheduledTask scheduledTask = db.ScheduledTasks.Find(id);
            db.ScheduledTasks.Remove(scheduledTask);
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