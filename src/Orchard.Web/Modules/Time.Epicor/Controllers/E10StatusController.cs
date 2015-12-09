using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.Production;
using Time.Epicor.ViewModels;

namespace Time.Epicor.Controllers
{
    [Themed]
    public class E10StatusController : Controller
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        private ProductionEntities db;

        public E10StatusController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
            db = new ProductionEntities();
            db.Database.CommandTimeout = 600;
        }

        public E10StatusController(ProductionEntities _db, IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
            db = _db;
            db.Database.CommandTimeout = 600;
        }

        private async Task<string> GetMrpStatus()
        {
            string returnMessage = "Idle";
            var qry = db.SysTasks
                .Where(x => x.TaskStatus.ToUpper() == "ACTIVE" && x.EndedOn == null)
                .OrderByDescending(x => x.SysTaskNum);

            if (qry.Where(x => x.TaskDescription == "Process MRP").Count() > 0)
            {
                var record = qry.Where(x => x.TaskDescription == "Process MRP").First();
                //TimeSpan t = TimeSpan.FromSeconds((int)record.StartedOn);
                //string starttime = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
                //returnMessage = String.Format("Running! - MRP started: {0:d} @ {1} by {2}: Status-{3}", record.StartedOn, starttime, record.SubmitUser, record.ActivityMsg);
                returnMessage = String.Format("Running! - MRP started: {0} by {1}: Status-{2}", record.StartedOn, record.SubmitUser, record.ActivityMsg);
            }

            //var status = db.C_TMC_Status.FirstOrDefault(x => x.Name == "MRP");
            //if (status != null)
            //{
            //    returnMessage += String.Format("{0}Latest MRP Status : {1}{0}{2}", Environment.NewLine, status.Complete, status.Status);
            //    returnMessage = returnMessage.Replace(Environment.NewLine, "<br />").Replace("<br /><br />", "<br />");
            //}

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
            E10_IMViewModel vm = new E10_IMViewModel()
            {
                ImJobOper = db.IMJobOpers.ToList(),
                ImPartBin = db.IMPartBins.ToList()
            };

            return PartialView(vm);
        }

        public ActionResult _Tasks()
        {
            var qry = db.SysTasks
               .Where(x => x.TaskStatus.ToUpper() == "ACTIVE" && x.EndedOn == null)
               .OrderByDescending(x => x.SysTaskNum);

            return PartialView(qry);
        }

        public ActionResult _CompletedTasks()
        {
            var compareDate = DateTime.Now.AddDays(-1);
            var qry = db.SysTasks
               .Where(x => x.TaskStatus.ToUpper() != "ACTIVE" && x.EndedOn >= compareDate && !x.TaskDescription.ToUpper().Contains("ECC"))
               .OrderByDescending(x => x.EndedOn);

            return PartialView(qry);
        }

        public ActionResult _SchedTasks()
        {
            var qry = db.SysAgentTasks;

            var vm = new List<E10_TaskVM>();

            foreach (var item in qry)
            {
                vm.Add(new E10_TaskVM { task = item, tasksched = db.SysAgentScheds.FirstOrDefault(x => x.AgentSchedNum == item.AgentSchedNum) });
            }

            return PartialView(vm);
        }
    }
}