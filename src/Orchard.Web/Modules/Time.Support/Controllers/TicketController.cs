using MoreLinq;
using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Time.Data.EntityModels.TimeMFG;
using Time.Support.Helpers;
using Time.Support.Models;

namespace Time.Support.Controllers
{
    [Authorize]
    [Themed]
    public class TicketController : Controller
    {
        #region Public Properties

        #endregion Public Properties

        public IOrchardServices Services { get; set; }

        public Localizer T { get; set; }
        private readonly TimeMFGEntities _db;

        public TicketController(IOrchardServices services)
        {
            Services = services;
            _db = new TimeMFGEntities();
            //Setup();
        }

        public TicketController(IOrchardServices services, TimeMFGEntities db)
        {
            Services = services;
            _db = db;
            //Setup();
        }

        private MenuViewModel Setup()
        {
            T = NullLocalizer.Instance;

            var tickets = _db.TicketProjects.Where(x => x.TicketStatus.isOpen);

            MenuViewModel vm = new MenuViewModel();

            var myTickets = tickets.Where(x => x.TicketEmployee.NTLogin == HttpContext.User.Identity.Name);

            vm.MyOpenTicketsbyStatus = myTickets.DistinctBy(x => x.TicketStatus)
                .Select(data => new Status { TicketStatus = data.TicketStatus, Count = myTickets.Count(x => x.TicketStatus.StatusID == data.TicketStatus.StatusID) })
                .OrderBy(x => x.TicketStatus.Name).ToList();

            if (Services.Authorizer.Authorize(Permissions.SupportAdmin))
            {
                vm.OpenTicketsbyAssignment = tickets.Where(x => x.TicketEmployee != null).DistinctBy(x => x.TicketEmployee)
                    .Select(data => new AssignedTo { Employee = data.TicketEmployee, Count = data.TicketEmployee.TicketProjects.Count(x => x.TicketStatus.isOpen) })
                    .OrderByDescending(x => x.Count).ToList();
            }

            vm.OpenTicketsbyCategory = tickets.DistinctBy(x => x.TicketCategory)
                .Select(data => new Category { TicketCategory = data.TicketCategory, Count = data.TicketCategory.TicketProjects.Count(x => x.TicketStatus.isOpen) })
                .OrderBy(x => x.TicketCategory.Name).ToList();

            vm.OpenTicketsbyDepartment = tickets.DistinctBy(x => x.TicketDepartment)
                .Select(data => new Department { TicketDepartment = data.TicketDepartment, Count = data.TicketDepartment.TicketProjects.Count(x => x.TicketStatus.isOpen) })
                .OrderBy(x => x.TicketDepartment.Name).ToList();

            vm.OpenTicketsbyStatus = tickets.DistinctBy(x => x.TicketStatus)
                .Select(data => new Status { TicketStatus = data.TicketStatus, Count = data.TicketStatus.TicketProjects.Count(x => x.TicketStatus.isOpen) })
                .OrderBy(x => x.TicketStatus.Name).ToList();

            if (Services.Authorizer.Authorize(Permissions.SupportAdmin))
            {
                vm.Admin = true;
            }
            if (Services.Authorizer.Authorize(Permissions.SupportIT))
            {
                vm.IT = true;
            }

            return vm;
        }

        [ChildActionOnly]
        public ActionResult SideBar()
        {
            var vm = Setup();
            return PartialView("SideBar", vm);
        }

        public ActionResult Index()
        {
            var qry = _db.TicketProjects.Include("TicketEmployee").Where(c => c.TicketStatus.isOpen).ToList();

            //qry = SortandPage(qry);

            // ViewData["OpenCount"] = _db.TicketProjects.Count(c => c.TicketStatus.isOpen);
            //if (!String.IsNullOrEmpty(Request.QueryString["sort"]))
            //{
            //    ViewData["SortBy"] = "sort=" + Request.QueryString["sort"];
            //}
            //else
            //{
            //    ViewData["SortBy"] = "";
            //}
            ViewBag.Title = "Overview";
            ViewBag.SubTitle = "Open Tickets";
            return View(qry);
        }

        public ActionResult Info(int id)
        {
            var qry = _db.TicketProjects.Where(c => c.TicketID == id).FirstOrDefault();
            if (qry == null) return RedirectToAction("Index");

            var vm = new TicketViewModel() { Ticket = qry , Tasks= qry.TicketTasks};
            if (Services.Authorizer.Authorize(Permissions.SupportAdmin))
            {
                vm.Admin = true;
            }
            if (Services.Authorizer.Authorize(Permissions.SupportIT))
            {
                vm.IT = true;
            }
            GenerateDropDowns(vm, qry);


            //qry = SortandPage(qry);

            //ViewData["OpenCount"] = _db.TicketProjects.Count(c => c.TicketStatus.isOpen);
            //if (!String.IsNullOrEmpty(Request.QueryString["sort"]))
            //{
            //    ViewData["SortBy"] = "sort=" + Request.QueryString["sort"];
            //}
            //else
            //{
            //    ViewData["SortBy"] = "";
            //}
            if (TempData["message"] != "") ViewBag.Message = TempData["message"];

            return View(vm);
        }

        public ActionResult Search(string Search, bool IncludeComplete)
        {
            if (string.IsNullOrEmpty(Search)) return RedirectToAction("Index");
            if (Regex.IsMatch(Search, @"#\d"))
                return RedirectToAction("Info", new { id = Search.Substring(1) });

            var qry = _db.TicketProjects.AsQueryable();
            if (!IncludeComplete) qry = qry.Where(x => x.TicketStatus.isOpen);

            //List<int> tickets = new List<int>();
            //var notes = _db.TicketNotes.AsQueryable();
            //if (!IncludeComplete) notes = notes.Where(x => x.TicketProject.TicketStatus.isOpen);

            //foreach (var item in Search.Split(' '))
            //{
            //    notes = notes.Where(x => x.Note.Contains(item));
            //}
            //if (notes.Count() > 0)
            //{
            //    tickets.AddRange(notes.Select(x => x.TicketID ?? 0).Distinct().ToList());
            //}
            var searchVisibility = 5;
            if (Services.Authorizer.Authorize(Permissions.SupportAdmin)) searchVisibility = 3;

            foreach (var item in Search.Split(' '))
            {
                qry = qry.Where(x => x.Title.Contains(item)
                    || x.Description.Contains(item)
                    || x.Notes.Contains(item)
                    || x.PrivateNotes.Contains(item)
                    || x.TicketNotes.Any(n => n.Visibility >= searchVisibility && n.Note.Contains(item))
                );

                var tmp = qry.ToList();
            }

            qry = qry.OrderBy(x => x.TicketID);
            //qry = SortandPage(qry);

            ViewData["OpenCount"] = _db.TicketProjects.Count(c => c.TicketStatus.isOpen);
            return View("Index", qry.ToList());
        }

        [HttpGet]
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
                if (ticketProject.PriorityID > 4) ticketProject.PriorityID = 4; // highest a new ticket can be set is 4=High
                ticketProject.Status = 1;   // Set Ticket Status to 1=Waiting Supervisor Approval
                ticketProject.RequestedDate = DateTime.Now;
                ticketProject.RequestedBy = HttpContext.User.Identity.Name;
                ticketProject.RequestedByFriendly = HttpContext.User.Identity.Name;
                _db.TicketProjects.Add(ticketProject);
                _db.SaveChanges();

                return RedirectToAction("Index");
            }

            GenerateDropDowns(ticketProject);
            return View(ticketProject);
        }

        [HttpPost]
        public ActionResult Update(TicketProject ticketProject)
        {
            string msg = string.Empty;
            var currentUser = System.Web.HttpContext.Current.User.Identity.Name;
            var ticket = _db.TicketProjects.Single(x => x.TicketID == ticketProject.TicketID);
            string notificationList = String.Empty;

            if (ticket.AssignedEmployeeID != ticketProject.AssignedEmployeeID)
            {
                var emp = _db.TicketEmployees.Single(x => x.EmployeeID == ticketProject.AssignedEmployeeID);

                if (ticket.TicketEmployee != null)
                {
                    msg += string.Format("Assigned Employee was changed: {0} -> {1}", ticket.TicketEmployee.FullName, emp.FullName);
                }
                else
                {
                    msg += string.Format("Assigned ticket to {0}", emp.FullName);
                    ticket.ResourceEmployeeID = ticket.AssignedEmployeeID;
                    msg += string.Format("{1}Assigned Ticket Resource to {0}", emp.FullName, Environment.NewLine);
                }

                ticket.AssignedEmployeeID = ticketProject.AssignedEmployeeID;
                ticket.TicketEmployee = emp;
                ticket.SendAssignmentNotification();
                ticket.TicketNotes.Add(new TicketNote
                {
                    CreatedBy = currentUser,
                    CreatedDate = DateTime.Now,
                    TicketID = ticket.TicketID,
                    Visibility = 1, // Private Note
                    Note = msg
                });
            }

            if (ticket.ResourceEmployeeID != ticketProject.ResourceEmployeeID)
            {
                var emp = _db.TicketEmployees.Single(x => x.EmployeeID == ticketProject.ResourceEmployeeID);

                //if (ticket.TicketEmployee != null)
                //{
                //    msg += string.Format("Assigned Employee was changed: {0} -> {1}", ticket.TicketEmployee.FullName, emp.FullName);
                //}
                //else
                //{
                //    msg += string.Format("Assigned ticket to {0}", emp.FullName);
                //    ticket.ResourceEmployeeID = ticket.AssignedEmployeeID;
                //    msg += string.Format("{1}Assigned Ticket Resource to {0}", emp.FullName, Environment.NewLine);
                //}

                //ticket.AssignedEmployeeID = ticketProject.AssignedEmployeeID;
                //ticket.TicketEmployee = emp;
                //ticket.SendAssignmentNotification();
                //ticket.TicketNotes.Add(new TicketNote
                //{
                //    CreatedBy = currentUser,
                //    CreatedDate = DateTime.Now,
                //    TicketID = ticket.TicketID,
                //    Visibility = 1, // Private Note
                //    Note = msg
                //});
            }

            if (ticket.PriorityID != ticketProject.PriorityID)
            {
                var priority = _db.TicketPriorities.Single(x => x.PriorityID == ticketProject.PriorityID);
                msg += string.Format("Priority was changed: {0} -> {1}", ticket.TicketPriority.Name, priority.Name);
            }

            if (ticket.DepartmentID != ticketProject.DepartmentID)
            {
                var dept = _db.TicketDepartments.Single(x => x.DepartmentID == ticketProject.DepartmentID);
                msg += string.Format("Department was changed: {0} -> {1}", ticket.TicketDepartment.Name, dept.Name);
            }

            if (ticket.CategoryID != ticketProject.CategoryID)
            {
                var cat = _db.TicketCategories.Single(x => x.CategoryID == ticketProject.CategoryID);
                msg += string.Format("Category was changed: {0} -> {1}", ticket.TicketCategory.Name, cat.Name);
            }

            if (ticket.ResourceEmployeeID != ticketProject.ResourceEmployeeID)
            {
                var emp = _db.TicketEmployees.Single(x => x.EmployeeID == ticketProject.ResourceEmployeeID);
                if (ticket.TicketEmployee1 != null)
                    msg += string.Format("Resource Employee was changed: {0} -> {1}", ticket.TicketEmployee1.FullName, emp.FullName);
                else
                    msg += string.Format("Set Resource Employee to {0}", emp.FullName);
            }

            if (ticket.Status != ticketProject.Status)
            {
                var stat = _db.TicketStatuses.Single(x => x.StatusID == ticketProject.Status);
                msg += string.Format("Status was changed: {0} -> {1}", ticket.TicketStatus.Name, stat.Name);
            }

            
            // Save all database changes 

            _db.SaveChanges();

            //public ActionResult Edit([Bind(Include = "TicketID,DepartmentID,PriorityID,CategoryID,Title,Description,Notes,PrivateNotes,RequestedBy,RequestedByFriendly,RequestedDate,AssignedEmployeeID,ResourceEmployeeID,Status,ApprovalDate,ApprovedBy,ProjectBeginDate,ProjectEndDate,TicketSequence,CompletionDate,ApprovalCode")] TicketProject ticketProject)

            //int PriorityID = items.GetValue("PriorityID").ConvertTo(int);
            TempData["message"] = msg;
            return RedirectToAction("Info", new { id = ticketProject.TicketID});
        }

        //[HttpPost]
        //public ActionResult Update(TicketViewModel ticketProject)
        //{
        //    return RedirectToAction("Info", new { id = ticketProject.Ticket.TicketID });
        //}


        public ActionResult MyStatus(int id)
        {
            var login = HttpContext.User.Identity.Name;
            var tickets = _db.TicketProjects.Where(x => x.TicketStatus.isOpen && x.TicketStatus.StatusID == id && x.TicketEmployee.NTLogin == login);
            ViewBag.Title = string.Format("[{0}] My Tickets", _db.TicketStatuses.Single(x => x.StatusID == id).Name);
            return View("Index",tickets);
        }

        public ActionResult Status(int id)
        {
            var tickets = _db.TicketProjects.Where(x => x.TicketStatus.isOpen && x.TicketStatus.StatusID == id);
            ViewBag.Title = string.Format("[{0}] Status", _db.TicketStatuses.Single(x => x.StatusID == id).Name);
            return View("Index", tickets);
        }

        public ActionResult Employee(int id)
        {
            var tickets = _db.TicketProjects.Where(x => x.TicketStatus.isOpen && x.TicketEmployee.EmployeeID == id);
            var employee = _db.TicketEmployees.Single(x => x.EmployeeID == id);
            ViewBag.Title = string.Format("[{0} {1}] Employee", employee.FirstName, employee.LastName);
            return View("Index", tickets);
        }

        public ActionResult Category(int id)
        {
            var tickets = _db.TicketProjects.Where(x => x.TicketStatus.isOpen && x.TicketCategory.CategoryID == id);
            ViewBag.Title = string.Format("[{0}] Category", _db.TicketCategories.Single(x => x.CategoryID == id).Name);
            return View("Index", tickets);
        }

        public ActionResult Department(int id)
        {
            var tickets = _db.TicketProjects.Where(x => x.TicketStatus.isOpen && x.TicketDepartment.DepartmentID == id);
            ViewBag.Title = string.Format("[{0}] Department", _db.TicketDepartments.Single(x => x.DepartmentID == id).Name);
            return View("Index", tickets);
        }

        public ActionResult MyOpenTickets()
        {
            var login = HttpContext.User.Identity.Name;
            var tickets = _db.TicketProjects.Where(x => x.TicketStatus.isOpen && x.RequestedBy == login);
            ViewBag.Title = "My Open Tickets";
            return View("Index", tickets);
        }

        public ActionResult MyTickets()
        {
            var login = HttpContext.User.Identity.Name;
            var tickets = _db.TicketProjects.Where(x => x.RequestedBy == login);
            ViewBag.Title = "All My Tickets";
            ViewBag.SubTitle = string.Format("{0} Tickets, {1} Open Tickets", tickets.Count(), tickets.Count(x => x.TicketStatus.isOpen));
            return View("Index", tickets);
        }

        private void GenerateDropDowns()
        {
            ViewBag.CategoryID = new SelectList(_db.TicketCategories.OrderBy(x => x.Name), "CategoryID", "Name");
            ViewBag.DepartmentID = new SelectList(_db.TicketDepartments.OrderBy(x => x.Name), "DepartmentID", "Name");
            ViewBag.AssignedEmployeeID = new SelectList(_db.TicketEmployees.OrderBy(x => x.FullName), "EmployeeID", "LastName");
            ViewBag.ResourceEmployeeID = new SelectList(_db.TicketEmployees.OrderBy(x => x.FullName), "EmployeeID", "LastName");
            ViewBag.PriorityID = new SelectList(_db.TicketPriorities, "PriorityID", "Name");
            ViewBag.Status = new SelectList(_db.TicketStatuses.OrderBy(x => x.Name), "StatusID", "Name");
        }

        private void GenerateDropDowns(TicketProject ticketProject)
        {
            ViewBag.CategoryID = new SelectList(_db.TicketCategories, "CategoryID", "Name", ticketProject.CategoryID);
            ViewBag.DepartmentID = new SelectList(_db.TicketDepartments, "DepartmentID", "Name", ticketProject.DepartmentID);
            ViewBag.AssignedEmployeeID = new SelectList(_db.TicketEmployees, "EmployeeID", "LastName", ticketProject.AssignedEmployeeID);
            ViewBag.ResourceEmployeeID = new SelectList(_db.TicketEmployees, "EmployeeID", "LastName", ticketProject.ResourceEmployeeID);
            ViewBag.PriorityID = new SelectList(_db.TicketPriorities, "PriorityID", "Name", ticketProject.PriorityID);
            ViewBag.Status = new SelectList(_db.TicketStatuses, "StatusID", "Name", ticketProject.Status);
        }

        private void GenerateDropDowns(TicketViewModel vm)
        {
            vm.CategoryID = new SelectList(_db.TicketCategories, "CategoryID", "Name");
            vm.DepartmentID = new SelectList(_db.TicketDepartments, "DepartmentID", "Name");
            vm.AssignedEmployeeID = new SelectList(_db.TicketEmployees, "EmployeeID", "LastName");
            vm.ResourceEmployeeID = new SelectList(_db.TicketEmployees, "EmployeeID", "LastName");
            vm.PriorityID = new SelectList(_db.TicketPriorities, "PriorityID", "Name");
            vm.Status = new SelectList(_db.TicketStatuses, "StatusID", "Name");
        }

        private void GenerateDropDowns(TicketViewModel vm, TicketProject ticketProject)
        {

            vm.CategoryID = new SelectList(_db.TicketCategories.Where(x => x.isActive == true).OrderBy(x => x.Name), "CategoryID", "Name", ticketProject.CategoryID);
            vm.DepartmentID = new SelectList(_db.TicketDepartments.OrderBy(x => x.Name), "DepartmentID", "Name", ticketProject.DepartmentID);
            vm.AssignedEmployeeID = new SelectList(_db.TicketEmployees.Where(x => x.InActive != true).OrderBy(x => x.FirstName), "EmployeeID", "FullName", ticketProject.AssignedEmployeeID);
            vm.ResourceEmployeeID = new SelectList(_db.TicketEmployees.Where(x => x.InActive != true).OrderBy(x => x.FirstName), "EmployeeID", "FullName", ticketProject.ResourceEmployeeID);
            vm.PriorityID = new SelectList(_db.TicketPriorities, "PriorityID", "Name", ticketProject.PriorityID);
            vm.Status = new SelectList(_db.TicketStatuses.OrderBy(x => x.Name), "StatusID", "Name", ticketProject.Status);

            vm.TicketVisibility = new SelectList(_db.TicketVisibilities.OrderByDescending(x => x.Id), "Id", "Name");
        }

        [HttpGet]
        public ActionResult ChangeUser(int id)
        {
            TicketProject ticketProject = _db.TicketProjects.Find(id);

            var requestors = new SelectList(_db.TicketProjects.DistinctBy(x => x.RequestedBy).ToList(), "RequestedBy", "RequestedBy", ticketProject.RequestedBy);
            //select new SelectListItem
            //{
            //    Text = col.RequestedBy,
            //    Value = col.RequestedBy,
            //    Selected = (ticketProject.RequestedBy == col.RequestedBy)
            //}).ToList();

            ViewData["requestor"] = requestors;
            ViewBag.requestor = requestors;

            return View(ticketProject);
        }

        [HttpPost]
        public ActionResult ChangeUser(int id, string requestor)
        {
            TicketProject ticketProject = _db.TicketProjects.Find(id);
            //var tr = new TicketRepository();
            ////var id = Convert.ToInt32(formValues.GetValue("id"));
            //var ticket = tr.Single(c => c.TicketID == id);
            var oldRequestor = ticketProject.RequestedBy;

            if (oldRequestor != requestor)
            {
                ticketProject.RequestedBy = requestor;
                ticketProject.RequestedByFriendly = requestor;
                //var saved = tr.Update(ent);
                //tr.SaveAll();

                // Log the change as a private note
                ticketProject.TicketNotes.Add(new TicketNote
                {
                    Note = string.Format("Requestor was changed: {0} -> {1}", oldRequestor, requestor),
                    CreatedBy = User.Identity.Name,
                    CreatedDate = DateTime.Now,
                    Visibility = 1
                });
                _db.SaveChanges();
            }

            return RedirectToAction("Info", new { id });
        }

        #region TicketTasks

        // GET: TicketTasks/Create
        [HttpGet]
        public ActionResult AddTask(int id)
        {
            var task = new TicketTask{ TicketID = id };
            ViewBag.AssignedTo = new SelectList(_db.TicketEmployees.OrderBy(x => x.FirstName), "EmployeeID", "FullName");
            //ViewBag.TicketID = new SelectList(_db.TicketProjects, "TicketID", "Title");
            return View(task);
        }

        // POST: TicketTasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddTask([Bind(Include = "ID,TicketID,AssignedTo,Task,Completed,CompletionDate,Notes")] TicketTask ticketTask)
        {
            if (ModelState.IsValid)
            {
                _db.TicketTasks.Add(ticketTask);
                await _db.SaveChangesAsync();
                return RedirectToAction("Info", new { id = ticketTask.TicketID});
            }

            ViewBag.AssignedTo = new SelectList(_db.TicketEmployees, "EmployeeID", "LastName", ticketTask.AssignedTo);
            return View(ticketTask);
        }

        // GET: TicketTasks/Create
        [HttpGet]
        public ActionResult EditTask(int id)
        {
            var task = _db.TicketTasks.SingleOrDefault(x => x.ID == id);
            if (task == null) return new HttpNotFoundResult();

            ViewBag.AssignedTo = new SelectList(_db.TicketEmployees.OrderBy(x => x.FirstName), "EmployeeID", "FullName", task.AssignedTo);
            //ViewBag.TicketID = new SelectList(_db.TicketProjects, "TicketID", "Title");
            return View(task);
        }

        // POST: TicketTasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditTask([Bind(Include = "ID,TicketID,AssignedTo,Task,Completed,CompletionDate,Notes")] TicketTask ticketTask)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(ticketTask).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Info", new { id = ticketTask.TicketID });
            }

            ViewBag.AssignedTo = new SelectList(_db.TicketEmployees, "EmployeeID", "LastName", ticketTask.AssignedTo);
            return View(ticketTask);
        }

        // GET: TicketTasks/Delete/5
        public async Task<ActionResult> DeleteTask(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketTask ticketTask = await _db.TicketTasks.FindAsync(id);
            if (ticketTask == null)
            {
                return HttpNotFound();
            }
            return View(ticketTask);
        }

        // POST: TicketTasks/Delete/5
        [HttpPost, ActionName("DeleteTask")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TicketTask ticketTask = await _db.TicketTasks.FindAsync(id);
            var ticketID = ticketTask.TicketID;
            _db.TicketTasks.Remove(ticketTask);
            await _db.SaveChangesAsync();
            return RedirectToAction("Info", new { id = ticketID});
        }

        #endregion

    }
}