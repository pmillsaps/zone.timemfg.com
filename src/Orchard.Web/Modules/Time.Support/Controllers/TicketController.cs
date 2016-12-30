using MoreLinq;
using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.TimeMFG;
using Time.Data.Models.MessageQueue;
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

            var myTickets = tickets.Where(x => x.TicketEmployee.NTLogin == HttpContext.User.Identity.Name || x.TicketEmployee1.NTLogin == HttpContext.User.Identity.Name);

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

        public ActionResult Index(string sortOrder)
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

            ViewBag.PrioritySortParm = String.IsNullOrEmpty(sortOrder) ? "priority_desc" : "";
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            switch (sortOrder)
            {
                case "priority_desc":
                    qry = qry.OrderByDescending(x => x.PriorityID).ToList();
                    break;
                case "name_desc":
                    qry = qry.OrderBy(x => x.Title).ToList();
                    break;
                default:
                    qry = qry.OrderBy(x => x.TicketID).ToList();
                    break;
            }

            return View(qry);
        }

        public ActionResult Info(int id)
        {
            var qry = _db.TicketProjects.FirstOrDefault(c => c.TicketID == id);
            if (qry == null) return RedirectToAction("Index");
            var user = HttpContext.User.Identity.Name;

            var vm = new TicketViewModel() { Ticket = qry, Tasks = qry.TicketTasks, TicketId = id };
            if (Services.Authorizer.Authorize(Permissions.SupportAdmin))
            {
                vm.Admin = true;
            }
            if (Services.Authorizer.Authorize(Permissions.SupportIT))
            {
                vm.IT = true;
            }

            if (Services.Authorizer.Authorize(Permissions.SupportApprover) || ADHelper.GetGroupNames(user).Contains("SupportTicketApprover"))
            {
                vm.Approver = true;
            }

            if (qry.TicketEmployee != null && user.ToUpper() == qry.TicketEmployee.NTLogin.ToUpper()) vm.AssignedToMe = true;

            GenerateDropDowns(vm, qry);

            // Obsolete code section ?
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
            //if ((string)TempData["message"] != "") ViewBag.Message = TempData["message"];
            if (!String.IsNullOrEmpty((string)TempData["ErrorMessage"])) ViewBag.ErrorMessage = TempData["ErrorMessage"];
            if (!String.IsNullOrEmpty((string)TempData["Notice"])) ViewBag.Notice = TempData["Notice"];

            ViewBag.Title = "Ticket Info";
            return View(vm);
        }

        public ActionResult GetAttachment(int id)
        {
            var att = _db.TicketAttachments.Where(c => c.AttachmentID == id).First();
            var path = Server.MapPath(String.Format(@"~\Modules\Time.Support\Content\AttachmentFiles\{0}\{1}", att.TicketProject.TicketID, att.FileName));
            var fi = new FileInfo(path);
            if (!System.IO.File.Exists(path))
            {
                string msg = "File Not Found - Please get with Support to report this issue";
                TempData["message"] = msg;
                RedirectToAction("Info", new { id = att.TicketID });
            }
            byte[] filedata = System.IO.File.ReadAllBytes(path);
            string contentType = MimeMapping.GetMimeMapping(path);
            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = path,
                Inline = false
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(filedata, contentType);
        }

        public ActionResult Search(string Search, bool IncludeComplete)
        {
            if (string.IsNullOrEmpty(Search)) return RedirectToAction("Index");
            if (Regex.IsMatch(Search, @"#\d"))
                return RedirectToAction("Info", new { id = Search.Substring(1) });

            //var qry = _db.TicketProjects.AsQueryable();
            var qry = from t in _db.TicketProjects
                      join emp in _db.TicketEmployees on t.AssignedEmployeeID equals emp.EmployeeID
                      select t;
                      

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
                    || x.TicketTasks.Any(t => t.Task.Contains(item) || t.Notes.Contains(item))
                    || x.RequestedBy.Contains(item)
                    || x.TicketEmployee.NTLogin.Contains(item)
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

                var user = HttpContext.User.Identity.Name;
                var groups = ADHelper.GetGroupNames(user);

                if (groups != null && groups.Contains("SupportTicketApprover"))
                {
                    ticketProject.Status = 2;
                }

                //if (Services.Authorizer.Authorize(Permissions.SupportApprover)) ticketProject.Status = 2;
                ticketProject.TicketStatus = _db.TicketStatuses.Find(ticketProject.Status);
                ticketProject.RequestedDate = DateTime.Now;
                ticketProject.RequestedBy = user;
                ticketProject.RequestedByFriendly = HttpContext.User.Identity.Name;
                _db.TicketStatusHistories.Add(new TicketStatusHistory { TicketStatus = ticketProject.TicketStatus, CreateDate = DateTime.Now });

                // Approval COdeGen
                if (ticketProject.Status == 1)
                {
                    var codegen = new CreateRandomCode();
                    var code = codegen.GenerateCode(12);
                    while (_db.TicketProjects.Any(x => x.ApprovalCode == code))
                    {
                        // regenerate a new code if it already exists
                        code = codegen.GenerateCode(12);
                    }
                    ticketProject.ApprovalCode = code;
                }

                _db.TicketProjects.Add(ticketProject);
                _db.SaveChanges();
                // ticketProject.SendNewTicketNotification();
                var command = new TicketNotificationMessage
                {
                    TicketId = ticketProject.TicketID,
                    Notification = TicketNotificationMessage.NotificationType.NewTicket,
                    Sender = HttpContext.User.Identity.Name
                };
                var success = MSMQ.SendQueueMessage(command, MessageType.TicketNotification.Value);
                if (ticketProject.Status == 2)
                {
                    var cmd = new TicketNotificationMessage
                    {
                        TicketId = ticketProject.TicketID,
                        Notification = TicketNotificationMessage.NotificationType.Approved,
                        Sender = HttpContext.User.Identity.Name
                    };
                    success = MSMQ.SendQueueMessage(cmd, MessageType.TicketNotification.Value);
                    // ticketProject.SendApprovedNotification();
                }
                else
                {
                    var cmd = new TicketNotificationMessage
                    {
                        TicketId = ticketProject.TicketID,
                        Notification = TicketNotificationMessage.NotificationType.SupervisorNewTicket,
                        Sender = HttpContext.User.Identity.Name
                    };
                    success = MSMQ.SendQueueMessage(cmd, MessageType.TicketNotification.Value);
                    //ticketProject.SendSupervisorNewTicketNotification();
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

            if (ticketProject.AssignedEmployeeID != null && ticket.AssignedEmployeeID != ticketProject.AssignedEmployeeID)
            {
                var emp = _db.TicketEmployees.Single(x => x.EmployeeID == ticketProject.AssignedEmployeeID);

                if (ticket.AssignedEmployeeID != null && ticket.AssignedEmployeeID != 0)
                {
                    msg += string.Format("Zone: Assigned Employee was changed: {0} -> {1}" + Environment.NewLine, ticket.TicketEmployee.FullName, emp.FullName);
                }
                else
                {
                    msg += string.Format("Zone: Assigned ticket to {0}" + Environment.NewLine, emp.FullName);
                }

                if (ticketProject.Status < 3) { ticketProject.Status = 3; }   // Assigned = 3
                ticket.AssignedEmployeeID = ticketProject.AssignedEmployeeID;
                ticket.TicketEmployee = emp;

                if (ticket.ResourceEmployeeID == null)
                {
                    ticket.ResourceEmployeeID = ticket.AssignedEmployeeID;
                    ticketProject.ResourceEmployeeID = ticket.AssignedEmployeeID;
                    msg += string.Format("Zone: Assigned Ticket Resource to {0}" + Environment.NewLine, emp.FullName);
                }
                _db.SaveChanges();

                //ticket.SendAssignmentNotification();
                var command = new TicketNotificationMessage
                {
                    TicketId = ticketProject.TicketID,
                    Notification = TicketNotificationMessage.NotificationType.Assignment,
                    Sender = HttpContext.User.Identity.Name
                };
                var success = MSMQ.SendQueueMessage(command, MessageType.TicketNotification.Value);
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

            if (ticketProject.ResourceEmployeeID != null && ticket.ResourceEmployeeID != ticketProject.ResourceEmployeeID)
            {
                var emp = _db.TicketEmployees.Single(x => x.EmployeeID == ticketProject.ResourceEmployeeID);

                if (emp != null)
                {
                    msg += string.Format("Resource Employee was changed: {0} -> {1}", ticket.TicketEmployee.FullName, emp.FullName);
                    ticket.ResourceEmployeeID = ticketProject.ResourceEmployeeID;
                    ticket.TicketEmployee1 = emp;
                }
                else
                {
                    msg += string.Format("Assigned ticket resource to {0}", emp.FullName);
                    ticket.ResourceEmployeeID = ticketProject.ResourceEmployeeID;
                    msg += string.Format("Assigned Ticket Resource to {0}", emp.FullName);
                }

                //ticket.AssignedEmployeeID = ticketProject.AssignedEmployeeID;
                //ticket.TicketEmployee = emp;
                //ticket.SendAssignmentNotification();
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

            if (ticket.PriorityID != ticketProject.PriorityID)
            {
                var priority = _db.TicketPriorities.Single(x => x.PriorityID == ticketProject.PriorityID);
                var oldPriority = _db.TicketPriorities.Single(x => x.PriorityID == ticket.PriorityID);
                ticket.PriorityID = ticketProject.PriorityID;
                string updateNote = string.Format("Zone: Priority was changed: {0} -> {1}" + Environment.NewLine, oldPriority.Name, priority.Name);
                msg += updateNote;
                ticket.TicketNotes.Add(new TicketNote() { Note = updateNote, CreatedBy = User.Identity.Name, CreatedDate = DateTime.Now, Visibility = 1 });
                _db.SaveChanges();
                if (ticket.AssignedEmployeeID != null)
                {
                    // Send update notification
                    // ticket.SendUpdateNotificationToAssigned(statusMessage: updateNote);
                    var command = new TicketNotificationMessage
                    {
                        TicketId = ticketProject.TicketID,
                        Notification = TicketNotificationMessage.NotificationType.UpdateAssigned,
                        Sender = HttpContext.User.Identity.Name,
                        NoteId = ticket.TicketNotes.OrderByDescending(x => x.CreatedDate).First().NoteID
                    };
                    var success = MSMQ.SendQueueMessage(command, MessageType.TicketNotification.Value);
                }
            }

            if (ticket.DepartmentID != ticketProject.DepartmentID)
            {
                var dept = _db.TicketDepartments.Single(x => x.DepartmentID == ticketProject.DepartmentID);
                var oldDept = _db.TicketDepartments.Single(x => x.DepartmentID == ticket.DepartmentID);
                ticket.DepartmentID = ticketProject.DepartmentID;
                string updateNote = string.Format("Zone: Department was changed: {0} -> {1}" + Environment.NewLine, oldDept.Name, dept.Name);
                msg += updateNote;
                ticket.TicketNotes.Add(new TicketNote() { Note = updateNote, CreatedBy = User.Identity.Name, CreatedDate = DateTime.Now, Visibility = 1 });
                _db.SaveChanges();
            }

            if (ticket.CategoryID != ticketProject.CategoryID)
            {
                var cat = _db.TicketCategories.Find(ticketProject.CategoryID);
                string updateNote = string.Format("Zone: Category was changed: {0} -> {1}" + Environment.NewLine, ticket.TicketCategory.Name, cat.Name);
                ticket.CategoryID = ticketProject.CategoryID;
                msg += updateNote;
                ticket.TicketNotes.Add(new TicketNote() { Note = updateNote, CreatedBy = User.Identity.Name, CreatedDate = DateTime.Now, Visibility = 1 });
                _db.SaveChanges();
            }

            if (ticket.ResourceEmployeeID != ticketProject.ResourceEmployeeID)
            {
                string updateNote = "";
                var emp = _db.TicketEmployees.Single(x => x.EmployeeID == ticketProject.ResourceEmployeeID);
                if (ticket.TicketEmployee1 != null)
                    updateNote = string.Format("Zone: Resource Employee was changed: {0} -> {1}" + Environment.NewLine, ticket.TicketEmployee1.FullName, emp.FullName);
                else
                    updateNote = string.Format("Zone: Set Resource Employee to {0}" + Environment.NewLine, emp.FullName);
                msg += updateNote;
                ticket.TicketNotes.Add(new TicketNote() { Note = updateNote, CreatedBy = User.Identity.Name, CreatedDate = DateTime.Now, Visibility = 1 });
                _db.SaveChanges();
            }

            if (ticket.Status != ticketProject.Status)
            {
                var oldstatus = ticket.TicketStatus;
                var stat = _db.TicketStatuses.Single(x => x.StatusID == ticketProject.Status);
                ticket.Status = ticketProject.Status;
                string updateNote = string.Format("Zone: Status was changed: {0} -> {1}" + Environment.NewLine, ticket.TicketStatus.Name, stat.Name);
                msg += updateNote;
                ticket.TicketNotes.Add(new TicketNote() { Note = updateNote, CreatedBy = User.Identity.Name, CreatedDate = DateTime.Now, Visibility = 1 });
                ticket.TicketStatusHistories.Add(new TicketStatusHistory { CreateDate = DateTime.Now, TicketID = ticket.TicketID, StatusID = ticketProject.Status });
                _db.SaveChanges();

                if (ticket.TicketStatus.isSupRequest)
                {
                    var command = new TicketNotificationMessage
                    {
                        TicketId = ticketProject.TicketID,
                        Notification = TicketNotificationMessage.NotificationType.SupervisorNewTicket,
                        Sender = HttpContext.User.Identity.Name,
                    };
                    var success = MSMQ.SendQueueMessage(command, MessageType.TicketNotification.Value);
                }

                if (ticket.TicketStatus.isReadyToComplete)
                {
                    //ticket.SendCompletionPendingNotification();
                    var command = new TicketNotificationMessage
                    {
                        TicketId = ticketProject.TicketID,
                        Notification = TicketNotificationMessage.NotificationType.CompletionPending,
                        Sender = HttpContext.User.Identity.Name,
                        NoteId = ticket.TicketNotes.OrderByDescending(x => x.CreatedDate).First().NoteID
                    };
                    var success = MSMQ.SendQueueMessage(command, MessageType.TicketNotification.Value);
                }
                if (ticket.TicketStatus.isOpen == false && oldstatus.isOpen == true)
                {
                    ticket.CompletionDate = DateTime.Now;
                }
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

        public ActionResult All(string sortOrder)
        {
            var login = HttpContext.User.Identity.Name.ToUpper();
            var tickets = _db.TicketProjects.Where(x => x.TicketStatus.isOpen && (x.TicketEmployee.NTLogin.ToUpper() == login || x.TicketEmployee1.NTLogin.ToUpper() == login));
            ViewBag.Title = "All My Tickets";

            ViewBag.PrioritySortParm = String.IsNullOrEmpty(sortOrder) ? "priority_desc" : "";
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            switch (sortOrder)
            {
                case "priority_desc":
                    tickets = tickets.OrderByDescending(x => x.PriorityID);
                    break;
                case "name_desc":
                    tickets = tickets.OrderBy(x => x.Title);
                    break;
                default:
                    tickets = tickets.OrderBy(x => x.TicketID);
                    break;
            }

            return View("Index", tickets);
        }

        public ActionResult MyStatus(int id, string sortOrder)
        {
            var login = HttpContext.User.Identity.Name;
            var tickets = _db.TicketProjects.Where(x => x.TicketStatus.isOpen && x.TicketStatus.StatusID == id && (x.TicketEmployee.NTLogin == login || x.TicketEmployee1.NTLogin == login));
            ViewBag.Title = string.Format("[{0}] My Tickets", _db.TicketStatuses.Single(x => x.StatusID == id).Name);

            ViewBag.PrioritySortParm = String.IsNullOrEmpty(sortOrder) ? "priority_desc" : "";
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            switch (sortOrder)
            {
                case "priority_desc":
                    tickets = tickets.OrderByDescending(x => x.PriorityID);
                    break;
                case "name_desc":
                    tickets = tickets.OrderBy(x => x.Title);
                    break;
                default:
                    tickets = tickets.OrderBy(x => x.TicketID);
                    break;
            }

            return View("Index", tickets);
        }

        public ActionResult Status(int id, string sortOrder)
        {
            var tickets = _db.TicketProjects.Where(x => x.TicketStatus.isOpen && x.TicketStatus.StatusID == id);
            ViewBag.Title = string.Format("[{0}] Status", _db.TicketStatuses.Single(x => x.StatusID == id).Name);

            ViewBag.PrioritySortParm = String.IsNullOrEmpty(sortOrder) ? "priority_desc" : "";
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            switch (sortOrder)
            {
                case "priority_desc":
                    tickets = tickets.OrderByDescending(x => x.PriorityID);
                    break;
                case "name_desc":
                    tickets = tickets.OrderBy(x => x.Title);
                    break;
                default:
                    tickets = tickets.OrderBy(x => x.TicketID);
                    break;
            }

            return View("Index", tickets);
        }

        public ActionResult Employee(int id)
        {
            var tickets = _db.TicketProjects.Where(x => x.TicketStatus.isOpen && (x.TicketEmployee.EmployeeID == id || x.TicketEmployee1.EmployeeID == id));
            var employee = _db.TicketEmployees.Single(x => x.EmployeeID == id);
            ViewBag.Title = string.Format("[{0} {1}] Employee", employee.FirstName, employee.LastName);
            return View("Index", tickets);
        }

        public ActionResult Category(int id, string sortOrder)
        {
            var tickets = _db.TicketProjects.Where(x => x.TicketStatus.isOpen && x.TicketCategory.CategoryID == id);
            ViewBag.Title = string.Format("[{0}] Category", _db.TicketCategories.Single(x => x.CategoryID == id).Name);

            ViewBag.PrioritySortParm = String.IsNullOrEmpty(sortOrder) ? "priority_desc" : "";
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            switch (sortOrder)
            {
                case "priority_desc":
                    tickets = tickets.OrderByDescending(x => x.PriorityID);
                    break;
                case "name_desc":
                    tickets = tickets.OrderBy(x => x.Title);
                    break;
                default:
                    tickets = tickets.OrderBy(x => x.TicketID);
                    break;
            }

            return View("Index", tickets);
        }

        public ActionResult Department(int id, string sortOrder)
        {
            var tickets = _db.TicketProjects.Where(x => x.TicketStatus.isOpen && x.TicketDepartment.DepartmentID == id);
            ViewBag.Title = string.Format("[{0}] Department", _db.TicketDepartments.Single(x => x.DepartmentID == id).Name);

            ViewBag.PrioritySortParm = String.IsNullOrEmpty(sortOrder) ? "priority_desc" : "";
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            switch (sortOrder)
            {
                case "priority_desc":
                    tickets = tickets.OrderByDescending(x => x.PriorityID);
                    break;
                case "name_desc":
                    tickets = tickets.OrderBy(x => x.Title);
                    break;
                default:
                    tickets = tickets.OrderBy(x => x.TicketID);
                    break;
            }

            return View("Index", tickets);
        }

        //GET: /Ticket/MyOpenTickets
        public ActionResult MyOpenTickets(string sortOrder)
        {
            var login = HttpContext.User.Identity.Name;
            var tickets = _db.TicketProjects.Where(x => x.TicketStatus.isOpen && x.RequestedBy == login);

            ViewBag.Title = "My Open Tickets";

            ViewBag.PrioritySortParm = String.IsNullOrEmpty(sortOrder) ? "priority_desc" : "";
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            switch (sortOrder)
            {
                case "priority_desc":
                    tickets = tickets.OrderByDescending(x => x.PriorityID);
                    break;
                case "name_desc":
                    tickets = tickets.OrderBy(x => x.Title);
                    break;
                default:
                    tickets = tickets.OrderBy(x => x.TicketSequence == null).ThenBy(x => x.TicketSequence).ThenBy(x => x.TicketID);
                    break;
            }

            return View("Index", tickets);
        }

        //POST: /Ticket/MyOpenTickets
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MyOpenTickets(TicketProject tickets, string command, TicketProject ticketup, TicketProject ticketdown)
        {
            var sequence = _db.TicketProjects.Where(x => x.TicketID == tickets.TicketID && x.TicketSequence != null).Select(x => x.TicketSequence).ToList();
            var existingsequence = _db.TicketProjects.Where(x => x.TicketStatus.isOpen && x.RequestedBy == tickets.RequestedBy).Max(x => x.TicketSequence);

            if (command == "UpArrow") //moves ticket priority up
            {
                if (sequence.Count == 0 && (existingsequence == 0 || existingsequence == null))
                {
                    tickets.TicketSequence = 1;
                }
                else if (sequence.Count == 0 && (existingsequence != 0 || existingsequence != null))
                {
                    tickets.TicketSequence = existingsequence + 1;
                }
                else if (sequence.Count != 0 && (existingsequence != 0 || existingsequence != null))
                {
                    var newpos = sequence.Single() - 1;

                    ticketdown = _db.TicketProjects.FirstOrDefault(x => x.RequestedBy == tickets.RequestedBy && x.TicketSequence == newpos);

                    tickets.TicketSequence = newpos;
                    ticketdown.TicketSequence = sequence.Single();
                }

                if (ModelState.IsValid)
                {
                    _db.Entry(tickets).State = EntityState.Modified;
                    _db.SaveChanges();
                    var ticket = _db.TicketProjects.First(x => x.TicketID == tickets.TicketID);
                    return RedirectToAction("MyOpenTickets");
                }
            }
            else //moves ticket priority down
            {
                if ((sequence.Count != 0 && sequence.Count != existingsequence) || existingsequence == 1)
                {
                    var newpos = sequence.Single() + 1;

                    ticketup = _db.TicketProjects.FirstOrDefault(x => x.RequestedBy == tickets.RequestedBy && x.TicketSequence == newpos);

                    if (sequence.Count != 0 && sequence.Single() == existingsequence)
                    {
                        tickets.TicketSequence = null;
                    }
                    else
                    {
                        tickets.TicketSequence = newpos;
                        ticketup.TicketSequence = sequence.Single();
                }

                    if (ModelState.IsValid)
                    {
                        _db.Entry(tickets).State = EntityState.Modified;
                        _db.SaveChanges();
                        var ticket = _db.TicketProjects.First(x => x.TicketID == tickets.TicketID);
                        return RedirectToAction("MyOpenTickets");
                    }
                }
            }

            return View(tickets);
        }


        public ActionResult MyTickets(string sortOrder)
        {
            var login = HttpContext.User.Identity.Name;
            var tickets = _db.TicketProjects.Where(x => x.RequestedBy == login);
            ViewBag.Title = "All My Tickets";
            ViewBag.SubTitle = string.Format("{0} Tickets, {1} Open Tickets", tickets.Count(), tickets.Count(x => x.TicketStatus.isOpen));

            ViewBag.PrioritySortParm = String.IsNullOrEmpty(sortOrder) ? "priority_desc" : "";
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            switch (sortOrder)
            {
                case "priority_desc":
                    tickets = tickets.OrderByDescending(x => x.PriorityID);
                    break;
                case "name_desc":
                    tickets = tickets.OrderBy(x => x.Title);
                    break;
                default:
                    tickets = tickets.OrderBy(x => x.TicketID);
                    break;
            }

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
            vm.CategoryID = new SelectList(_db.TicketCategories.Where(x => x.isActive == true).OrderBy(x => x.Name).ToList(), "CategoryID", "Name", ticketProject.CategoryID);
            ViewBag.CategoryID = vm.CategoryID;

            vm.DepartmentID = new SelectList(_db.TicketDepartments.Where(x => x.ITOnly != true).OrderBy(x => x.Name).ToList(), "DepartmentID", "Name", ticketProject.DepartmentID);
            if (vm.IT || vm.Admin) vm.DepartmentID = new SelectList(_db.TicketDepartments.OrderBy(x => x.Name).ToList(), "DepartmentID", "Name", ticketProject.DepartmentID);
            ViewBag.DepartmentID = vm.DepartmentID;

            vm.AssignedEmployeeID = new SelectList(_db.TicketEmployees.Where(x => x.InActive != true).OrderBy(x => x.FirstName).ToList(), "EmployeeID", "FullName", ticketProject.AssignedEmployeeID);
            ViewBag.AssignedEmployeeID = vm.AssignedEmployeeID;

            vm.ResourceEmployeeID = new SelectList(_db.TicketEmployees.Where(x => x.InActive != true).OrderBy(x => x.FirstName).ToList(), "EmployeeID", "FullName", ticketProject.ResourceEmployeeID);
            ViewBag.ResourceEmployeeID = vm.ResourceEmployeeID;

            vm.PriorityID = new SelectList(_db.TicketPriorities.ToList(), "PriorityID", "Name", ticketProject.PriorityID);
            ViewBag.PriorityID = vm.PriorityID;

            vm.Status = new SelectList(_db.TicketStatuses.OrderBy(x => x.Name).ToList(), "StatusID", "Name", ticketProject.Status);
            if (!vm.IT && !vm.Admin)
            {
                if ((ticketProject.TicketEmployee != null && HttpContext.User.Identity.Name.ToUpper() == ticketProject.TicketEmployee.NTLogin.ToUpper()) || HttpContext.User.Identity.Name.ToUpper() == ticketProject.RequestedBy.ToUpper())
                {
                    vm.Status = new SelectList(_db.TicketStatuses.Where(x => x.isAssignedVisible).OrderBy(x => x.Name).ToList(), "StatusID", "Name", ticketProject.Status);
                }
            }
            ViewBag.Status = vm.Status;

            vm.TicketVisibility = new SelectList(_db.TicketVisibilities.OrderByDescending(x => x.Id), "Id", "Name");
            ViewBag.TicketVisibility = vm.TicketVisibility;
        }

        [HttpGet]
        public ActionResult ChangeUser(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.SupportAdmin, T("You Do Not Have Permission to Change Users")))
                return new HttpUnauthorizedResult();
            TicketProject ticketProject = _db.TicketProjects.Find(id);

            var requestors = new SelectList(_db.TicketProjects.DistinctBy(x => x.RequestedBy).OrderBy(x => x.RequestedBy).ToList(), "RequestedBy", "RequestedBy", ticketProject.RequestedBy);
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
        public ActionResult AddNote(int TicketId, string TicketNote, int TicketVisibility, string operation, HttpPostedFileBase fileBlob, FormCollection collection)
        {
            //HttpPostedFileBase file = Request.Files["fileBlob"];
            if (operation == "Upload File")
            {
                return UploadFile(TicketId, TicketNote, TicketVisibility, operation, fileBlob, collection);
            }

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

                    var note = new TicketNote()
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
                        //note.SendUpdateNotification();
                        var command = new TicketNotificationMessage
                        {
                            TicketId = ticket.TicketID,
                            Notification = TicketNotificationMessage.NotificationType.Update,
                            Sender = HttpContext.User.Identity.Name,
                            NoteId = ticket.TicketNotes.OrderByDescending(x => x.CreatedDate).First().NoteID
                        };
                        var success = MSMQ.SendQueueMessage(command, MessageType.TicketNotification.Value);
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

        private ActionResult UploadFile(int ticketId, string ticketNote, int ticketVisibility, string operation, HttpPostedFileBase fileBlob, FormCollection collection)
        {
            if (String.IsNullOrEmpty(ticketNote) || fileBlob == null)
            {
                if (fileBlob == null) TempData["ErrorMessage"] += "You HAVE to Select A File To Attach<br />";
                if (String.IsNullOrEmpty(ticketNote)) TempData["ErrorMessage"] += "The Note/File Description can not be empty<br />";
                return RedirectToAction("Info", new { id = ticketId });
            }
            else
            {
                try
                {
                    // File Logic being added here
                    var ticket = _db.TicketProjects.FirstOrDefault(c => c.TicketID == ticketId);
                    if (ticket == null)
                    {
                        TempData["ErrorMessage"] += "The Selected Ticket Does Not Exist<br />";
                        return RedirectToAction("Info", new { id = ticketId });
                    }
                    var f = new TicketAttachment();

                    f.FileName = fileBlob.FileName.Substring(fileBlob.FileName.LastIndexOf("\\") + 1).ToLower();
                    f.FileExt = f.FileName.Substring(f.FileName.LastIndexOf(".") + 1).ToLower();
                    f.Description = ticketNote;
                    f.UploadedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    f.UploadedDate = DateTime.Now;
                    var attach = ticket.TicketAttachments.FirstOrDefault(x => x.FileName.ToUpper() == f.FileName.ToUpper());

                    try
                    {
                        fileBlob.StoreAttachmentFile(ticket.TicketID);
                        if (attach == null)
                            ticket.TicketAttachments.Add(f);
                        else
                        {
                            attach.Description = string.Format(@"{0}{3}{1}{3}{2}", attach.Description, f.UploadedDate.ToLocalTime(), f.Description, Environment.NewLine);
                        }
                        _db.SaveChanges();
                        TempData["Notice"] = "File was uploaded successfully!";
                        var command = new TicketNotificationMessage
                        {
                            TicketId = ticket.TicketID,
                            Notification = TicketNotificationMessage.NotificationType.Update,
                            Sender = HttpContext.User.Identity.Name,
                        };
                        var success = MSMQ.SendQueueMessage(command, MessageType.TicketNotification.Value);
                        return RedirectToAction("Info", new { id = ticket.TicketID });
                    }
                    catch (Exception err)
                    {
                        TempData["error"] = "Could not save file, Please try again later";
                        ErrorTools.SendEmail(Request.Url, err, User.Identity.Name);
                    }

                    return RedirectToAction("Info", new { id = ticketId });
                }
                catch (Exception err)
                {
                    TempData["ErrorMessage"] = "Opps...We had a problem";
                    ErrorTools.SendEmail(Request.Url, err, User.Identity.Name);
                    return RedirectToAction("Info", new { id = ticketId });
                }
            }
        }

        public ActionResult Approval(bool approved, int ticketId)
        {
            var user = HttpContext.User.Identity.Name;
            if (!ADHelper.GetGroupNames(user).Contains("SupportTicketApprover"))
            {
                // This will throw an error in all cases except if SupportAdmin
                if (!Services.Authorizer.Authorize(Permissions.SupportAdmin, T("You Do Not Have Permission to Approve Tickets"))
                    //|| Services.Authorizer.Authorize(Permissions.SupportAdmin, T("You Do Not Have Permission to Approve Tickets"))
                    )
                    return new HttpUnauthorizedResult();
            }

            //if (!Services.Authorizer.Authorize(Permissions.SupportAdmin, T("You Do Not Have Permission to Approve Tickets")) &&
            //    !Services.Authorizer.Authorize(Permissions.SupportApprover, T("You Do Not Have Permission to Approve Tickets")) &&
            //    !Services.Authorizer.Authorize(Permissions.SupportIT, T("You Do Not Have Permission to Approve Tickets")))
            //    return new HttpUnauthorizedResult();

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

                //ticket.SendApprovedNotification();                // Added the Approved notification to let Ticket Admins know it is ready to be assigned.
                var command = new TicketNotificationMessage
                {
                    TicketId = ticket.TicketID,
                    Notification = TicketNotificationMessage.NotificationType.Approved,
                    Sender = HttpContext.User.Identity.Name,
                    NoteId = ticket.TicketNotes.OrderByDescending(x => x.CreatedDate).First().NoteID
                };
                var success = MSMQ.SendQueueMessage(command, MessageType.TicketNotification.Value);

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
            var ticket = _db.TicketProjects.Single(c => c.TicketID == ticketId);
            var user = HttpContext.User.Identity.Name;
            if (user.ToLower() != ticket.RequestedBy.ToLower())
            {
                if (!Services.Authorizer.Authorize(Permissions.SupportAdmin, T("You Do Not Have Permission to Complete Tickets")) &&
                !Services.Authorizer.Authorize(Permissions.SupportApprover, T("You Do Not Have Permission to Complete Tickets")) &&
                !Services.Authorizer.Authorize(Permissions.SupportIT, T("You Do Not Have Permission to Complete Tickets")))
                    return new HttpUnauthorizedResult();
            }
            string msg = "";
            try
            {
                var oldstatus = ticket.TicketStatus.Name;
                var origTicket = ticket;

                ticket.TicketStatus = completed ? _db.TicketStatuses.First(c => c.StatusID == 5) : _db.TicketStatuses.First(c => c.StatusID == 3);
                if (completed)
                {
                    ticket.Status = 5;
                    ticket.CompletionDate = DateTime.Now;

                    var restoftickets = _db.TicketProjects.Where(x => x.TicketSequence > ticket.TicketSequence && x.TicketSequence != null).ToList();

                    foreach (var item in restoftickets)
                    {
                        item.TicketSequence = item.TicketSequence - 1;
                    }

                    ticket.TicketSequence = null;
                }
                else
                    ticket.Status = 3;
                ticket.TicketNotes.Add(new TicketNote() { Note = String.Format("Status was changed: {0} -> {1}", oldstatus, ticket.TicketStatus.Name), CreatedBy = User.Identity.Name, CreatedDate = DateTime.Now, Visibility = 1 });
                ticket.TicketStatusHistories.Add(new TicketStatusHistory() { TicketStatus = ticket.TicketStatus, CreateDate = DateTime.Now });
                if (ticket.ResourceEmployeeID == null && ticket.AssignedEmployeeID != null)
                {
                    ticket.ResourceEmployeeID = ticket.AssignedEmployeeID;
                    ticket.TicketEmployee1 = ticket.TicketEmployee;
                }
                _db.SaveChanges();

                var command = new TicketNotificationMessage             // Added the completion notification to let the assigned person know it is complete.
                {
                    TicketId = ticket.TicketID,
                    Notification = TicketNotificationMessage.NotificationType.UpdateAssigned,
                    Sender = HttpContext.User.Identity.Name,
                };
                var success = MSMQ.SendQueueMessage(command, MessageType.TicketNotification.Value);
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
            var ticket = _db.TicketProjects.Single(c => c.TicketID == ticketId);
            var user = HttpContext.User.Identity.Name;
            if (user.ToLower() != ticket.RequestedBy.ToLower())
            {
                if (!Services.Authorizer.Authorize(Permissions.SupportAdmin, T("You Do Not Have Permission to Cancel Tickets")) &&
                //!Services.Authorizer.Authorize(Permissions.SupportApprover, T("You Do Not Have Permission to Cancel Tickets")) &&
                !Services.Authorizer.Authorize(Permissions.SupportIT, T("You Do Not Have Permission to Cancel Tickets")))
                { return new HttpUnauthorizedResult(); }
            }

            string msg = "";
            var result = "";
            string statusMessage = "";

            try
            {
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
                    ticket.TicketStatus = _db.TicketStatuses.Where(c => c.StatusID == 3).First(); // Status = Cancellation Rejected
                    statusMessage += "The ticket cancellation has been rejected.  Please look at the ticket, and continue working on it, or get with the requestor, and discuss how it should be handled.";
                    statusMessage += "<br />If you feel this is an error, please contact your Supervisor, or IT to discuss the issue";
                }

                ticket.TicketNotes.Add(new TicketNote() { Note = String.Format("Status was changed: {0} -> {1}", oldstatus, ticket.TicketStatus.Name), CreatedBy = User.Identity.Name, CreatedDate = DateTime.Now, Visibility = 1 });
                ticket.TicketStatusHistories.Add(new TicketStatusHistory { TicketStatus = ticket.TicketStatus, CreateDate = DateTime.Now });

                _db.SaveChanges();

                // send notifications to the submitter
                //ticket.SendUpdateNotification(statusMessage);
                var command = new TicketNotificationMessage
                {
                    TicketId = ticket.TicketID,
                    Notification = TicketNotificationMessage.NotificationType.Update,
                    Sender = HttpContext.User.Identity.Name,
                    NoteId = ticket.TicketNotes.OrderByDescending(x => x.CreatedDate).First().NoteID
                };
                var success = MSMQ.SendQueueMessage(command, MessageType.TicketNotification.Value);

                // send notification to the person assigned to the ticket
                //if (ticket.TicketEmployee != null)
                //{
                //    if (ticket.TicketEmployee.NTLogin != ticket.RequestedBy)
                //        ticket.SendUpdateNotificationToAssigned(statusMessage);                // Added the completion notification to let the assigned person know it is complete.
                //}

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

        public ActionResult ReOpen(int ticketId)
        {
            var ticket = _db.TicketProjects.Single(c => c.TicketID == ticketId);
            var user = HttpContext.User.Identity.Name;
            if (user.ToLower() != ticket.RequestedBy.ToLower())
            {
                if (!Services.Authorizer.Authorize(Permissions.SupportAdmin, T("You Do Not Have Permission to Re-Open this Ticket")) &&
                !Services.Authorizer.Authorize(Permissions.SupportApprover, T("You Do Not Have Permission to Re-Open this Ticket")) &&
                !Services.Authorizer.Authorize(Permissions.SupportIT, T("You Do Not Have Permission to Re-Open this Ticket")))
                    return new HttpUnauthorizedResult();
            }
            string msg = "";
            try
            {
                ticket.TicketStatus = _db.TicketStatuses.First(c => c.StatusID == 3);
                ticket.Status = 3;

                ticket.TicketNotes.Add(new TicketNote() { Note = String.Format("Ticket Re-Opened by {0}", HttpContext.User.Identity.Name), CreatedBy = User.Identity.Name, CreatedDate = DateTime.Now, Visibility = 1 });
                ticket.TicketStatusHistories.Add(new TicketStatusHistory() { TicketStatus = ticket.TicketStatus, CreateDate = DateTime.Now });
                if (ticket.ResourceEmployeeID == null && ticket.AssignedEmployeeID != null)
                {
                    ticket.ResourceEmployeeID = ticket.AssignedEmployeeID;
                    ticket.TicketEmployee1 = ticket.TicketEmployee;
                }

                _db.SaveChanges();

                var command = new TicketNotificationMessage         // Added the approved notification to let the .
                {
                    TicketId = ticket.TicketID,
                    Notification = TicketNotificationMessage.NotificationType.Assignment,
                    Sender = HttpContext.User.Identity.Name,
                };
                var success = MSMQ.SendQueueMessage(command, MessageType.TicketNotification.Value);
            }
            catch (Exception err)
            {
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
                    var command = new TicketNotificationMessage
                    {
                        TicketId = ticket.TicketID,
                        Notification = TicketNotificationMessage.NotificationType.TaskAssignment,
                        Sender = HttpContext.User.Identity.Name,
                        TaskId = ticketTask.ID
                    };
                    var success = MSMQ.SendQueueMessage(command, MessageType.TicketNotification.Value);
                    //ticket.SendTaskAssignmentNotification(ticketTask);
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
                if (ticketTask.Completed && ticketTask.CompletionDate == null) ticketTask.CompletionDate = DateTime.Now;
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


        public ActionResult PrintScreen(int id)
        {
            var qry = _db.TicketProjects.FirstOrDefault(c => c.TicketID == id);
            if (qry == null) return RedirectToAction("Index");
            var user = HttpContext.User.Identity.Name;

            var vm = new TicketViewModel() { Ticket = qry, Tasks = qry.TicketTasks, TicketId = id };
            if (Services.Authorizer.Authorize(Permissions.SupportAdmin))
            {
                vm.Admin = true;
            }
            if (Services.Authorizer.Authorize(Permissions.SupportIT))
            {
                vm.IT = true;
            }

            if (Services.Authorizer.Authorize(Permissions.SupportApprover) || ADHelper.GetGroupNames(user).Contains("SupportTicketApprover"))
            {
                vm.Approver = true;
            }

            if (qry.TicketEmployee != null && user.ToUpper() == qry.TicketEmployee.NTLogin.ToUpper()) vm.AssignedToMe = true;

            GenerateDropDowns(vm, qry);

            if (!String.IsNullOrEmpty((string)TempData["ErrorMessage"])) ViewBag.ErrorMessage = TempData["ErrorMessage"];
            if (!String.IsNullOrEmpty((string)TempData["Notice"])) ViewBag.Notice = TempData["Notice"];

            ViewBag.Title = "Ticket Info";
            return View(vm);
        }
    }
}