using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using PagedList;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Time.Drawings.EntityModels;
using Time.Drawings.ViewModels;

namespace Time.Drawings.Controllers
{
    [Themed]
    [Authorize]
    public class AdditionalController : Controller
    {
        private readonly DrawingsEntities db;

        public IOrchardServices Services { get; set; }

        public Localizer T { get; set; }

        public AdditionalController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
            db = new DrawingsEntities();
        }

        // GET: Additional
        public ActionResult Index(string search, int? page)
        {
            return View(db.Drawings_PDF.Include(x => x.AdditionalDrawings).Include(x => x.SourceDrawing)
                                       .Where(x => x.Drawing.Contains(search) || search == null).ToList()
                                       .ToPagedList(page ?? 1, 15));
        }

        // GET: Additional/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Drawings_PDF drawings_PDF = db.Drawings_PDF.Find(id);
            if (drawings_PDF == null)
            {
                return HttpNotFound();
            }
            return View(drawings_PDF);
        }

        // GET: Additional/AddAdditionalDrawing/5
        public ActionResult AddAdditionalDrawing(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.DrawingManager, T("You Do Not Have Permission to View this Page"))
                && !Services.Authorizer.Authorize(Permissions.DrawingAdmin, T("You Do Not Have Permission to View this Page"))
               ) return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Drawings_PDF drawings_PDF = db.Drawings_PDF.Find(id);
            AdditionalDrawingViewModel vm = new AdditionalDrawingViewModel();
            vm.SourceId = id;
            vm.pdf = drawings_PDF;
            ViewBag.TargetId = new SelectList(db.Drawings_PDF.OrderBy(x => x.Id), "Id", "Drawing");

            if (drawings_PDF == null)
            {
                return HttpNotFound();
            }
            return View(vm);
        }

        // POST: Additional/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAdditionalDrawing(AdditionalDrawingViewModel vm)
        {
            if (!Services.Authorizer.Authorize(Permissions.DrawingManager, T("You Do Not Have Permission to View this Page"))
                && !Services.Authorizer.Authorize(Permissions.DrawingAdmin, T("You Do Not Have Permission to View this Page"))
               ) return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
                var pdf = db.Drawings_PDF.Find(vm.SourceId);
                Drawings_PDF additionalD = db.Drawings_PDF.Find(vm.TargetId);
                pdf.AdditionalDrawings.Add(additionalD);

                db.SaveChanges();
                return RedirectToAction("Details", new { id = vm.SourceId});
            }
            ViewBag.TargetId = new SelectList(db.Drawings_PDF.OrderBy(x => x.Id), "Id", "Drawing");
            return View(vm);
        }

        // GET: Additional/Create
        public ActionResult Create()
        {
            if (!Services.Authorizer.Authorize(Permissions.DrawingManager, T("You Do Not Have Permission to View this Page"))
                && !Services.Authorizer.Authorize(Permissions.DrawingAdmin, T("You Do Not Have Permission to View this Page"))
               ) return new HttpUnauthorizedResult();
            return View();
        }

        // POST: Additional/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Drawings_PDF drawings_PDF)
        {
            if (!Services.Authorizer.Authorize(Permissions.DrawingManager, T("You Do Not Have Permission to View this Page"))
                && !Services.Authorizer.Authorize(Permissions.DrawingAdmin, T("You Do Not Have Permission to View this Page"))
               ) return new HttpUnauthorizedResult();
            if (ModelState.IsValid)
            {
                db.Drawings_PDF.Add(drawings_PDF);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(drawings_PDF);
        }

        // GET: Additional/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.DrawingManager, T("You Do Not Have Permission to View this Page"))
                && !Services.Authorizer.Authorize(Permissions.DrawingAdmin, T("You Do Not Have Permission to View this Page"))
               ) return new HttpUnauthorizedResult();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.AddDrawing = new SelectList(db.Drawings_PDF.OrderBy(x => x.Id), "Id", "Drawing");
            Drawings_PDF drawings_PDF = db.Drawings_PDF.Find(id);
            if (drawings_PDF == null)
            {
                return HttpNotFound();
            }
            return View(drawings_PDF);
        }

        // POST: Additional/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Drawings_PDF drawings_PDF, int AddDrawing)
        {
            if (!Services.Authorizer.Authorize(Permissions.DrawingManager, T("You Do Not Have Permission to View this Page"))
                && !Services.Authorizer.Authorize(Permissions.DrawingAdmin, T("You Do Not Have Permission to View this Page"))
               ) return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
                var pdf = db.Drawings_PDF.Find(drawings_PDF.Id);
                Drawings_PDF additionalD = db.Drawings_PDF.Find(AddDrawing);
                pdf.AdditionalDrawings.Add(additionalD);

                //drawings_PDF.AdditionalDrawings.Add(additionalD);
                //drawings_PDF.SourceDrawing.Add(drawings_PDF);

                // db.Entry(drawings_PDF).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdditionalDrawing = new SelectList(db.Drawings_PDF.OrderBy(x => x.Id), "Id", "Drawing");
            return View(drawings_PDF);
        }

        // GET: Additional/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!Services.Authorizer.Authorize(Permissions.DrawingManager, T("You Do Not Have Permission to View this Page"))
                && !Services.Authorizer.Authorize(Permissions.DrawingAdmin, T("You Do Not Have Permission to View this Page"))
               ) return new HttpUnauthorizedResult();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Drawings_PDF drawings_PDF = db.Drawings_PDF.Find(id);
            if (drawings_PDF == null)
            {
                return HttpNotFound();
            }
            return View(drawings_PDF);
        }

        // POST: Additional/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.DrawingManager, T("You Do Not Have Permission to View this Page"))
                && !Services.Authorizer.Authorize(Permissions.DrawingAdmin, T("You Do Not Have Permission to View this Page"))
               ) return new HttpUnauthorizedResult();

            Drawings_PDF drawings_PDF = db.Drawings_PDF.Find(id);
            db.Drawings_PDF.Remove(drawings_PDF);
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