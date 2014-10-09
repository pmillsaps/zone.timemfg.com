using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Time.Support.EntityModels.TimeMfg;

namespace Time.Support.Controllers
{
    public class TicketProjectsController : Controller
    {
        private TimeMFGEntities db = new TimeMFGEntities();

        // GET: TicketProjects
        public ActionResult Index()
        {
            var ticketProjects = db.TicketProjects.Include(t => t.TicketCategory).Include(t => t.TicketDepartment).Include(t => t.TicketEmployee).Include(t => t.TicketEmployee1).Include(t => t.TicketPriority).Include(t => t.TicketStatus);
            return View(ticketProjects.ToList());
        }

        // GET: TicketProjects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketProject ticketProject = db.TicketProjects.Find(id);
            if (ticketProject == null)
            {
                return HttpNotFound();
            }
            return View(ticketProject);
        }

        // GET: TicketProjects/Create
        public ActionResult Create()
        {
            GenerateDropDowns();
            return View();
        }

        // POST: TicketProjects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TicketID,DepartmentID,PriorityID,CategoryID,Title,Description,Notes,PrivateNotes,RequestedBy,RequestedByFriendly,RequestedDate,AssignedEmployeeID,ResourceEmployeeID,Status,ApprovalDate,ApprovedBy,ProjectBeginDate,ProjectEndDate,TicketSequence,CompletionDate,ApprovalCode")] TicketProject ticketProject)
        {
            if (ModelState.IsValid)
            {
                db.TicketProjects.Add(ticketProject);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            GenerateDropDowns(ticketProject);
            return View(ticketProject);
        }

        // GET: TicketProjects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketProject ticketProject = db.TicketProjects.Find(id);
            if (ticketProject == null)
            {
                return HttpNotFound();
            }
            GenerateDropDowns(ticketProject);
            return View(ticketProject);
        }

        // POST: TicketProjects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TicketID,DepartmentID,PriorityID,CategoryID,Title,Description,Notes,PrivateNotes,RequestedBy,RequestedByFriendly,RequestedDate,AssignedEmployeeID,ResourceEmployeeID,Status,ApprovalDate,ApprovedBy,ProjectBeginDate,ProjectEndDate,TicketSequence,CompletionDate,ApprovalCode")] TicketProject ticketProject)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketProject).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(ticketProject);
            return View(ticketProject);
        }

        // GET: TicketProjects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketProject ticketProject = db.TicketProjects.Find(id);
            if (ticketProject == null)
            {
                return HttpNotFound();
            }
            return View(ticketProject);
        }

        // POST: TicketProjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketProject ticketProject = db.TicketProjects.Find(id);
            db.TicketProjects.Remove(ticketProject);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void GenerateDropDowns()
        {
            ViewBag.CategoryID = new SelectList(db.TicketCategories, "CategoryID", "Name");
            ViewBag.DepartmentID = new SelectList(db.TicketDepartments, "DepartmentID", "Prefix");
            ViewBag.AssignedEmployeeID = new SelectList(db.TicketEmployees, "EmployeeID", "LastName");
            ViewBag.ResourceEmployeeID = new SelectList(db.TicketEmployees, "EmployeeID", "LastName");
            ViewBag.PriorityID = new SelectList(db.TicketPriorities, "PriorityID", "Name");
            ViewBag.Status = new SelectList(db.TicketStatuses, "StatusID", "Name");
        }

        private void GenerateDropDowns(TicketProject ticketProject)
        {
            ViewBag.CategoryID = new SelectList(db.TicketCategories, "CategoryID", "Name", ticketProject.CategoryID);
            ViewBag.DepartmentID = new SelectList(db.TicketDepartments, "DepartmentID", "Prefix", ticketProject.DepartmentID);
            ViewBag.AssignedEmployeeID = new SelectList(db.TicketEmployees, "EmployeeID", "LastName", ticketProject.AssignedEmployeeID);
            ViewBag.ResourceEmployeeID = new SelectList(db.TicketEmployees, "EmployeeID", "LastName", ticketProject.ResourceEmployeeID);
            ViewBag.PriorityID = new SelectList(db.TicketPriorities, "PriorityID", "Name", ticketProject.PriorityID);
            ViewBag.Status = new SelectList(db.TicketStatuses, "StatusID", "Name", ticketProject.Status);
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