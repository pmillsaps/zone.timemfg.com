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
    public class ConfigPricingController : Controller
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        // GET: /ConfigPricing/
        public ActionResult Index()
        {
            return View(db.ConfigPricings.OrderBy(x => x.ConfigID).ThenBy(x => x.ConfigOption).ToList());
        }

        // GET: /ConfigPricing/Details/5
        public ActionResult Details(string id, string opt)
        {
            if (id == null && opt == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigPricing configpricing = db.ConfigPricings.Find(id, opt);
            if (configpricing == null)
            {
                return HttpNotFound();
            }
            return View(configpricing);
        }

        // GET: /ConfigPricing/Create
        public ActionResult Create()
        {
            GenerateDropDowns();
            return View();
        }

        // POST: /ConfigPricing/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Exclude="ConfigID,ConfigOption")] ConfigPricing configpricing)
            public ActionResult Create(ConfigPricing configpricing)
        {
            var Configs = db.ConfigPricings.FirstOrDefault(x => x.ConfigID == configpricing.ConfigID && x.ConfigOption == configpricing.ConfigOption && x.Price == configpricing.Price
                && x.AltPrice == configpricing.AltPrice);

            if (Configs != null) ModelState.AddModelError("", "Duplicate Pricing Created---Please Recheck Inputed Data");

            if (ModelState.IsValid)
            {
                db.ConfigPricings.Add(configpricing);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(configpricing);
            return View(configpricing);
        }

        // GET: /ConfigPricing/Edit/5
        public ActionResult Edit(string id, string opt)
        {
            if (id == null && opt == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigPricing configpricing = db.ConfigPricings.Find(id, opt);
            if (configpricing == null)
            {
                return HttpNotFound();
            }
            GenerateDropDowns();
            return View(configpricing);
        }

        // POST: /ConfigPricing/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ConfigPricing configpricing)
        {
            var Configs = db.ConfigPricings.FirstOrDefault(x => x.ConfigID == configpricing.ConfigID && x.ConfigOption == configpricing.ConfigOption && x.Price == configpricing.Price 
                && x.AltPrice == configpricing.AltPrice);

            if (Configs != null) ModelState.AddModelError("", "Duplicate Pricing Created---Please Recheck Inputed Data");

            if (ModelState.IsValid)
            {
                db.Entry(configpricing).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(configpricing);
            return View(configpricing);
        }

        // GET: /ConfigPricing/Delete/5
        public ActionResult Delete(string id, string opt)
        {
            if (id == null && opt == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfigPricing configpricing = db.ConfigPricings.Find(id, opt);
            if (configpricing == null)
            {
                return HttpNotFound();
            }
            return View(configpricing);
        }

        // POST: /ConfigPricing/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ConfigPricing configpricing = db.ConfigPricings.Find(id);
            db.ConfigPricings.Remove(configpricing);
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
            var ConfigIDList = from firstList in db.ConfigPricings
                                 group firstList by firstList.ConfigID into newList1
                                 let x = newList1.FirstOrDefault()
                                 select x;

            var ConfigOptionList = from secondList in db.ConfigPricings
                                 group secondList by secondList.ConfigOption into newList2
                                 let x = newList2.FirstOrDefault()
                                 select x;

            ViewBag.ConfigID = new SelectList(ConfigIDList.ToList(), "ConfigID", "ConfigID");
            ViewBag.ConfigOption = new SelectList(ConfigOptionList.ToList(), "ConfigOption", "ConfigOption");
        }
        
        private void GenerateDropDowns(ConfigPricing configpricing)
        {
            ViewBag.ConfigID = new SelectList(db.ConfigPricings.OrderBy(x => x.ConfigID), "ConfigID", "ConfigID");
            ViewBag.ConfigOption = new SelectList(db.ConfigPricings.OrderBy(x => x.ConfigOption), "ConfigOption", "ConfigOption");
        }
    }
}
