using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.TimeMFG;

namespace Time.Support.Controllers
{
    public class TicketTasksController : Controller
    {
        private TimeMFGEntities db = new TimeMFGEntities();

        // GET: TicketTasks
        public async Task<ActionResult> Index()
        {
            var ticketTasks = db.TicketTasks.Include(t => t.TicketEmployee).Include(t => t.TicketProject);
            return View(await ticketTasks.ToListAsync());
        }

        // GET: TicketTasks/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketTask ticketTask = await db.TicketTasks.FindAsync(id);
            if (ticketTask == null)
            {
                return HttpNotFound();
            }
            return View(ticketTask);
        }

        // GET: TicketTasks/Create
        public ActionResult Create()
        {
            ViewBag.AssignedTo = new SelectList(db.TicketEmployees, "EmployeeID", "LastName");
            ViewBag.TicketID = new SelectList(db.TicketProjects, "TicketID", "Title");
            return View();
        }

        // POST: TicketTasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,TicketID,AssignedTo,Task,Completed,CompletionDate,Notes")] TicketTask ticketTask)
        {
            if (ModelState.IsValid)
            {
                db.TicketTasks.Add(ticketTask);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AssignedTo = new SelectList(db.TicketEmployees, "EmployeeID", "LastName", ticketTask.AssignedTo);
            ViewBag.TicketID = new SelectList(db.TicketProjects, "TicketID", "Title", ticketTask.TicketID);
            return View(ticketTask);
        }

        // GET: TicketTasks/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketTask ticketTask = await db.TicketTasks.FindAsync(id);
            if (ticketTask == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssignedTo = new SelectList(db.TicketEmployees, "EmployeeID", "LastName", ticketTask.AssignedTo);
            ViewBag.TicketID = new SelectList(db.TicketProjects, "TicketID", "Title", ticketTask.TicketID);
            return View(ticketTask);
        }

        // POST: TicketTasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,TicketID,AssignedTo,Task,Completed,CompletionDate,Notes")] TicketTask ticketTask)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketTask).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AssignedTo = new SelectList(db.TicketEmployees, "EmployeeID", "LastName", ticketTask.AssignedTo);
            ViewBag.TicketID = new SelectList(db.TicketProjects, "TicketID", "Title", ticketTask.TicketID);
            return View(ticketTask);
        }

        // GET: TicketTasks/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketTask ticketTask = await db.TicketTasks.FindAsync(id);
            if (ticketTask == null)
            {
                return HttpNotFound();
            }
            return View(ticketTask);
        }

        // POST: TicketTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TicketTask ticketTask = await db.TicketTasks.FindAsync(id);
            db.TicketTasks.Remove(ticketTask);
            await db.SaveChangesAsync();
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
