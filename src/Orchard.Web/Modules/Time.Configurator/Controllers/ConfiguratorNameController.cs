using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.Configurator;

namespace Time.Configurator.Controllers
{
    [Themed]
    [Authorize]
    public class ConfiguratorNameController : Controller
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        // GET: /ConfiguratorName/
        public ActionResult Index()
        {
            return View(db.ConfiguratorNames.OrderBy(x => x.ConfigName).ToList());
        }

        // GET: /ConfiguratorName/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfiguratorName configuratorname = db.ConfiguratorNames.Find(id);
            if (configuratorname == null)
            {
                return HttpNotFound();
            }
            return View(configuratorname);
        }

        // GET: /ConfiguratorName/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /ConfiguratorName/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude="Id")] ConfiguratorName configuratorname)
        {
            var Configs = db.ConfiguratorNames.FirstOrDefault(x => x.ConfigName == configuratorname.ConfigName);

            if (Configs != null) ModelState.AddModelError("ConfigName", "Duplicate Configurator Name");

            if (ModelState.IsValid)
            {
                db.ConfiguratorNames.Add(configuratorname);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(configuratorname);
        }

        // GET: /ConfiguratorName/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfiguratorName configuratorname = db.ConfiguratorNames.Find(id);
            if (configuratorname == null)
            {
                return HttpNotFound();
            }
            return View(configuratorname);
        }

        // POST: /ConfiguratorName/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ConfiguratorName configuratorname)
        {
            var Configs = db.ConfiguratorNames.FirstOrDefault(x => x.ConfigName == configuratorname.ConfigName && x.Id != configuratorname.Id);

            if (Configs != null) ModelState.AddModelError("ConfigName", "Duplicate Configurator Name");

            if (ModelState.IsValid)
            {
                db.Entry(configuratorname).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(configuratorname);
        }

        // GET: /ConfiguratorName/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfiguratorName configuratorname = db.ConfiguratorNames.Find(id);
            if (configuratorname == null)
            {
                return HttpNotFound();
            }
            return View(configuratorname);
        }

        // POST: /ConfiguratorName/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ConfiguratorName configuratorname = db.ConfiguratorNames.Find(id);
            db.ConfiguratorNames.Remove(configuratorname);
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
