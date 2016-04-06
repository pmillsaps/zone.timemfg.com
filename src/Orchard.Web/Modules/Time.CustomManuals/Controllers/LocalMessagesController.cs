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
    public class LocalMessagesController : Controller
    {
        private CustomManualsEntities db = new CustomManualsEntities();

        // GET: LocalMessages
        public ActionResult Index()
        {
            var localMessages = db.LocalMessages.Include(l => l.LanguageCode);
            return View(localMessages.ToList());
        }

        // GET: LocalMessages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocalMessage localMessage = db.LocalMessages.Find(id);
            if (localMessage == null)
            {
                return HttpNotFound();
            }
            return View(localMessage);
        }

        // GET: LocalMessages/Create
        public ActionResult Create()
        {
            ViewBag.LanguageID = new SelectList(db.LanguageCodes, "Language_ID", "Language_Code");
            return View();
        }

        // POST: LocalMessages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LanguageID,Name,MessageText")]LocalMessage localMessage)
        {
            var dupeCheck = db.LocalMessages.FirstOrDefault(x => x.LanguageID == localMessage.LanguageID && x.Name == localMessage.Name);
            if (dupeCheck != null) ModelState.AddModelError("", "There is aready an entry for this language and Name combination.");

            if (ModelState.IsValid)
            {
                db.LocalMessages.Add(localMessage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.LanguageID = new SelectList(db.LanguageCodes, "Language_ID", "Language_Code", localMessage.LanguageID);
            return View(localMessage);
        }

        // GET: LocalMessages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocalMessage localMessage = db.LocalMessages.Find(id);
            if (localMessage == null)
            {
                return HttpNotFound();
            }
            ViewBag.LanguageID = new SelectList(db.LanguageCodes, "Language_ID", "Language_Code", localMessage.LanguageID);
            return View(localMessage);
        }

        // POST: LocalMessages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,LanguageID,Name,MessageText")] LocalMessage localMessage)
        {
            var dupeCheck = db.LocalMessages.FirstOrDefault(x => x.LanguageID == localMessage.LanguageID && x.Name == localMessage.Name && x.ID != localMessage.ID);
            if (dupeCheck != null) ModelState.AddModelError("", "There is aready an entry for this language and Name combination.");

            if (ModelState.IsValid)
            {
                db.Entry(localMessage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LanguageID = new SelectList(db.LanguageCodes, "Language_ID", "Language_Code", localMessage.LanguageID);
            return View(localMessage);
        }

        // GET: LocalMessages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocalMessage localMessage = db.LocalMessages.Find(id);
            if (localMessage == null)
            {
                return HttpNotFound();
            }
            return View(localMessage);
        }

        // POST: LocalMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LocalMessage localMessage = db.LocalMessages.Find(id);
            db.LocalMessages.Remove(localMessage);
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
