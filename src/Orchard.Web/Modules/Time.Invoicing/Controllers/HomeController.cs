using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System.Web.Mvc;
using Time.Data.Models.MessageQueue;

namespace Time.Invoicing.Controllers
{
    [Authorize]
    [Themed]
    public class HomeController : Controller
    {
        public IOrchardServices Services { get; set; }

        public Localizer T { get; set; }

        public HomeController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SendInvoices()
        {
            var command = new EmptyMessage();
            var success = MSMQ.SendQueueMessage(command, MessageType.ProcessPCInvoices.Value);
            //if (!string.IsNullOrEmpty(ErrorMessage)) ViewBag.ErrorMessage = ErrorMessage;

            ViewBag.Title = "Send PC Invoices";
            if (success)
            {
                ViewBag.Message = "PCInvoices Queued.  They should be sent within the next few moments.";
            }
            else
            {
                ViewBag.Message = "Error queueing/sending the PCInvoices Message. Please try later, or get with IT support to troubleshoot the issue.";
            }

            return View("MessageResult");
        }
    }
}