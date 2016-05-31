using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.Install;
using Time.Install.Models;
using Time.Install.ViewModels;

namespace Time.Install.Controllers
{
    [Themed]
    public class VSWOptionsController : Controller
    {
        private VSWQuotesEntities db = new VSWQuotesEntities();

        // GET: VSWOptions
        public ActionResult Index(int liftFamilyId = 0, int groupId = 0)
        {
            ViewBag.GroupId = new SelectList(db.OptionGroups.OrderBy(x => x.GroupName), "Id", "GroupName");
            ViewBag.LiftFamilyId = new SelectList(db.LiftFamilies.OrderBy(x => x.FamilyName), "Id", "FamilyName");

            if (liftFamilyId == 0 && groupId == 0)
            {
                return View();
            }
            else
            {
                var installOptions = db.VSWOptions.AsQueryable();
                if (liftFamilyId != 0) { installOptions = installOptions.Where(x => x.LiftFamilyId == liftFamilyId); }
                if (groupId != 0) { installOptions = installOptions.Where(x => x.GroupId == groupId); }
                return View(installOptions.OrderBy(x => x.GroupId).ToList());
            }
            //var vSWOptions = db.VSWOptions.Include(v => v.LiftFamily).Include(v => v.OptionGroup);
            //return View(vSWOptions.ToList());
        }

        // GET: VSWOptions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VSWOption vSWOption = db.VSWOptions.Find(id);
            if (vSWOption == null)
            {
                return HttpNotFound();
            }
            return View(vSWOption);
        }

        // GET: VSWOptions/Create
        public ActionResult Create()
        {
            ViewBag.LiftFamilyId = new SelectList(db.LiftFamilies, "Id", "FamilyName");
            ViewBag.GroupId = new SelectList(db.OptionGroups, "Id", "GroupName");
            return View();
        }

        // POST: VSWOptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")]  VSWOption vSWOption)
        {
            if (ModelState.IsValid)
            {
                db.VSWOptions.Add(vSWOption);
                db.SaveChanges();
                return RedirectToAction("Index", new { liftFamilyId = vSWOption.LiftFamilyId, groupId = vSWOption.GroupId });
            }

            ViewBag.LiftFamilyId = new SelectList(db.LiftFamilies, "Id", "FamilyName", vSWOption.LiftFamilyId);
            ViewBag.GroupId = new SelectList(db.OptionGroups, "Id", "GroupName", vSWOption.GroupId);
            return View(vSWOption);
        }

        // GET: VSWOptions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VSWOption vSWOption = db.VSWOptions.Find(id);
            if (vSWOption == null)
            {
                return HttpNotFound();
            }
            ViewBag.LiftFamilyId = new SelectList(db.LiftFamilies, "Id", "FamilyName", vSWOption.LiftFamilyId);
            ViewBag.GroupId = new SelectList(db.OptionGroups, "Id", "GroupName", vSWOption.GroupId);
            return View(vSWOption);
        }

        // POST: VSWOptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(VSWOption vSWOption)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vSWOption).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { liftFamilyId = vSWOption.LiftFamilyId, groupId = vSWOption.GroupId });
            }
            ViewBag.LiftFamilyId = new SelectList(db.LiftFamilies, "Id", "FamilyName", vSWOption.LiftFamilyId);
            ViewBag.GroupId = new SelectList(db.OptionGroups, "Id", "GroupName", vSWOption.GroupId);
            return View(vSWOption);
        }

        // GET: VSWOptions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VSWOption vSWOption = db.VSWOptions.Find(id);
            if (vSWOption == null)
            {
                return HttpNotFound();
            }
            return View(vSWOption);
        }

        // POST: VSWOptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VSWOption vSWOption = db.VSWOptions.Find(id);
            db.VSWOptions.Remove(vSWOption);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Adding options to Groups
        [HttpGet]
        public ActionResult AddOptions()
        {
            ViewBag.GroupId = new SelectList(db.OptionGroups.OrderBy(x => x.GroupName), "Id", "GroupName");
            ViewBag.LiftFamilyId = new SelectList(db.LiftFamilies.OrderBy(x => x.FamilyName), "Id", "FamilyName");

            return View();
        }

        // Adding options to Groups
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOptions(AddOptionsViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var groupName = db.OptionGroups.SingleOrDefault(x => x.Id == vm.GroupId); // To set the paint flag
                bool paint = false;
                if (groupName.GroupName.Contains("PAINT")) paint = true;
                var splittedOptions = SplitOptions(vm.OptionsToParse);// Parsing the options

                for (int i = 0; i < splittedOptions.Count(); i++)
                {
                    VSWOption newOption = new VSWOption
                    {
                        LiftFamilyId = vm.LiftFamilyId,
                        GroupId = vm.GroupId,
                        OptionName = splittedOptions[i].Option,
                        Price = splittedOptions[i].Price,
                        LaborHours = Convert.ToDecimal(splittedOptions[i].Hours),
                        PaintFlag = paint
                    };
                    var optionExists = db.VSWOptions.FirstOrDefault(x => x.LiftFamilyId == newOption.LiftFamilyId && x.GroupId == newOption.GroupId
                                                                 && x.OptionName == newOption.OptionName && x.Price == newOption.Price
                                                                 && x.LaborHours == newOption.LaborHours);
                    if (optionExists == null)
                    {
                        db.VSWOptions.Add(newOption); // Adding a new option to group
                    }
                    else
                    {
                        // Updating price or hours in an existing option
                        optionExists.Price = splittedOptions[i].Price;
                        optionExists.LaborHours = Convert.ToDecimal(splittedOptions[i].Hours);
                        db.VSWOptions.Attach(optionExists);
                        var entry = db.Entry(optionExists);
                        entry.Property(e => e.Price).IsModified = true;
                        entry.Property(e => e.LaborHours).IsModified = true;
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index", new { liftFamilyId = vm.LiftFamilyId, groupId = vm.GroupId });
            }
            ViewBag.GroupId = new SelectList(db.OptionGroups.OrderBy(x => x.GroupName), "Id", "GroupName");
            ViewBag.LiftFamilyId = new SelectList(db.LiftFamilies.OrderBy(x => x.FamilyName), "Id", "FamilyName");

            return View(vm);
        }

        // Splitting options imported from an Excel file
        private List<SplitOptions> SplitOptions(string stringToSplit)
        {
            var options = new List<SplitOptions>();
            string[] parseString = stringToSplit.Split(new String[] { "\t", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            // Looping through the parseString and assigning the values to the option, price, and altPrice lists
            for (int i = 0; i < (parseString.Count()); i += 3)
            {
                var sO = new SplitOptions();
                sO.Option = parseString[i];
                sO.Price = Convert.ToDecimal(parseString[i + 1]);
                sO.Hours = Convert.ToDouble(parseString[i + 2]);
                options.Add(sO);
            }

            return options;
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
