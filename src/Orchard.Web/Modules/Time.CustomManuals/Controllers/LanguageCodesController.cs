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
using Time.Data.EntityModels.CustomManuals;

namespace Time.CustomManuals.Controllers
{
    //sets theme and requires you to log in to go to the page
    [Themed]
    [Authorize]
    public class LanguageCodesController : Controller
    {
        private CustomManualsEntities db = new CustomManualsEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public LanguageCodesController(IOrchardServices services)
        {
            Services = services;
            db = new CustomManualsEntities();
        }

        public LanguageCodesController(IOrchardServices services, CustomManualsEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: LanguageCodes
        public ActionResult Index()
        {
            return View(db.LanguageCodes.ToList());
        }

        ////Not used
        //// GET: LanguageCodes/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    LanguageCode languageCode = db.LanguageCodes.Find(id);
        //    if (languageCode == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(languageCode);
        //}

        // GET: LanguageCodes/Create
        public ActionResult Create()
        {
            //if (!Services.Authorizer.Authorize(Permissions.CustomManualsAdmin, T("You Do Not Have Permission to View this Page")))
            //    return new HttpUnauthorizedResult();

            return View();
        }

        // POST: LanguageCodes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Language_ID,Language_Code,Language")] LanguageCode languageCode)
        {
            //if (!Services.Authorizer.Authorize(Permissions.CustomManualsAdmin, T("You Do Not Have Permission to View this Page")))
            //    return new HttpUnauthorizedResult();

            var dupeCheck = db.LanguageCodes.FirstOrDefault(x => x.Language_Code == languageCode.Language_Code);
            if (dupeCheck != null) ModelState.AddModelError("", "Duplicate Language Codes are not Allowed...");

            if (ModelState.IsValid)
            {
                db.LanguageCodes.Add(languageCode);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(languageCode);
        }

        // GET: LanguageCodes/Edit/5
        public ActionResult Edit(int? id)
        {
            //if (!Services.Authorizer.Authorize(Permissions.CustomManualsAdmin, T("You Do Not Have Permission to View this Page")))
            //    return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LanguageCode languageCode = db.LanguageCodes.Find(id);
            if (languageCode == null)
            {
                return HttpNotFound();
            }
            return View(languageCode);
        }

        // POST: LanguageCodes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Language_ID,Language_Code,Language")] LanguageCode languageCode)
        {
            //if (!Services.Authorizer.Authorize(Permissions.CustomManualsAdmin, T("You Do Not Have Permission to View this Page")))
            //    return new HttpUnauthorizedResult();

            var dupeCheck = db.LanguageCodes.FirstOrDefault(x => x.Language_Code == languageCode.Language_Code && x.Language == languageCode.Language && x.Language_ID != languageCode.Language_ID);
            if (dupeCheck != null) ModelState.AddModelError("", "Duplicate Language Codes are not Allowed...");

            if (ModelState.IsValid)
            {
                db.Entry(languageCode).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(languageCode);
        }

        // GET: LanguageCodes/Delete/5
        public ActionResult Delete(int? id)
        {
            //if (!Services.Authorizer.Authorize(Permissions.CustomManualsAdmin, T("You Do Not Have Permission to View this Page")))
            //    return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LanguageCode languageCode = db.LanguageCodes.Find(id);
            if (languageCode == null)
            {
                return HttpNotFound();
            }
            return View(languageCode);
        }

        // POST: LanguageCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //if (!Services.Authorizer.Authorize(Permissions.CustomManualsAdmin, T("You Do Not Have Permission to View this Page")))
            //    return new HttpUnauthorizedResult();

            LanguageCode languageCode = db.LanguageCodes.Find(id);
            db.LanguageCodes.Remove(languageCode);
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
