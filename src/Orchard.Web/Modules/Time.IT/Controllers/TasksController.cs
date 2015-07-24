﻿using Orchard;
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
    public class TasksController : Controller
    {
        private TimeMFGEntities db = new TimeMFGEntities();
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public TasksController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
        }

        // GET: Tasks
        public ActionResult Index()
        {
            var sysTasks = db.SysTasks.Include(s => s.SysTaskSchedule);
            return View(sysTasks.ToList());
        }

        // GET: Tasks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SysTask sysTask = db.SysTasks.Find(id);
            if (sysTask == null)
            {
                return HttpNotFound();
            }
            return View(sysTask);
        }

        // GET: Tasks/Create
        public ActionResult Create()
        {
            ViewBag.SysTaskSchedNum = new SelectList(db.SysTaskSchedules, "Id", "Description");
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] SysTask sysTask)
        {
            if (ModelState.IsValid)
            {
                db.SysTasks.Add(sysTask);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SysTaskSchedNum = new SelectList(db.SysTaskSchedules, "Id", "Description", sysTask.SysTaskSchedNum);
            return View(sysTask);
        }

        // GET: Tasks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SysTask sysTask = db.SysTasks.Find(id);
            if (sysTask == null)
            {
                return HttpNotFound();
            }
            ViewBag.SysTaskSchedNum = new SelectList(db.SysTaskSchedules, "Id", "Description", sysTask.SysTaskSchedNum);
            return View(sysTask);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SysTask sysTask)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sysTask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SysTaskSchedNum = new SelectList(db.SysTaskSchedules, "Id", "Description", sysTask.SysTaskSchedNum);
            return View(sysTask);
        }

        // GET: Tasks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SysTask sysTask = db.SysTasks.Find(id);
            if (sysTask == null)
            {
                return HttpNotFound();
            }
            return View(sysTask);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SysTask sysTask = db.SysTasks.Find(id);
            db.SysTasks.Remove(sysTask);
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