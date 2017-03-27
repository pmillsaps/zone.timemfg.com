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
    public class FormattingsController : Controller
    {
        private CustomManualsEntities db = new CustomManualsEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public FormattingsController(IOrchardServices services)
        {
            Services = services;
            db = new CustomManualsEntities();
        }

        public FormattingsController(IOrchardServices services, CustomManualsEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: Formattings
        public ActionResult Index()
        {
            var formattings = db.Formattings.Include(f => f.LiftGroup);
            return View(formattings.OrderBy(x => x.Lift_Group).ToList());
        }

        // GET: Formattings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Formatting formatting = db.Formattings.Find(id);
            if (formatting == null)
            {
                return HttpNotFound();
            }
            return View(formatting);
        }

        //// Not used
        //// GET: Formattings/Create
        //public ActionResult Create()
        //{
        //    ViewBag.Lift_Group = new SelectList(db.LiftGroups, "Lift_Group", "Lift_Group");
        //    return View();
        //}

        //// POST: Formattings/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Formatting_ID,Lift_Group,Sequence,Section,PullsFrom,Title,EmptyPageL,EmptyPageR,BlankPage,SectionStarts,PageNumbers,BlankPageMsg,CheckFacings,Active")] Formatting formatting)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Formattings.Add(formatting);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.Lift_Group = new SelectList(db.LiftGroups, "Lift_Group", "Lift_Group", formatting.Lift_Group);
        //    return View(formatting);
        //}

        // GET: Formattings/Edit/5
        public ActionResult Edit(int? id)
        {
            //if (!Services.Authorizer.Authorize(Permissions.CustomManualsAdmin, T("You Do Not Have Permission to View this Page")))
            //    return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Formatting formatting = db.Formattings.Find(id);
            if (formatting == null)
            {
                return HttpNotFound();
            }
            ViewBag.Lift_Group = new SelectList(db.LiftGroups, "Lift_Group", "Lift_Group", formatting.Lift_Group);
            return View(formatting);
        }

        // POST: Formattings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Formatting_ID,Lift_Group,Sequence,Section,PullsFrom,Title,EmptyPageL,EmptyPageR,BlankPage,SectionStarts,PageNumbers,BlankPageMsg,CheckFacings,Active")] Formatting formatting)
        {
            //if (!Services.Authorizer.Authorize(Permissions.CustomManualsAdmin, T("You Do Not Have Permission to View this Page")))
            //    return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
                db.Entry(formatting).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Lift_Group = new SelectList(db.LiftGroups, "Lift_Group", "Lift_Group", formatting.Lift_Group);
            return View(formatting);
        }

        // GET: Formattings/Delete/5
        public ActionResult Delete(int? id)
        {
            //if (!Services.Authorizer.Authorize(Permissions.CustomManualsAdmin, T("You Do Not Have Permission to View this Page")))
            //    return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Formatting formatting = db.Formattings.Find(id);
            if (formatting == null)
            {
                return HttpNotFound();
            }
            return View(formatting);
        }

        // POST: Formattings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //if (!Services.Authorizer.Authorize(Permissions.CustomManualsAdmin, T("You Do Not Have Permission to View this Page")))
            //    return new HttpUnauthorizedResult();

            Formatting formatting = db.Formattings.Find(id);
            db.Formattings.Remove(formatting);
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
