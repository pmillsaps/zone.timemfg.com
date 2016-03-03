using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.TimeMFG;
using Time.Data.Models;
using Time.Data.Models.MessageQueue;

namespace Time.Support.Controllers
{
    [Themed]
    public class ApproveController : Controller
    {
        //private TimeMFGEntities _db;

        //public ApproveController()
        //{
        //    _db = new TimeMFGEntities();
        //}

        //public ApproveController(TimeMFGEntities db)
        //{
        //    _db = db;
        //}

        // GET: Approve
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Ticket(string id)
        {
            using (var _db = new TimeMFGEntities())
            {
                var ticket = _db.TicketProjects.FirstOrDefault(x => x.ApprovalCode == id);
                if (ticket == null || String.IsNullOrEmpty(id)) return RedirectToAction("TicketNotFound");
                if (ticket.Status != 1)
                {
                    // Ticket is already approved
                    return RedirectToAction("ApproveTicketConfirm");
                }

                return View(ticket);
            }
        }

        [HttpPost]
        public ActionResult Ticket(TicketProject ticket)
        {
            using (var _db = new TimeMFGEntities())
            {
                var changeticket = _db.TicketProjects
                    .Include("TicketStatus")
                    .First(x => x.TicketID == ticket.TicketID);

                var department = _db.TicketDepartments.First(x => x.DepartmentID == changeticket.DepartmentID);
                var supervisor = _db.TicketEmployees.First(x => x.EmployeeID == department.SupervisorID);

                changeticket.Status = 2;
                changeticket.ApprovalDate = DateTime.Now;
                changeticket.ApprovedBy = User.Identity.Name;
                changeticket.ApprovalCode = "";
                var note = new TicketNote
                {
                    TicketID = changeticket.TicketID,
                    Note = "Ticket was approved over email.",
                    CreatedBy = supervisor.NTLogin,
                    CreatedDate = DateTime.Now,
                    Visibility = 1
                };

                var tshistory = new TicketStatusHistory
                {
                    TicketID = changeticket.TicketID,
                    CreateDate = DateTime.Now,
                    StatusID = 2
                };

                _db.TicketStatusHistories.Add(tshistory);
                _db.TicketNotes.Add(note);
                _db.SaveChanges();

                SendTicketNotification(note, changeticket);

                return RedirectToAction("ApproveTicketConfirm");
            }
        }

        private void SendTicketNotification(TicketNote note, TicketProject changeticket)
        {
            // TODO: Finish Implementation
            var email = GetSetting.String("SupportAdministrator");

            var emailBody = new StringBuilder();
            string subject = String.Format("Ticket {0} Approved: Ready for Assignment", note.TicketID);

            emailBody.AppendLine(subject);
            emailBody.AppendLine("<br />");
            emailBody.AppendLine(note.Note);
            emailBody.AppendLine("<br />");
            emailBody.AppendLine(changeticket.Title);
            emailBody.AppendLine("<br />");
            emailBody.AppendLine(String.Format("<a href='http://my.timemfg.com/Support/Info/{0}'>Ticket {0}</a>", note.TicketID));
            emailBody.AppendLine("<br />");
            var success = MSMQ.SendEmailMessage(subject, emailBody.ToString(), email, true);
        }

        public ActionResult ApproveTicketConfirm()
        {
            return View();
        }

        public ActionResult TicketNotFound()
        {
            return View();
        }
    }
}