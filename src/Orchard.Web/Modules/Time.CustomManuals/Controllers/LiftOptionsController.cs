using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.CustomManuals;

namespace Time.CustomManuals.Controllers
{
    //sets theme and requires you to log in to go to the page
    [Themed]
    [Authorize]
    public class LiftOptionsController : Controller
    {
        private CustomManualsEntities db = new CustomManualsEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public LiftOptionsController(IOrchardServices services)
        {
            Services = services;
            db = new CustomManualsEntities();
        }

        public LiftOptionsController(IOrchardServices services, CustomManualsEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: LiftOptions
        public ActionResult Index(string search = "")
        {
            var parts = db.LiftOptions.Where(x => x.Active == true || x.Active == false);

            if (!String.IsNullOrEmpty(search))
            {
                foreach (var item in search.Split(' '))
                {
                    parts = parts.Where(x => x.OptionNo.Contains(item) || x.ShortTitle.Contains(item));
                }

                var partsfinal = parts.OrderBy(x => x.OptionNo).ToList();

                return View(partsfinal);
            }
            else
            {
                return View();
            }
        }

        // GET: LiftOptions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiftOption liftOption = db.LiftOptions.Find(id);
            if (liftOption == null)
            {
                return HttpNotFound();
            }
            return View(liftOption);
        }

        // GET: LiftOptions/Create
        public ActionResult Create()
        {
            //if (!Services.Authorizer.Authorize(Permissions.CustomManualsAdmin, T("You Do Not Have Permission to View this Page")))
            //    return new HttpUnauthorizedResult();

            return View();
        }

        // POST: LiftOptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,OptionNo,Title,ShortTitle,Active,DistributorViewable")] LiftOption liftOption)
        {
            //if (!Services.Authorizer.Authorize(Permissions.CustomManualsAdmin, T("You Do Not Have Permission to View this Page")))
            //    return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
                db.LiftOptions.Add(liftOption);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(liftOption);
        }

        // GET: LiftOptions/Edit/5
        public ActionResult Edit(int? id)
        {
            //if (!Services.Authorizer.Authorize(Permissions.CustomManualsAdmin, T("You Do Not Have Permission to View this Page")))
            //    return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiftOption liftOption = db.LiftOptions.Find(id);
            if (liftOption == null)
            {
                return HttpNotFound();
            }
            return View(liftOption);
        }

        // POST: LiftOptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OptionNo,Title,ShortTitle,Active,DistributorViewable")] LiftOption liftOption)
        {
            //if (!Services.Authorizer.Authorize(Permissions.CustomManualsAdmin, T("You Do Not Have Permission to View this Page")))
            //    return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
                db.Entry(liftOption).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(liftOption);
        }

        // GET: LiftOptions/Delete/5
        public ActionResult Delete(int? id)
        {
            //if (!Services.Authorizer.Authorize(Permissions.CustomManualsAdmin, T("You Do Not Have Permission to View this Page")))
            //    return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiftOption liftOption = db.LiftOptions.Find(id);
            if (liftOption == null)
            {
                return HttpNotFound();
            }
            return View(liftOption);
        }

        // POST: LiftOptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //if (!Services.Authorizer.Authorize(Permissions.CustomManualsAdmin, T("You Do Not Have Permission to View this Page")))
            //    return new HttpUnauthorizedResult();

            LiftOption liftOption = db.LiftOptions.Find(id);
            db.LiftOptions.Remove(liftOption);
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
