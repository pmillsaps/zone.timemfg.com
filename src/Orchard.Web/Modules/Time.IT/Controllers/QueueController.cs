using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.TimeMFG;
using Time.Data.Models.MessageQueue;

namespace Time.IT.Controllers
{
    [Themed]
    public class QueueController : Controller
    {
        // GET: Queue
        public ActionResult Index()
        {
            var msgviews = MSMQ.ListMessagesInQueue();

            return View(msgviews);
        }

        public PartialViewResult _ActiveQueue()
        {
            var active = new List<MSMQ_Status>();
            using (var db = new TimeMFGEntities())
            {
                active = db.MSMQ_Status.ToList();
            }
            return PartialView(active);
        }
    }
}