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
using Time.Configurator.Helpers;
using Time.Configurator.Models;
using Time.Configurator.Services;
using Time.Configurator.ViewModels;
using Time.Data.EntityModels.Configurator;

namespace Time.Configurator.Controllers
{
    //sets theme and requires you to log in to go to the page
    [Themed]
    [Authorize]
    public class ConfigPricingController : Controller
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        private IConfigPricingExportExcelHelper _cfgPrExEcHp = new ConfigPricingExportExcelHelper();

        // These lists will hold the values for the ImportPricing method. They are also used by the SplitConfigOption() method.
        public List<decimal> price = new List<decimal>();

        public List<decimal> altPrice = new List<decimal>();
        public List<string> option = new List<string>();
        public bool altPriceEmpty = false; // To insert the Price in AltPrice if no AltPrice provided
        public bool changeAltPrice = false; // To update AltPrice if it is provided

        public ConfigPricingController(IOrchardServices services)
        {
            Services = services;
            db = new ConfiguratorEntities();
        }

        public ConfigPricingController(IOrchardServices services, ConfiguratorEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: /ConfigPricing/
        public ActionResult Index(string ConfigNames, string ConfigOptions)
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to View this Page")))
                return new HttpUnauthorizedResult();
            ViewBag.ConfigNames = new SelectList(db.ConfiguratorNames.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName");
            ViewBag.ConfigOptions = new SelectList(db.ConfigPricings.Select(x => new { x.ConfigOption }).Distinct().OrderBy(x => x.ConfigOption), "ConfigOption", "ConfigOption");

            if (String.IsNullOrEmpty(ConfigNames) && String.IsNullOrEmpty(ConfigOptions))
            {
                return View();
            }
            else
            {
                var configP = db.ConfigPricings.AsQueryable();
                if (!String.IsNullOrEmpty(ConfigNames)) configP = configP.Where(x => x.ConfigID == ConfigNames);
                if (!String.IsNullOrEmpty(ConfigOptions))
                {
                    string[] option = ConfigOptions.Split('-');
                    string opt = option[0] + "-";
                    configP = configP.Where(x => x.ConfigOption.Contains(opt));
                }

                return View(configP.OrderBy(x => x.ConfigID).ThenBy(x => x.ConfigOption).ToList());
            }
        }

        // This method allows user to export ConfigPricings for a specific configurator to Excel
        public ActionResult ConfigPricingExport()
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to Export")))
                return new HttpUnauthorizedResult();
            ViewBag.ConfigNames = new SelectList(db.ConfiguratorNames.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName");
            //return View(db.ConfigPricings.OrderBy(x => x.ConfigID).ThenBy(x => x.ConfigOption).ToList());
            return View();
        }

        // This method respond to ConfigPricingExport when a list is exported to Excel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfigPricingExport(string ConfigNames)
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to Export")))
                return new HttpUnauthorizedResult();
            if (String.IsNullOrEmpty(ConfigNames)) ModelState.AddModelError("", "Please select a Configurator for the list.");

            if (ModelState.IsValid)
            {
                var configs = _cfgPrExEcHp.GetConfigPricingForCfgName(ConfigNames).Select(x => new ConfigPricingViewModel
                {
                    option = x.ConfigOption,
                    price = x.Price,
                    altPrice = x.AltPrice,
                }).ToList();

                return new ExporttoExcelResult(ConfigNames + "_Price_List", configs);
            }

            ViewBag.ConfigNames = new SelectList(db.ConfiguratorNames.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName");
            //return View(db.ConfigPricings.OrderBy(x => x.ConfigID).ThenBy(x => x.ConfigOption).ToList());
            return View();
        }

        // GET: /ConfigPricing/Details/5
        public ActionResult Details(string id, string opt)
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to View the Details")))
                return new HttpUnauthorizedResult();
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
            ViewBag.ConfigID = new SelectList(db.ConfiguratorNames.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName");
            return View();
        }

        // POST: /ConfigPricing/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] ConfigPricing configpricing)
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            //prevents a duplicate from being created
            var Configs = db.ConfigPricings.FirstOrDefault(x => x.ConfigID == configpricing.ConfigID && x.ConfigOption == configpricing.ConfigOption);

            //displays if previous code found a duplicate
            if (Configs != null) ModelState.AddModelError("", "Duplicate Pricing Option Created---Please Recheck Data");

            if (ModelState.IsValid)
            {
                db.ConfigPricings.Add(configpricing);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ConfigID = new SelectList(db.ConfiguratorNames.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName");
            return View(configpricing);
        }

        // GET: /ImportPricing
        // Whit this method the user can paste a list of Options, Prices, and Alternate Prices and import them to the db
        public ActionResult ImportPricing()
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            ConfigPriceImportVM coPrImVM = new ConfigPriceImportVM();

            ViewBag.ConfigID = new SelectList(db.ConfiguratorNames.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName");
            return View(coPrImVM);
        }

        // POST: /ImportPricing
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportPricing(ConfigPriceImportVM coPrImVM)
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            if (ModelState.IsValid)
            {
                SplitConfigOption(coPrImVM.OptionAndPrice); // Calling the method to split the string from the view

                //for loop that loops through and inputs each piece of data
                for (int i = 0; i < option.Count(); i++)
                {
                    ConfigPricing newConfigPricing = new ConfigPricing
                    {
                        ConfigID = coPrImVM.ConfigId,
                        ConfigOption = option[i],
                        Price = price[i],
                        AltPrice = (altPriceEmpty) ? price[i] : altPrice[i]
                    };
                    var Configs = db.ConfigPricings.FirstOrDefault(x => x.ConfigID == newConfigPricing.ConfigID && x.ConfigOption == newConfigPricing.ConfigOption);
                    if (Configs == null)
                    {
                        db.ConfigPricings.Add(newConfigPricing);
                    }
                    else
                    {
                        Configs.Price = price[i];
                        if (changeAltPrice) Configs.AltPrice = altPrice[i];
                        db.ConfigPricings.Attach(Configs);
                        var entry = db.Entry(Configs);
                        entry.Property(e => e.Price).IsModified = true;
                        if (changeAltPrice) entry.Property(e => e.AltPrice).IsModified = true;
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index", new { ConfigNames = coPrImVM.ConfigId, ConfigOptions = option[0] });
            }
            ViewBag.ConfigID = new SelectList(db.ConfiguratorNames.OrderBy(x => x.ConfigName), "ConfigName", "ConfigName");
            return View(coPrImVM);
        }

        // This method is called from the ImportPricing method to parse the input in the ConfigOption textbox
        private void SplitConfigOption(string rawString)
        {
            string[] newString = rawString.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            string[] testUpperBound = newString[0].Split(new String[] { "\t", "," }, StringSplitOptions.RemoveEmptyEntries);
            string[] parseString = rawString.Split(new String[] { "\t", ",", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (testUpperBound.Length == 2)
            {
                // Looping through the parseString and assigning the values to the option and price
                for (int i = 0; i < parseString.Count(); i++)
                {
                    if (i == 0 || i % 2 == 0)
                    {
                        option.Add(parseString[i]);
                    }
                    else
                    {
                        price.Add(Convert.ToDecimal(parseString[i]));
                        altPriceEmpty = true;
                    }
                }
            }
            else if (testUpperBound.Length == 3)
            {
                // Looping through the parseString and assigning the values to the option, price, and altPrice lists
                for (int i = 0; i < parseString.Count(); i += 3)
                {
                    option.Add(parseString[i]);
                }
                for (int i = 1; i < parseString.Count(); i += 3)
                {
                    price.Add(Convert.ToDecimal(parseString[i]));
                }
                for (int i = 2; i < parseString.Count(); i += 3)
                {
                    altPrice.Add(Convert.ToDecimal(parseString[i]));
                }
                changeAltPrice = true;
            }
        }

        // GET: /ConfigPricing/Edit/5
        public ActionResult Edit(string id, string opt)
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
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
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            //prevents a duplicate from being saved when editing
            var Configs = db.ConfigPricings.FirstOrDefault(x => x.ConfigID == configpricing.ConfigID && x.ConfigOption == configpricing.ConfigOption && x.Price == configpricing.Price
                && x.AltPrice == configpricing.AltPrice);

            //displays if previous code found a duplicate
            if (Configs != null) ModelState.AddModelError("", "Duplicate Pricing Created---Please Recheck Data");

            if (ModelState.IsValid)
            {
                db.Entry(configpricing).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { ConfigNames = configpricing.ConfigID, ConfigOptions = configpricing.ConfigOption });
            }
            GenerateDropDowns(configpricing);
            return View(configpricing);
        }

        // GET: /ConfigPricing/Delete/5
        public ActionResult Delete(string id, string opt)
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
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
        public ActionResult DeleteConfirmed(string id, string opt)
        {
            if (!Services.Authorizer.Authorize(Permissions.ConfiguratorSales, T("You Do Not Have Permission to Edit")))
                return new HttpUnauthorizedResult();
            ConfigPricing configpricing = db.ConfigPricings.Find(id, opt);
            db.ConfigPricings.Remove(configpricing);
            db.SaveChanges();
            return RedirectToAction("Index", new { ConfigNames = configpricing.ConfigID, ConfigOptions = configpricing.ConfigOption });
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
            //This and above ViewBags pull in the data to put into the drop down lists
            //prevent duplicates from showing up in drop down
            //without var list codes, every CFG and Global shows up in drop down and whatever else for the other drop downs

            //var ConfigOptionList = from secondList in db.ConfigPricings
            //                     group secondList by secondList.ConfigOption into newList2
            //                     let x = newList2.FirstOrDefault()
            //                     select x;

            ViewBag.ConfigID = new SelectList(db.ConfiguratorNames.OrderBy(x => x.ConfigName), "Id", "ConfigName");
            //ViewBag.ConfigOption = new SelectList(db.ConfigPricings.OrderBy(x => x.ConfigOption), "ConfigOption", "ConfigOption");
        }

        //This and above ViewBags pull in the data to put into the drop down lists
        private void GenerateDropDowns(ConfigPricing configpricing)
        {
            ViewBag.ConfigID = new SelectList(db.ConfiguratorNames.OrderBy(x => x.ConfigName), "Id", "ConfigName", configpricing.ConfigID);
            //ViewBag.ConfigOption = new SelectList(db.ConfigPricings.OrderBy(x => x.ConfigOption), "ConfigOption", "ConfigOption", configpricing.ConfigOption);
        }
    }
}