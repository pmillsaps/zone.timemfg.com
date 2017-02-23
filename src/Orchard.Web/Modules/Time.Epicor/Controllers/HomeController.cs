using Newtonsoft.Json;
using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.IO;
using System.Text;
using System.Web.Mvc;
using Time.Data.Models.MessageQueue;
using Time.Epicor.Helpers;

namespace Time.Epicor.Controllers
{
    [Authorize]
    [Themed]
    public class HomeController : Controller
    {
        // private string queueAddress = @"FormatName:Direct=OS:Aruba-Connect\private$\time.messagequeue";
        // private string queueAddress = "FormatName:Direct=OS:Aruba-Connect\\private$\\time.messagequeue";

        public IOrchardServices Services { get; set; }
        private string ErrorMessage { get; set; }

        public HomeController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult RTI()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RTI(FormCollection col)
        {
            var command = new EmptyMessage();
            var success = MSMQ.SendQueueMessage(command, MessageType.SendRTIMessage.Value);
            if (!string.IsNullOrEmpty(ErrorMessage)) ViewBag.ErrorMessage = ErrorMessage;

            ViewBag.Title = "Send RTI Email";
            if (success)
            {
                ViewBag.Message = "RTI Message Queued.  It should be sent within the next few moments.";
            }
            else
            {
                ViewBag.Message = "Error queueing/sending the RTI Message. Please try later, or get with IT support to troubleshoot the issue.";
            }

            return View("MessageResult");
        }

        [HttpGet]
        public ActionResult ProblemJobs()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ProblemJobs(FormCollection col)
        {
            var command = new EmptyMessage();
            var success = MSMQ.SendQueueMessage(command, MessageType.EmailProblemJobs.Value);
            if (!string.IsNullOrEmpty(ErrorMessage)) ViewBag.Message = ErrorMessage;
            ViewBag.Title = "Send Problem Job Email";
            if (success)
            {
                ViewBag.Message = "Problem Jobs Message Queued.  It should be sent within the next few moments.";
            }
            else
            {
                ViewBag.Message = "Error queueing/sending the Problem Jobs Message. Please try later, or get with IT support to troubleshoot the issue.";
            }
            return View("MessageResult");
        }

        public ActionResult ChangePrimBin()
        {
            return View();
        }
    }
}