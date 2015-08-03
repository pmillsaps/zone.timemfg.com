using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Time.Data.EntityModels.Epicor;
using Time.Epicor.ViewModels;

namespace Time.Epicor.Controllers
{
    [Themed]
    public class StatusController : Controller
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        private EpicorEntities db;

        public StatusController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
            db = new EpicorEntities();
            db.Database.CommandTimeout = 600;
        }

        public StatusController(EpicorEntities _db, IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
            db = _db;
            db.Database.CommandTimeout = 600;
        }

        private async Task<string> GetMrpStatus()
        {
            string returnMessage = "Idle";
            var qry = db.systasks
                .Where(x => x.taskstatus.ToUpper() == "ACTIVE" && x.enddate == null && x.endtime == 0)
                .OrderByDescending(x => x.systasknum);

            if (qry.Where(x => x.taskdescription == "Process MRP").Count() > 0)
            {
                var record = qry.Where(x => x.taskdescription == "Process MRP").First();
                TimeSpan t = TimeSpan.FromSeconds((int)record.starttime);
                string starttime = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
                returnMessage = String.Format("Running! - MRP started: {0:d} @ {1} by {2}: Status-{3}", record.startdate, starttime, record.submituser, record.activitymsg);
            }

            var status = db.C_TMC_Status.FirstOrDefault(x => x.Name == "MRP");
            if (status != null)
            {
                returnMessage += String.Format("{0}Latest MRP Status : {1}{0}{2}", Environment.NewLine, status.Complete, status.Status);
                returnMessage = returnMessage.Replace(Environment.NewLine, "<br />").Replace("<br /><br />", "<br />");
            }

            return returnMessage;
        }

        [HttpGet]
        public async Task<ActionResult> Index2()
        {
            if (!Services.Authorizer.Authorize(Permissions.EpicorAccess, T("You do not have access to this area. Please log in")))
                return new HttpUnauthorizedResult();

            return this.View();
        }

        // GET: Interim
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            if (!Services.Authorizer.Authorize(Permissions.EpicorAccess, T("You do not have access to this area. Please log in")))
                return new HttpUnauthorizedResult();

            //var vm = new EpicorStatusViewModel();

            //var tasks = db.sysagenttasks
            //    .Join(db.sysagentscheds,
            //        c => c.agentschednum,
            //        t => t.agentschednum,
            //        (c, t) => new myTask
            //        {
            //            sysagenttask = c,
            //            tasksched = t
            //        }
            //    );
            //vm.ScheduledTasks = tasks;
            ViewBag.MRPStatus = await GetMrpStatus();

            return View();
        }

        public ActionResult _Interim()
        {
            IMViewModel vm = new IMViewModel()
            {
                ImJobOper = db.imjobopers.ToList(),
                ImPartBin = db.impartbins.ToList()
            };

            return PartialView(vm);
        }

        public ActionResult _Tasks()
        {
            var qry = db.systasks
               .Where(x => x.taskstatus.ToUpper() == "ACTIVE" && x.enddate == null && x.endtime == 0)
               .OrderByDescending(x => x.systasknum);

            return PartialView(qry);
        }

        public ActionResult _SchedTasks()
        {
            var qry = db.sysagenttasks;

            var vm = new List<TaskVM>();
            Parallel.ForEach(qry, item =>
            {
                vm.Add(new TaskVM { task = item, tasksched = db.sysagentscheds.FirstOrDefault(x => x.agentschednum == item.agentschednum) });
            });

            return PartialView(vm);
        }
    }
}