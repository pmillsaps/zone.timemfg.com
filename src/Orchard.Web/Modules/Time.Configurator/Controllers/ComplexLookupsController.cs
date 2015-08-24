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
using Time.Data.EntityModels.Configurator;

namespace Time.Configurator.Controllers
{
    [Themed]
    [Authorize]
    public class ComplexLookupsController : Controller
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ComplexLookupsController(IOrchardServices services)
        {
            Services = services;
            db = new ConfiguratorEntities();
        }

        public ComplexLookupsController(IOrchardServices services, ConfiguratorEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: ComplexLookups
        public ActionResult Index(string ConfigNames, string ConfigData)
        {
            ViewBag.ConfigNames = new SelectList(db.ConfiguratorNames.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName");
            ViewBag.ConfigData = new SelectList(db.ComplexLookups.Select(x => x.ConfigData).Distinct());

            if (String.IsNullOrEmpty(ConfigNames) && String.IsNullOrEmpty(ConfigData))
            {
                return View();
            }
            else
            {
                var complexLookups = db.ComplexLookups.AsQueryable();
                if (!String.IsNullOrEmpty(ConfigNames)) complexLookups = complexLookups.Where(x => x.ConfigName == ConfigNames);
                if (!String.IsNullOrEmpty(ConfigData)) complexLookups = complexLookups.Where(x => x.ConfigData == ConfigData);

                return View(complexLookups.OrderBy(x => x.ConfigName).ThenBy(x => x.ConfigData).ToList());
            }
        }

        public ActionResult ComplexLinks(int? id)
        {
            // id = ComplexStructure.Id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var complexLinks = db.ComplexLinks.Where(x => x.ComplexDataId == id);

            if (complexLinks == null)
            {
                return HttpNotFound();
            }

            return View();
        }

        public ActionResult _ComplexLinkMatrix(int? id)
        {
            // id = ComplexStructure.Id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var complexLinks = db.ComplexLinks.Where(x => x.ComplexDataId == id);

            if (complexLinks == null)
            {
                return HttpNotFound();
            }

            return View();
        }

        private void SaveOrderOptionMatrix(int id, List<string> matrix)
        {
            //var details = db.OrderDetails
            //    .Include(x => x.OrderDetailsOrderOptionsLinks)
            //    .Where(x => x.OrderHeaderId == id);

            //foreach (var orderDetail in details.OrderBy(x => x.Id))
            //{
            //    foreach (var source in orderDetail.OrderDetailsOrderOptionsLinks.OrderBy(x => x.OrderOptionId))
            //    {
            //        string lookup = String.Format("{0}, {1}", source.OrderDetailId, source.OrderOptionId);
            //        if (matrix != null && matrix.Contains(lookup))
            //        {
            //            source.Include = true;
            //        }
            //        else
            //        {
            //            source.Include = false;
            //        }
            //    }
            //}

            //db.SaveChanges();
        }

        // GET: ComplexLookups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComplexLookup complexLookup = db.ComplexLookups.Find(id);
            if (complexLookup == null)
            {
                return HttpNotFound();
            }
            return View(complexLookup);
        }

        // GET: ComplexLookups/Create
        public ActionResult Create()
        {
            GenerateDropDowns();
            return View();
        }

        // POST: ComplexLookups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] ComplexLookup complexLookup)
        {
            var Configs = db.ComplexLookups.FirstOrDefault(x => x.ConfigName == complexLookup.ConfigName && x.ConfigData == complexLookup.ConfigData
            && x.LookupData == complexLookup.LookupData);

            if (Configs != null) ModelState.AddModelError("", "Duplicate Option Created---Please Recheck Data");

            if (ModelState.IsValid)
            {
                db.ComplexLookups.Add(complexLookup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(complexLookup);
            return View(complexLookup);
        }

        // GET: ComplexLookups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComplexLookup complexLookup = db.ComplexLookups.Find(id);
            if (complexLookup == null)
            {
                return HttpNotFound();
            }
            GenerateDropDowns();
            return View(complexLookup);
        }

        // POST: ComplexLookups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ComplexLookup complexLookup)
        {
            var Configs = db.ComplexLookups.FirstOrDefault(x => x.ConfigName == complexLookup.ConfigName && x.ConfigData == complexLookup.ConfigData && x.LookupData == complexLookup.LookupData
            && x.Id != complexLookup.Id);

            if (Configs != null) ModelState.AddModelError("", "Duplicate Option Created---Please Recheck Data");

            if (ModelState.IsValid)
            {
                db.Entry(complexLookup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(complexLookup);
            return View(complexLookup);
        }

        // GET: ComplexLookups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComplexLookup complexLookup = db.ComplexLookups.Find(id);
            if (complexLookup == null)
            {
                return HttpNotFound();
            }
            return View(complexLookup);
        }

        // POST: ComplexLookups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ComplexLookup complexLookup = db.ComplexLookups.Find(id);
            db.ComplexLookups.Remove(complexLookup);
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
            var ConfigNameList = from firstList in db.ComplexLookups
                                 group firstList by firstList.ConfigName into newList1
                                 let x = newList1.FirstOrDefault()
                                 select x;

            var ConfigDataList = from secondList in db.ComplexLookups
                                 group secondList by secondList.ConfigData into newList2
                                 let x = newList2.FirstOrDefault()
                                 select x;

            var LookupDataList = from thirdList in db.ComplexLookups
                                 group thirdList by thirdList.LookupData into newList3
                                 let x = newList3.FirstOrDefault()
                                 select x;

            ViewBag.ConfigName = new SelectList(ConfigNameList.ToList(), "ConfigName", "ConfigName");
            ViewBag.ConfigData = new SelectList(ConfigDataList.ToList(), "ConfigData", "ConfigData");
            ViewBag.LookupData = new SelectList(LookupDataList.ToList(), "LookupData", "LookupData");
        }

        private void GenerateDropDowns(ComplexLookup complexLookup)
        {
            ViewBag.ConfigName = new SelectList(db.ComplexLookups.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName", complexLookup.ConfigName);
            ViewBag.ConfigData = new SelectList(db.ComplexLookups.OrderBy(x => x.ConfigData), "ConfigData", "ConfigData", complexLookup.ConfigData);
            ViewBag.LookupData = new SelectList(db.ComplexLookups.OrderBy(x => x.LookupData), "LookupData", "LookupData", complexLookup.LookupData);
        }
    }
}