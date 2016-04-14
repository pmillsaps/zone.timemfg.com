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
using Time.CustomManuals.Models;
using Time.Data.EntityModels.CustomManuals;

namespace Time.CustomManuals.Controllers
{
    //sets theme and requires you to log in to go to the page
    [Themed]
    [Authorize]
    public class LiftGroupsController : Controller
    {
        private CustomManualsEntities db = new CustomManualsEntities();

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public LiftGroupsController(IOrchardServices services)
        {
            Services = services;
            db = new CustomManualsEntities();
        }

        public LiftGroupsController(IOrchardServices services, CustomManualsEntities _db)
        {
            Services = services;
            db = _db;
        }

        // GET: LiftGroups
        public ActionResult Index()
        {
            return View(db.LiftGroups.ToList());
        }

        //// Not used
        //// GET: LiftGroups/Details/5
        //public ActionResult Details(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    LiftGroup liftGroup = db.LiftGroups.Find(id);
        //    if (liftGroup == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(liftGroup);
        //}

        // GET: LiftGroups/Create
        public ActionResult Create()
        {
            //if (!Services.Authorizer.Authorize(Permissions.CustomManualsAdmin, T("You Do Not Have Permission to View this Page")))
            //    return new HttpUnauthorizedResult();

            return View();
        }

        // POST: LiftGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Lift_Group")] LiftGroup liftGroup)
        {
            //if (!Services.Authorizer.Authorize(Permissions.CustomManualsAdmin, T("You Do Not Have Permission to View this Page")))
            //    return new HttpUnauthorizedResult();

            var dupeCheck = db.LiftGroups.FirstOrDefault(x => x.Lift_Group == liftGroup.Lift_Group);
            if (dupeCheck != null) ModelState.AddModelError("", "Duplicate Lift Groups are not Allowed...");

            if (ModelState.IsValid)
            {
                db.LiftGroups.Add(liftGroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(liftGroup);
        }

        // GET: CustomManual/FormattingCopy
        public ActionResult FormattingCopy(string id)
        {
            //if (!Services.Authorizer.Authorize(Permissions.CustomManualsAdmin, T("You Do Not Have Permission to View this Page")))
            //    return new HttpUnauthorizedResult();

            ViewBag.Lift_Group = new SelectList(db.Formattings, "Lift_Group", "Lift_Group");
            ViewBag.LiftGroup = id;
            var vm = new FormattingCopyVM();
            vm.LiftGroup = id;

            return View(vm);
        }

        // POST: CustomManual/FormattingCopy
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FormattingCopy(FormattingCopyVM vm)
        {
            //if (!Services.Authorizer.Authorize(Permissions.CustomManualsAdmin, T("You Do Not Have Permission to View this Page")))
            //    return new HttpUnauthorizedResult();

            var dupeCheck = db.Formattings.Where(x => x.Lift_Group == vm.LiftGroup);
            if (dupeCheck.Count() > 0) ModelState.AddModelError("", "There are already Lift Group Formatting entries...");
            dupeCheck = db.Formattings.Where(x => x.Lift_Group == vm.Lift_Group);
            if (dupeCheck.Count() == 0) ModelState.AddModelError("", "You must select a source Lift Group...");

            if (ModelState.IsValid)
            {
                foreach (var item in dupeCheck.ToList())
                {
                    var newItem = new Formatting
                    {
                        Active = true,
                        BlankPage = item.BlankPage,
                        BlankPageMsg = item.BlankPageMsg,
                        CheckFacings = item.CheckFacings,
                        EmptyPageL = item.EmptyPageL,
                        EmptyPageR = item.EmptyPageR,
                        Lift_Group = vm.LiftGroup,
                        PageNumbers = item.PageNumbers,
                        PullsFrom = item.PullsFrom,
                        Section = item.Section,
                        SectionStarts = item.SectionStarts,
                        Sequence = item.Sequence,
                        Title = item.Title
                    };

                    db.Formattings.Add(newItem);
                }

                db.SaveChanges();
                //logger.Info(String.Format("User '{0}' copied {3} Lift Formatting Entries from {1} to {2}", HttpContext.User.Identity.Name, vm.Lift_Group, vm.LiftGroup, dupeCheck.Count()));
                return RedirectToAction("Index");
            }
            ViewBag.Lift_Group = new SelectList(db.Formattings, "Lift_Group", "Lift_Group");
            return View(vm);
        }

        ////Not used
        //// GET: LiftGroups/Edit/5
        //public ActionResult Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    LiftGroup liftGroup = db.LiftGroups.Find(id);
        //    if (liftGroup == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(liftGroup);
        //}

        //// POST: LiftGroups/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Lift_Group")] LiftGroup liftGroup)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(liftGroup).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(liftGroup);
        //}

        // GET: LiftGroups/Delete/5
        public ActionResult Delete(string id)
        {
            //if (!Services.Authorizer.Authorize(Permissions.CustomManualsAdmin, T("You Do Not Have Permission to View this Page")))
            //    return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiftGroup liftGroup = db.LiftGroups.Find(id);
            if (liftGroup == null)
            {
                return HttpNotFound();
            }

            if (liftGroup.Lifts.Count() > 0 || liftGroup.Formattings.Count() > 0)
            {
                return View("DeleteNotAllowed", liftGroup);
            }

            return View(liftGroup);
        }

        // POST: LiftGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            //if (!Services.Authorizer.Authorize(Permissions.CustomManualsAdmin, T("You Do Not Have Permission to View this Page")))
            //    return new HttpUnauthorizedResult();

            LiftGroup liftGroup = db.LiftGroups.Find(id);
            db.LiftGroups.Remove(liftGroup);
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
