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
using Time.Data.EntityModels.Install;
using Time.Install.ViewModels;

namespace Time.Install.Controllers
{
    [Themed]
    [Authorize]
    public class OptionTitleDescpForWordDocsController : Controller
    {
        private string ErrorMessage { get; set; }
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public OptionTitleDescpForWordDocsController(IOrchardServices services)
        {
            Services = services;
        }
        private VSWQuotesEntities db = new VSWQuotesEntities();

        // GET: OptionTitleDescpForWordDocs
        public ActionResult Index(string FamilyName, string OptionName)
        {
            ViewBag.FamilyName = new SelectList(db.LiftFamilies.OrderBy(x => x.FamilyName), "Id", "FamilyName");
            return View();
        }

        // This method retrieves the Options and Descriptions and appends them to the Index view. It's called from the Index view.
        public JsonResult LoadOptions(string FamilyName, string OptionName)
        {
            List<OptionTitlesAndDescrVM> model = new List<OptionTitlesAndDescrVM>();
            if (String.IsNullOrEmpty(FamilyName) && String.IsNullOrEmpty(OptionName))
            {
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var opts = db.OptionTitleDescpForWordDocs.Include(o => o.LiftFamily).Include(o => o.OptionTitlesForWordDoc);
                int liftFmlyId = Convert.ToInt32(FamilyName);
                if (!String.IsNullOrEmpty(FamilyName)) opts = opts.Where(x => x.LiftFamily.Id == liftFmlyId);
                if (OptionName != "Select Family first" && !String.IsNullOrEmpty(OptionName)) opts = opts.Where(x => x.OptionTitlesForWordDoc.OptionTitle == OptionName);
                foreach (var item in opts)
                {
                    model.Add(new OptionTitlesAndDescrVM
                    {
                        Id = item.Id,
                        FamilyName = item.LiftFamily.FamilyName,
                        OptionName = item.OptionTitlesForWordDoc.OptionTitle,
                        Description = item.Description
                    });
                }
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        // This method generates the drop down options for the OptionName. It's called from the Index view.
        public JsonResult OptionsNamesDropDown(string FamilyName)
        {
            int liftFmlyId = Convert.ToInt32(FamilyName);
            var result = new SelectList(db.OptionTitlesForWordDocs.Where(x => x.LiftFamilyId == liftFmlyId), "OptionTitle", "OptionTitle");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: OptionTitleDescpForWordDocs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OptionTitleDescpForWordDoc optionTitleDescpForWordDoc = db.OptionTitleDescpForWordDocs.Find(id);
            if (optionTitleDescpForWordDoc == null)
            {
                return HttpNotFound();
            }
            return View(optionTitleDescpForWordDoc);
        }

        // GET: OptionTitleDescpForWordDocs/Create
        public ActionResult Create()
        {
            ViewBag.LiftFamilyId = new SelectList(db.LiftFamilies, "Id", "FamilyName");
            return View();
        }

        // POST: OptionTitleDescpForWordDocs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] OptionTitlesAndDescrVM model)
        {
            var alreadyExists = db.OptionTitlesForWordDocs.FirstOrDefault(x => x.OptionTitle == model.OptionName);
            if (alreadyExists != null) ModelState.AddModelError("", "Option Name Already Exists!");

            if (ModelState.IsValid)
            {
                OptionTitlesForWordDoc newTitle = new OptionTitlesForWordDoc // Saving the Option Title
                {
                    LiftFamilyId = model.LiftFamilyId,
                    OptionTitle = model.OptionName
                };
                db.OptionTitlesForWordDocs.Add(newTitle);
                db.SaveChanges();
                var title = db.OptionTitlesForWordDocs.Include(c => c.LiftFamily).OrderByDescending(x => x.Id).FirstOrDefault();
                OptionTitleDescpForWordDoc newDescr = new OptionTitleDescpForWordDoc // Saving the Option Description
                {
                    LiftFamilyId = model.LiftFamilyId,
                    OptionTitlesId = title.Id,
                    Description = model.Description
                };
                db.OptionTitleDescpForWordDocs.Add(newDescr);
                db.SaveChanges();
                return RedirectToAction("Index", new { FamilyName = title.LiftFamily.FamilyName, OptionName = title.OptionTitle });
            }

            ViewBag.LiftFamilyId = new SelectList(db.LiftFamilies, "Id", "FamilyName", model.LiftFamilyId);
            return View(model);
        }

        // GET: OptionTitleDescpForWordDocs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OptionTitleDescpForWordDoc optionTitleDescpForWordDoc = db.OptionTitleDescpForWordDocs.Find(id);
            if (optionTitleDescpForWordDoc == null)
            {
                return HttpNotFound();
            }
            OptionTitlesAndDescrVM model = new OptionTitlesAndDescrVM
            {
                Id = optionTitleDescpForWordDoc.Id,
                FamilyName = optionTitleDescpForWordDoc.LiftFamily.FamilyName,
                OptionTitlesId = optionTitleDescpForWordDoc.OptionTitlesId,
                OptionName = optionTitleDescpForWordDoc.OptionTitlesForWordDoc.OptionTitle,
                Description = optionTitleDescpForWordDoc.Description
            };
            ViewBag.LiftFamilyId = new SelectList(db.LiftFamilies, "Id", "FamilyName", optionTitleDescpForWordDoc.LiftFamilyId);
            return View(model);
        }

        // POST: OptionTitleDescpForWordDocs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OptionTitlesAndDescrVM model)
        {
            var alreadyExists = db.OptionTitlesForWordDocs.FirstOrDefault(x => x.OptionTitle == model.OptionName);
            if (alreadyExists != null) ModelState.AddModelError("", "Option Name Already Exists!");

            if (ModelState.IsValid)
            {
                OptionTitlesForWordDoc editTitle = new OptionTitlesForWordDoc // Saving the Option Title
                {
                    Id = model.OptionTitlesId,
                    LiftFamilyId = model.LiftFamilyId,
                    OptionTitle = model.OptionName
                };
                db.Entry(editTitle).State = EntityState.Modified;
                OptionTitleDescpForWordDoc editDescr = new OptionTitleDescpForWordDoc // Saving the Option Description
                {
                    Id = model.Id,
                    LiftFamilyId = model.LiftFamilyId,
                    OptionTitlesId = model.OptionTitlesId,
                    Description = model.Description
                };
                db.Entry(editDescr).State = EntityState.Modified;
                db.SaveChanges();
                var optn = db.OptionTitlesForWordDocs.Include(c => c.LiftFamily).FirstOrDefault(x => x.Id == model.OptionTitlesId);
                return RedirectToAction("Index", new { FamilyName = optn.LiftFamily.FamilyName, OptionName = optn.OptionTitle });
            }
            ViewBag.LiftFamilyId = new SelectList(db.LiftFamilies, "Id", "FamilyName", model.LiftFamilyId);
            return View(model);
        }

        // GET: OptionTitleDescpForWordDocs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OptionTitleDescpForWordDoc optionTitleDescpForWordDoc = db.OptionTitleDescpForWordDocs.Find(id);
            if (optionTitleDescpForWordDoc == null)
            {
                return HttpNotFound();
            }
            OptionTitlesAndDescrVM model = new OptionTitlesAndDescrVM
            {
                Id = optionTitleDescpForWordDoc.Id,
                LiftFamilyId = optionTitleDescpForWordDoc.LiftFamilyId,
                FamilyName = optionTitleDescpForWordDoc.LiftFamily.FamilyName,
                OptionTitlesId = optionTitleDescpForWordDoc.OptionTitlesId,
                OptionName = optionTitleDescpForWordDoc.OptionTitlesForWordDoc.OptionTitle,
                Description = optionTitleDescpForWordDoc.Description
            };
            return View(model);
        }

        // POST: OptionTitleDescpForWordDocs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OptionTitleDescpForWordDoc optionTitleDescpForWordDoc = db.OptionTitleDescpForWordDocs.Find(id);
            db.OptionTitleDescpForWordDocs.Remove(optionTitleDescpForWordDoc);
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
