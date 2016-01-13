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
                    .OrderBy(x => x.Employee.FirstName).ToList();
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

            vm.TaskCount = _db.TicketTasks.Count(x => x.TicketEmployee.NTLogin == HttpContext.User.Identity.Name && x.Completed != true);
            vm.MyTicketCount = _db.TicketProjects.Count(x => x.RequestedBy == HttpContext.User.Identity.Name);
            vm.MyOpenTicketCount = _db.TicketProjects.Count(x => x.RequestedBy == HttpContext.User.Identity.Name && x.TicketStatus.isOpen);

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
            var qry = _db.TicketProjects.FirstOrDefault(c => c.TicketID == id);
            if (qry == null) return RedirectToAction("Index");

            var vm = new TicketViewModel() { Ticket = qry, Tasks = qry.TicketTasks, TicketId = id };
            if (Services.Authorizer.Authorize(Permissions.SupportAdmin))
            {
                vm.Admin = true;
            }
            if (Services.Authorizer.Authorize(Permissions.SupportIT))
            {
                vm.IT = true;
            }

            if (Services.Authorizer.Authorize(Permissions.SupportApprover))
            {
                vm.Approver = true;
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
            if ((string)TempData["message"] != "") ViewBag.Message = TempData["message"];

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
        public ActionResult Create([Bind(Exclude = "TicketID")] TicketProject ticketProject)
        {
            if (ModelState.IsValid)
            {
                if (ticketProject.PriorityID > 4) ticketProject.PriorityID = 4; // highest a new ticket can be set is 4=High
                ticketProject.Status = 1;   // Set Ticket Status to 1=Waiting Supervisor Approval
                if (Services.Authorizer.Authorize(Permissions.SupportApprover)) ticketProject.Status = 2;
                ticketProject.TicketStatus = _db.TicketStatuses.Find(ticketProject.Status);
                ticketProject.RequestedDate = DateTime.Now;
                ticketProject.RequestedBy = HttpContext.User.Identity.Name;
                ticketProject.RequestedByFriendly = HttpContext.User.Identity.Name;

                _db.TicketStatusHistories.Add(new TicketStatusHistory { TicketStatus = ticketProject.TicketStatus, CreateDate = DateTime.Now });
                var codegen = new CreateRandomCode();
                ticketProject.ApprovalCode = codegen.GenerateCode(12);
                _db.TicketProjects.Add(ticketProject);
                _db.SaveChanges();
                ticketProject.SendNewTicketNotification();
                if (Services.Authorizer.Authorize(Permissions.SupportApprover))
                {
                    ticketProject.SendApprovedNotification();
                }
                else
                {
                    ticketProject.SendSupervisorNewTicketNotification();
                }

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

                if (ticket.AssignedEmployeeID != null && ticket.AssignedEmployeeID != 0)
                {
                    msg += string.Format("Assigned Employee was changed: {0} -> {1}<br />", ticket.TicketEmployee.FullName, emp.FullName);
                }
                else
                {
                    msg += string.Format("Assigned ticket to {0}<br />", emp.FullName);
                }

                if (ticketProject.Status < 3) { ticketProject.Status = 3; }   // Assigned = 3
                ticket.AssignedEmployeeID = ticketProject.AssignedEmployeeID;
                ticket.TicketEmployee = emp;

                if (ticket.ResourceEmployeeID == null)
                {
                    ticket.ResourceEmployeeID = ticket.AssignedEmployeeID;
                    ticketProject.ResourceEmployeeID = ticket.AssignedEmployeeID;
                    msg += string.Format("{1}Assigned Ticket Resource to {0}<br />", emp.FullName, Environment.NewLine);
                }

                ticket.SendAssignmentNotification();
                ticket.TicketNotes.Add(new TicketNote
                {
                    CreatedBy = currentUser,
                    CreatedDate = DateTime.Now,
                    TicketID = ticket.TicketID,
                    Visibility = 1, // Private Note
                    Note = msg
                });
                _db.SaveChanges();
            }

            if (ticket.ResourceEmployeeID == null && ticket.AssignedEmployeeID != null)
            {
                ticket.ResourceEmployeeID = ticket.AssignedEmployeeID;
                _db.SaveChanges();
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
                ticket.PriorityID = ticketProject.PriorityID;
                string updateNote = string.Format("Priority was changed: {0} -> {1}<br />", ticket.TicketPriority.Name, priority.Name);
                msg += updateNote;
                ticket.TicketNotes.Add(new TicketNote() { Note = updateNote, CreatedBy = User.Identity.Name, CreatedDate = DateTime.Now, Visibility = 1 });

                if (ticket.AssignedEmployeeID != null)
                {
                    // Send update notification
                    ticket.SendUpdateNotificationToAssigned(statusMessage: updateNote);
                }
            }

            if (ticket.DepartmentID != ticketProject.DepartmentID)
            {
                var dept = _db.TicketDepartments.Single(x => x.DepartmentID == ticketProject.DepartmentID);
                ticket.DepartmentID = ticketProject.DepartmentID;
                string updateNote = string.Format("Department was changed: {0} -> {1}<br />", ticket.TicketDepartment.Name, dept.Name);
                msg += updateNote;
                ticket.TicketNotes.Add(new TicketNote() { Note = updateNote, CreatedBy = User.Identity.Name, CreatedDate = DateTime.Now, Visibility = 1 });
            }

            if (ticket.CategoryID != ticketProject.CategoryID)
            {
                var cat = _db.TicketCategories.Single(x => x.CategoryID == ticketProject.CategoryID);
                ticket.CategoryID = ticketProject.CategoryID;
                string updateNote = string.Format("Category was changed: {0} -> {1}<br />", ticket.TicketCategory.Name, cat.Name);
                msg += updateNote;
                ticket.TicketNotes.Add(new TicketNote() { Note = updateNote, CreatedBy = User.Identity.Name, CreatedDate = DateTime.Now, Visibility = 1 });
            }

            if (ticket.ResourceEmployeeID != ticketProject.ResourceEmployeeID)
            {
                string updateNote = "";
                var emp = _db.TicketEmployees.Single(x => x.EmployeeID == ticketProject.ResourceEmployeeID);
                if (ticket.TicketEmployee1 != null)
                    updateNote = string.Format("Resource Employee was changed: {0} -> {1}<br />", ticket.TicketEmployee1.FullName, emp.FullName);
                else
                    updateNote = string.Format("Set Resource Employee to {0}<br />", emp.FullName);
                msg += updateNote;
                ticket.TicketNotes.Add(new TicketNote() { Note = updateNote, CreatedBy = User.Identity.Name, CreatedDate = DateTime.Now, Visibility = 1 });
            }

            if (ticket.Status != ticketProject.Status)
            {
                var oldstatus = ticket.TicketStatus;
                var stat = _db.TicketStatuses.Single(x => x.StatusID == ticketProject.Status);
                ticket.Status = ticketProject.Status;
                string updateNote = string.Format("Status was changed: {0} -> {1}<br />", ticket.TicketStatus.Name, stat.Name);
                msg += updateNote;
                ticket.TicketNotes.Add(new TicketNote() { Note = updateNote, CreatedBy = User.Identity.Name, CreatedDate = DateTime.Now, Visibility = 1 });
                _db.SaveChanges();

                if (ticket.TicketStatus.isReadyToComplete)
                    ticket.SendCompletionPendingNotification();
                if (ticket.TicketStatus.isOpen == false && oldstatus.isOpen == true)
                    ticket.CompletionDate = DateTime.Now;
            }

            // Save all database changes

            _db.SaveChanges();

            //public ActionResult Edit([Bind(Include = "TicketID,DepartmentID,PriorityID,CategoryID,Title,Description,Notes,PrivateNotes,RequestedBy,RequestedByFriendly,RequestedDate,AssignedEmployeeID,ResourceEmployeeID,Status,ApprovalDate,ApprovedBy,ProjectBeginDate,ProjectEndDate,TicketSequence,CompletionDate,ApprovalCode")] TicketProject ticketProject)

            //int PriorityID = items.GetValue("PriorityID").ConvertTo(int);
            TempData["message"] = msg;
            return RedirectToAction("Info", new { id = ticketProject.TicketID });
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
            return View("Index", tickets);
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
            ViewBag.DepartmentID = new SelectList(_db.TicketDepartments.Where(x => x.ITOnly != true).OrderBy(x => x.Name), "DepartmentID", "Name");
            if (Services.Authorizer.Authorize(Permissions.SupportAdmin) || Services.Authorizer.Authorize(Permissions.SupportIT))
                ViewBag.DepartmentID = new SelectList(_db.TicketDepartments.OrderBy(x => x.Name), "DepartmentID", "Name");
            ViewBag.AssignedEmployeeID = new SelectList(_db.TicketEmployees.OrderBy(x => x.FullName), "EmployeeID", "FullName");
            ViewBag.ResourceEmployeeID = new SelectList(_db.TicketEmployees.OrderBy(x => x.FullName), "EmployeeID", "FullName");
            ViewBag.PriorityID = new SelectList(_db.TicketPriorities, "PriorityID", "Name");
            ViewBag.Status = new SelectList(_db.TicketStatuses.OrderBy(x => x.Name), "StatusID", "Name");
        }

        private void GenerateDropDowns(TicketProject ticketProject)
        {
            ViewBag.CategoryID = new SelectList(_db.TicketCategories.OrderBy(x => x.Name), "CategoryID", "Name", ticketProject.CategoryID);
            ViewBag.DepartmentID = new SelectList(_db.TicketDepartments.Where(x => x.ITOnly != true).OrderBy(x => x.Name), "DepartmentID", "Name", ticketProject.DepartmentID);
            if (Services.Authorizer.Authorize(Permissions.SupportAdmin) || Services.Authorizer.Authorize(Permissions.SupportIT))
                ViewBag.DepartmentID = new SelectList(_db.TicketDepartments.OrderBy(x => x.Name), "DepartmentID", "Name", ticketProject.DepartmentID);
            ViewBag.AssignedEmployeeID = new SelectList(_db.TicketEmployees.OrderBy(x => x.FirstName), "EmployeeID", "FullName", ticketProject.AssignedEmployeeID);
            ViewBag.ResourceEmployeeID = new SelectList(_db.TicketEmployees.OrderBy(x => x.FirstName), "EmployeeID", "FullName", ticketProject.ResourceEmployeeID);
            ViewBag.PriorityID = new SelectList(_db.TicketPriorities, "PriorityID", "Name", ticketProject.PriorityID);
            ViewBag.Status = new SelectList(_db.TicketStatuses.OrderBy(x => x.Name), "StatusID", "Name", ticketProject.Status);
        }

        private void GenerateDropDowns(TicketViewModel vm)
        {
            vm.CategoryID = new SelectList(_db.TicketCategories.OrderBy(x => x.Name), "CategoryID", "Name");
            vm.DepartmentID = new SelectList(_db.TicketDepartments.Where(x => x.ITOnly != true).OrderBy(x => x.Name), "DepartmentID", "Name");
            if (vm.IT || vm.Admin) vm.DepartmentID = new SelectList(_db.TicketDepartments.OrderBy(x => x.Name), "DepartmentID", "Name");
            vm.AssignedEmployeeID = new SelectList(_db.TicketEmployees.OrderBy(x => x.FirstName), "EmployeeID", "FullName");
            vm.ResourceEmployeeID = new SelectList(_db.TicketEmployees.OrderBy(x => x.FirstName), "EmployeeID", "FullName");
            vm.PriorityID = new SelectList(_db.TicketPriorities, "PriorityID", "Name");
            vm.Status = new SelectList(_db.TicketStatuses.OrderBy(x => x.Name), "StatusID", "Name");
        }

        private void GenerateDropDowns(TicketViewModel vm, TicketProject ticketProject)
        {
            vm.CategoryID = new SelectList(_db.TicketCategories.Where(x => x.isActive == true).OrderBy(x => x.Name), "CategoryID", "Name", ticketProject.CategoryID);
            vm.DepartmentID = new SelectList(_db.TicketDepartments.Where(x => x.ITOnly != true).OrderBy(x => x.Name), "DepartmentID", "Name", ticketProject.DepartmentID);
            if (vm.IT || vm.Admin) vm.DepartmentID = new SelectList(_db.TicketDepartments.OrderBy(x => x.Name), "DepartmentID", "Name", ticketProject.DepartmentID);
            vm.AssignedEmployeeID = new SelectList(_db.TicketEmployees.Where(x => x.InActive != true).OrderBy(x => x.FirstName), "EmployeeID", "FullName", ticketProject.AssignedEmployeeID);
            vm.ResourceEmployeeID = new SelectList(_db.TicketEmployees.Where(x => x.InActive != true).OrderBy(x => x.FirstName), "EmployeeID", "FullName", ticketProject.ResourceEmployeeID);
            vm.PriorityID = new SelectList(_db.TicketPriorities, "PriorityID", "Name", ticketProject.PriorityID);
            vm.Status = new SelectList(_db.TicketStatuses.OrderBy(x => x.Name), "StatusID", "Name", ticketProject.Status);

            vm.TicketVisibility = new SelectList(_db.TicketVisibilities.OrderByDescending(x => x.Id), "Id", "Name");

            GenerateDropDowns(ticketProject);
        }

        [HttpGet]
        public ActionResult ChangeUser(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.SupportAdmin, T("You Do Not Have Permission to Change Users")))
                return new HttpUnauthorizedResult();
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
            if (!Services.Authorizer.Authorize(Permissions.SupportAdmin, T("You Do Not Have Permission to Change Users")))
                return new HttpUnauthorizedResult();
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

        [HttpPost]
        public ActionResult AddNote(int TicketId, string TicketNote, int TicketVisibility, FormCollection collection)
        {
            if (String.IsNullOrEmpty(TicketNote))
            {
                ModelState.AddModelError("Note", "something here maybe?");
                TempData["tempMessage"] = "The Note can not be empty";
                return RedirectToAction("Info", new { id = TicketId });
            }
            else
            {
                try
                {
                    // TODO: Add insert logic here

                    var note = new Time.Data.EntityModels.TimeMFG.TicketNote()
                    {
                        CreatedDate = DateTime.Now,
                        Visibility = TicketVisibility,
                        Note = TicketNote,
                        CreatedBy = User.Identity.Name
                    };
                    if (String.IsNullOrEmpty(note.Note))
                    {
                        ModelState.AddModelError("Note", "something here maybe?");
                        throw new Exception("The Note can not be empty");
                    }

                    var ticket = _db.TicketProjects.FirstOrDefault(c => c.TicketID == TicketId);
                    if (ticket != null)
                    {
                        ticket.TicketNotes.Add(note);
                        _db.SaveChanges();
                        note.SendUpdateNotification();
                    }

                    //note.SendUpdateNotification();

                    return RedirectToAction("Info", new { id = TicketId });
                }
                catch (Exception err)
                {
                    TempData["error"] = "Opps...We had a problem";
                    ErrorTools.SendEmail(Request.Url, err, User.Identity.Name);
                    return RedirectToAction("Info", new { id = TicketId });
                }
            }
        }

        public ActionResult Approval(bool approved, int ticketId)
        {
            if (!Services.Authorizer.Authorize(Permissions.SupportAdmin, T("You Do Not Have Permission to Change Users")) &&
                !Services.Authorizer.Authorize(Permissions.SupportApprover, T("You Do Not Have Permission to Change Users")) &&
                !Services.Authorizer.Authorize(Permissions.SupportIT, T("You Do Not Have Permission to Change Users")))
                return new HttpUnauthorizedResult();

            string msg = "";
            try
            {
                var ticket = _db.TicketProjects.Single(c => c.TicketID == ticketId);
                //if (!ticket.TicketStatusesReference.IsLoaded)
                //    ticket.TicketStatusesReference.Load();

                var oldstatus = ticket.TicketStatus.Name;
                ticket.TicketStatus = approved ? _db.TicketStatuses.First(c => c.StatusID == 2) : _db.TicketStatuses.First(c => c.StatusID == 7);

                ticket.ApprovalDate = DateTime.Now;
                ticket.ApprovedBy = User.Identity.Name;
                ticket.TicketNotes.Add(new TicketNote { Note = String.Format("Status was changed: {0} -> {1}", oldstatus, ticket.TicketStatus.Name), CreatedBy = User.Identity.Name, CreatedDate = DateTime.Now, Visibility = 1 });
                ticket.TicketStatusHistories.Add(new TicketStatusHistory() { TicketStatus = ticket.TicketStatus, CreateDate = DateTime.Now });

                _db.SaveChanges();
                ticket = _db.TicketProjects.Single(c => c.TicketID == ticketId);

                ticket.SendApprovedNotification();                // Added the Approved notification to let Ticket Admins know it is ready to be assigned.
                if (approved) msg = "Ticket has been Approved"; else msg = "Ticket has been Rejected";
            }
            catch (Exception err)
            {
                ErrorTools.SendEmail(Request.Url, err);
                msg = "Error while approving ticketId";
            }

            TempData["message"] = msg;
            return RedirectToAction("Info", new { id = ticketId });
        }

        public ActionResult Complete(bool completed, int ticketId)
        {
            if (!Services.Authorizer.Authorize(Permissions.SupportAdmin, T("You Do Not Have Permission to Complete Tickets")) &&
                !Services.Authorizer.Authorize(Permissions.SupportApprover, T("You Do Not Have Permission to Complete Tickets")) &&
                !Services.Authorizer.Authorize(Permissions.SupportIT, T("You Do Not Have Permission to Complete Tickets")))
                return new HttpUnauthorizedResult();
            string msg = "";
            try
            {
                var ticket = _db.TicketProjects.Single(c => c.TicketID == ticketId);
                //if (!ticket.TicketStatusesReference.IsLoaded)
                //    ticket.TicketStatusesReference.Load();
                //if (!ticket.TicketEmployeesReference.IsLoaded)
                //    ticket.TicketEmployeesReference.Load();

                var oldstatus = ticket.TicketStatus.Name;
                var origTicket = ticket;
                ticket.TicketStatus = completed ? _db.TicketStatuses.First(c => c.StatusID == 5) : _db.TicketStatuses.First(c => c.StatusID == 11);

                ticket.CompletionDate = DateTime.Now;
                ticket.TicketNotes.Add(new TicketNote() { Note = String.Format("Status was changed: {0} -> {1}", oldstatus, ticket.TicketStatus.Name), CreatedBy = User.Identity.Name, CreatedDate = DateTime.Now, Visibility = 1 });
                ticket.TicketStatusHistories.Add(new TicketStatusHistory() { TicketStatus = ticket.TicketStatus, CreateDate = DateTime.Now });
                if (ticket.AssignedEmployeeID != ticket.ResourceEmployeeID)
                {
                    string oldEmp = String.Format("{0} {1}", ticket.TicketEmployee.FirstName, ticket.TicketEmployee.LastName);
                    string newEmp = String.Format("{0} {1}", ticket.TicketEmployee1.FirstName, ticket.TicketEmployee1.LastName);
                    ticket.AssignedEmployeeID = ticket.ResourceEmployeeID;
                    ticket.TicketEmployee = ticket.TicketEmployee1;
                    ticket.TicketNotes.Add(new TicketNote() { Note = String.Format("Reset Assigned To from {0} to {1}", oldEmp, newEmp), CreatedBy = User.Identity.Name, CreatedDate = DateTime.Now, Visibility = 1 });
                }
                _db.SaveChanges();

                ticket.SendUpdateNotificationToAssigned();                // Added the completion notification to let the assigned person know it is complete.
                if (completed) msg = "Ticket has been completed"; else msg = "Ticket has been denied completion";
            }
            catch (Exception err)
            {
                ErrorTools.SendEmail(Request.Url, err);
            }

            TempData["message"] = msg;
            return RedirectToAction("Info", new { id = ticketId });
        }

        public ActionResult CancelTicket(bool cancelled, int ticketId)
        {
            if (!Services.Authorizer.Authorize(Permissions.SupportAdmin, T("You Do Not Have Permission to Cancel Tickets")) &&
                   !Services.Authorizer.Authorize(Permissions.SupportApprover, T("You Do Not Have Permission to Cancel Tickets")) &&
                   !Services.Authorizer.Authorize(Permissions.SupportIT, T("You Do Not Have Permission to Cancel Tickets")))

                return new HttpUnauthorizedResult();
            string msg = "";
            var result = "";
            string statusMessage = "";

            try
            {
                var ticket = _db.TicketProjects.Single(c => c.TicketID == ticketId);
                //if (!ticket.TicketStatusesReference.IsLoaded)
                //    ticket.TicketStatusesReference.Load();
                //if (!ticket.TicketEmployeesReference.IsLoaded)
                //    ticket.TicketEmployeesReference.Load();

                var oldstatus = ticket.TicketStatus.Name;
                if (cancelled)
                {
                    ticket.TicketStatus = _db.TicketStatuses.Where(c => c.StatusID == 13).First();   // Status = Cancelled Request
                    statusMessage = "This ticket has been cancelled, and will not be worked on or completed.  If you feel this is an error, please contact your Supervisor, or IT to discuss the issue";
                    ticket.CompletionDate = DateTime.Now;
                }
                else
                {
                    ticket.TicketStatus = _db.TicketStatuses.Where(c => c.StatusID == 14).First(); // Status = Cancellation Rejected
                    statusMessage += "The ticket cancellation has been rejected.  Please look at the ticket, and continue working on it, or get with the requestor, and discuss how it should be handled.";
                    statusMessage += "<br />If you feel this is an error, please contact your Supervisor, or IT to discuss the issue";
                }

                ticket.TicketNotes.Add(new TicketNote() { Note = String.Format("Status was changed: {0} -> {1}", oldstatus, ticket.TicketStatus.Name), CreatedBy = User.Identity.Name, CreatedDate = DateTime.Now, Visibility = 1 });
                ticket.TicketStatusHistories.Add(new TicketStatusHistory { TicketStatus = ticket.TicketStatus, CreateDate = DateTime.Now });

                _db.SaveChanges();

                // send notifications to the submitter
                ticket.SendUpdateNotification(statusMessage);

                // send notification to the person assigned to the ticket
                if (ticket.TicketEmployee != null)
                {
                    if (ticket.TicketEmployee.NTLogin != ticket.RequestedBy)
                        ticket.SendUpdateNotificationToAssigned(statusMessage);                // Added the completion notification to let the assigned person know it is complete.
                }

                result = (String.Format("Ticket has been updated to '{0}'.", ticket.TicketStatus.Name));
                result += String.Format(" <a href='/support/info/{0}'>Refresh</a> page to view changes", ticket.TicketID);
                if (cancelled) msg = "Ticket has been cancelled"; else msg = "Ticket has been denied cancellation";
            }
            catch (Exception err)
            {
                //result = "Could not process your request at this time";
                ErrorTools.SendEmail(Request.Url, err);
            }

            TempData["message"] = msg;
            return RedirectToAction("Info", new { id = ticketId });
        }

        #region TicketTasks

        // GET: TicketTasks
        public async Task<ActionResult> MyTasks()
        {
            var user = System.Web.HttpContext.Current.User.Identity.Name;
            var ticketTasks = _db.TicketTasks.Where(x => x.TicketEmployee.NTLogin == user && x.Completed != true).OrderBy(x => x.TicketProject.Title).ThenBy(x => x.ID).Include(t => t.TicketEmployee).Include(t => t.TicketProject);
            return View(await ticketTasks.ToListAsync());
        }

        // GET: TicketTasks/Create
        [HttpGet]
        public ActionResult AddTask(int id)
        {
            var task = new TicketTask { TicketID = id };
            ViewBag.AssignedTo = new SelectList(_db.TicketEmployees.OrderBy(x => x.FirstName), "EmployeeID", "FullName");
            //ViewBag.TicketID = new SelectList(_db.TicketProjects, "TicketID", "Title");
            return View(task);
        }

        // POST: TicketTasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddTask([Bind(Exclude = "ID")] TicketTask ticketTask)
        {
            ticketTask.RequestDate = DateTime.Now;
            ticketTask.Requestor = System.Web.HttpContext.Current.User.Identity.Name;

            if (ModelState.IsValid)
            {
                _db.TicketTasks.Add(ticketTask);
                await _db.SaveChangesAsync();
                if (ticketTask.AssignedTo != null)
                {
                    var ticket = _db.TicketProjects.Single(x => x.TicketID == ticketTask.TicketID);
                    var emp = _db.TicketEmployees.Single(x => x.EmployeeID == ticketTask.AssignedTo);
                    ticketTask.TicketEmployee = emp;
                    ticket.SendTaskAssignmentNotification(ticketTask);
                }
                return RedirectToAction("Info", new { id = ticketTask.TicketID });
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
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EditTask(TicketTask task)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var original = _db.TicketTasks.AsNoTracking().Single(x => x.ID == task.ID);

        //        _db.Entry(task).State = EntityState.Modified;
        //        _db.SaveChanges();

        //        if (original.AssignedTo != null && (task.AssignedTo == null || original.AssignedTo != task.AssignedTo))
        //        {
        //            var ticket = _db.TicketProjects.Single(x => x.TicketID == task.TicketID);
        //            var emp = _db.TicketEmployees.Single(x => x.EmployeeID == task.AssignedTo);
        //            task.TicketEmployee = emp;
        //            ticket.SendTaskAssignmentNotification(task);
        //        }

        //        if (task.Completed && !original.Completed)
        //        {
        //            var ticket = _db.TicketProjects.Single(x => x.TicketID == task.TicketID);
        //            var emp = _db.TicketEmployees.Single(x => x.EmployeeID == task.AssignedTo);
        //            task.TicketEmployee = emp;
        //            ticket.SendTaskCompletedNotification(task);
        //        }

        //        return RedirectToAction("Info", new { id = task.TicketID });
        //    }

        //    ViewBag.AssignedTo = new SelectList(_db.TicketEmployees, "EmployeeID", "LastName", task.AssignedTo);
        //    return View(task);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditTask(TicketTask ticketTask)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(ticketTask).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Info", new { id = ticketTask.TicketID });
            }
            ViewBag.AssignedTo = new SelectList(_db.TicketEmployees, "EmployeeID", "LastName", ticketTask.AssignedTo);
            //ViewBag.TicketID = new SelectList(_db.TicketProjects, "TicketID", "Title", ticketTask.TicketID);
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
            return RedirectToAction("Info", new { id = ticketID });
        }

        #endregion TicketTasks
    }
}