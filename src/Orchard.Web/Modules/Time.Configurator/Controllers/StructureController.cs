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
    public class StructureController : Controller
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        // GET: /Structure/
        public ActionResult Index()
        {
            return View(db.Structures.OrderBy(x => x.ConfigName).ThenBy(x => x.ConfigData).ToList());
        }

        // GET: /Structure/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Structure structure = db.Structures.Find(id);
            if (structure == null)
            {
                return HttpNotFound();
            }
            return View(structure);
        }

        // GET: /Structure/Create
        public ActionResult Create()
        {
            GenerateDropDowns();
            return View();
        }

        // POST: /Structure/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude="Id")] Structure structure)
        {

            var Configs = db.Structures.FirstOrDefault(x => x.ConfigName == structure.ConfigName && x.ConfigData == structure.ConfigData && x.Id != structure.Id);

            if (Configs != null) ModelState.AddModelError("", "Duplicate Structure Created---Please Check Inputed Data");

            if (ModelState.IsValid)
            {
                db.Structures.Add(structure);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(structure);
            return View(structure);
        }

        // GET: /Structure/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Structure structure = db.Structures.Find(id);
            if (structure == null)
            {
                return HttpNotFound();
            }
            GenerateDropDowns();
            return View(structure);
        }

        // POST: /Structure/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Structure structure)
        {
            var Configs = db.Structures.FirstOrDefault(x => x.ConfigName == structure.ConfigName && x.ConfigData == structure.ConfigData && x.Id != structure.Id);

            if (Configs != null) ModelState.AddModelError("", "Duplicate Structure Created---Please Check Inputed Data");

            if (ModelState.IsValid)
            {
                db.Entry(structure).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(structure);
            return View(structure);
        }

        // GET: /Structure/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Structure structure = db.Structures.Find(id);
            if (structure == null)
            {
                return HttpNotFound();
            }
            return View(structure);
        }

        // POST: /Structure/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Structure structure = db.Structures.Find(id);
            db.Structures.Remove(structure);
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

        private void GenerateDropDowns()
        {
            //prevent duplicates from showing up in drop down
            //without var list codes, every CFG and Global shows up in drop down and whatever else for the other drop downs
            var ConfigNameList = from firstList in db.Structures
                                 group firstList by firstList.ConfigName into newList1
                                 let x = newList1.FirstOrDefault()
                                 select x;

            ViewBag.ConfigName = new SelectList(ConfigNameList.ToList(), "ConfigName", "ConfigName");
        }

        private void GenerateDropDowns(Structure structure)
        {
            ViewBag.ConfigName = new SelectList(db.Structures.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName", structure.ConfigName);
        }
    }
}
