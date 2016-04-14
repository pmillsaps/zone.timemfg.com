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
    public class NoPrintsController : Controller
    {
        private CustomManualsEntities db = new CustomManualsEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public NoPrintsController(IOrchardServices services)
        {
            Services = services;
            db = new CustomManualsEntities();
        }

        public NoPrintsController(IOrchardServices services, CustomManualsEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: NoPrints
        public ActionResult Index()
        {
            return View(db.NoPrints.OrderBy(x => x.Part).ToList());
        }

        ////Not used
        //// GET: NoPrints/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    NoPrint noPrint = db.NoPrints.Find(id);
        //    if (noPrint == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(noPrint);
        //}

        // GET: NoPrints/Create
        public ActionResult Create()
        {
            //if (!Services.Authorizer.Authorize(Permissions.CustomManualsAdmin, T("You Do Not Have Permission to View this Page")))
            //    return new HttpUnauthorizedResult();

            return View();
        }

        // POST: NoPrints/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Part")] NoPrint noPrint)
        {
            //if (!Services.Authorizer.Authorize(Permissions.CustomManualsAdmin, T("You Do Not Have Permission to View this Page")))
            //    return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
                db.NoPrints.Add(noPrint);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(noPrint);
        }

        ////Not used
        //// GET: NoPrints/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    NoPrint noPrint = db.NoPrints.Find(id);
        //    if (noPrint == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(noPrint);
        //}

        ////Not used
        //// POST: NoPrints/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,Part")] NoPrint noPrint)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(noPrint).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(noPrint);
        //}

        // GET: NoPrints/Delete/5
        public ActionResult Delete(int? id)
        {
            //if (!Services.Authorizer.Authorize(Permissions.CustomManualsAdmin, T("You Do Not Have Permission to View this Page")))
            //    return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NoPrint noPrint = db.NoPrints.Find(id);
            if (noPrint == null)
            {
                return HttpNotFound();
            }
            return View(noPrint);
        }

        // POST: NoPrints/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //if (!Services.Authorizer.Authorize(Permissions.CustomManualsAdmin, T("You Do Not Have Permission to View this Page")))
            //    return new HttpUnauthorizedResult();

            NoPrint noPrint = db.NoPrints.Find(id);
            db.NoPrints.Remove(noPrint);
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
