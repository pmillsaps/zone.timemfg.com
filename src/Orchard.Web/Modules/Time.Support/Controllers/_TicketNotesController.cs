using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.TimeMFG;

namespace Time.Support.Controllers
{
    public class _TicketNotesController : Controller
    {
        private TimeMFGEntities db = new TimeMFGEntities();

        // GET: _TicketNotes
        public ActionResult Index()
        {
            var ticketNotes = db.TicketNotes.Include(t => t.TicketProject);
            return View(ticketNotes.ToList());
        }

        // GET: _TicketNotes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketNote ticketNote = db.TicketNotes.Find(id);
            if (ticketNote == null)
            {
                return HttpNotFound();
            }
            return View(ticketNote);
        }

        // GET: _TicketNotes/Create
        public ActionResult Create()
        {
            ViewBag.TicketID = new SelectList(db.TicketProjects, "TicketID", "Title");
            return View();
        }

        // POST: _TicketNotes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NoteID,TicketID,Note,CreatedBy,CreatedDate,Visibility")] TicketNote ticketNote)
        {
            if (ModelState.IsValid)
            {
                db.TicketNotes.Add(ticketNote);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TicketID = new SelectList(db.TicketProjects, "TicketID", "Title", ticketNote.TicketID);
            return View(ticketNote);
        }

        // GET: _TicketNotes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketNote ticketNote = db.TicketNotes.Find(id);
            if (ticketNote == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketID = new SelectList(db.TicketProjects, "TicketID", "Title", ticketNote.TicketID);
            return View(ticketNote);
        }

        // POST: _TicketNotes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NoteID,TicketID,Note,CreatedBy,CreatedDate,Visibility")] TicketNote ticketNote)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketNote).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TicketID = new SelectList(db.TicketProjects, "TicketID", "Title", ticketNote.TicketID);
            return View(ticketNote);
        }

        // GET: _TicketNotes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketNote ticketNote = db.TicketNotes.Find(id);
            if (ticketNote == null)
            {
                return HttpNotFound();
            }
            return View(ticketNote);
        }

        // POST: _TicketNotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketNote ticketNote = db.TicketNotes.Find(id);
            db.TicketNotes.Remove(ticketNote);
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
