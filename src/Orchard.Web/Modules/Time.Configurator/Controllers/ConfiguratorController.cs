using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Time.Data.Models.MessageQueue;

namespace Time.Configurator.Controllers
{
    [Themed]
    public class ConfiguratorController : Controller
    {
        private string ErrorMessage { get; set; }

        // GET: Configurator
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BuildComplexlookups()
        {
            var command = new EmptyMessage();
            var success = MSMQ.SendQueueMessage(command, MessageType.BuildComplexLookups);
            if (!string.IsNullOrEmpty(ErrorMessage)) ViewBag.ErrorMessage = ErrorMessage;

            ViewBag.Title = "Build Complex Lookup";
            if (success)
            {
                ViewBag.Message = "Build Complex Lookup Message Queued.  It should be sent within the next few moments.";
            }
            else
            {
                ViewBag.Message = "Error queuing/sending the Build Complex Lookup Message. Please try later, or get with IT support to troubleshoot the issue.";
            }

            return View("MessageResult");
        }

        public ActionResult BuildConfigOptions()
        {
            var command = new EmptyMessage();
            var success = MSMQ.SendQueueMessage(command, MessageType.BuildConfigOptions);
            if (!string.IsNullOrEmpty(ErrorMessage)) ViewBag.ErrorMessage = ErrorMessage;

            ViewBag.Title = "Build Config Options";
            if (success)
            {
                ViewBag.Message = "Build Config Options Message Queued.  It should be sent within the next few moments.";
            }
            else
            {
                ViewBag.Message = "Error queuing/sending the Build Config Options Message. Please try later, or get with IT support to troubleshoot the issue.";
            }

            return View("MessageResult");
        }
    }
}