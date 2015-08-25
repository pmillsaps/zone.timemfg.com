using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Time.Data.Models.MessageQueue;

namespace Time.CustomManuals.Controllers
{
    [Themed]
    public class ProcessController : Controller
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        private string InformationMessage { get; set; }
        private string ErrorMessage { get; set; }

        public ProcessController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
        }

        // GET: Process
        public ActionResult Index()
        {
            if (!String.IsNullOrEmpty(InformationMessage))
            {
                ViewBag.InformationMessage = InformationMessage;
                InformationMessage = String.Empty;
            }
            if (!String.IsNullOrEmpty(ErrorMessage))
            {
                ViewBag.ErrorMessage = ErrorMessage;
                ErrorMessage = String.Empty;
            }

            return View();
        }

        [HttpPost]
        public ActionResult CheckManuals()
        {
            var command = new EmptyMessage();
            var success = MSMQ.SendQueueMessage(command, MessageType.CustomManualCheckProblemJobs.Value);

            ViewBag.Title = "Send Custom Manual Status Email";
            if (success)
            {
                ViewBag.Message = "Custom Manual Message Queued.  It should be sent within the next few moments.";
            }
            else
            {
                ViewBag.Message = "Error queueing/sending the Custom Manual Message. Please try later, or get with IT support to troubleshoot the issue.";
            }

            return View("MessageResult");
        }

        [HttpPost]
        public ActionResult QueueJobsForManuals()
        {
            var command = new EmptyMessage();
            var success = MSMQ.SendQueueMessage(command, MessageType.CustomManualJobQueue.Value);

            ViewBag.Title = "Queue New Jobs for Custom Manuals";
            if (success)
            {
                ViewBag.Message = "Custom Manual Updates Queued.  It should be sent within the next few moments.";
            }
            else
            {
                ViewBag.Message = "Error queueing/sending the Custom Manual Update Message. Please try later, or get with IT support to troubleshoot the issue.";
            }

            return View("MessageResult");
        }
    }
}