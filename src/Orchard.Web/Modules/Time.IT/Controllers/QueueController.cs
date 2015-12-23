using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Messaging;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.TimeMFG;
using Time.Data.Models.MessageQueue;
using Time.IT.ViewModel;

namespace Time.IT.Controllers
{
    [Themed]
    [Authorize]
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

        public PartialViewResult _Processes()
        {
            var procs = new List<ProcessViewModel>();
            try
            {
                procs = CheckForExpectedProcess("Tasks", "Hknmtt14", "Aruba-Connect", "TIME", "MessageQueue");
            }
            catch (Exception ex)
            {
                ViewBag.Results = ex.Message.Replace(Environment.NewLine, @"<br />");
            }

            return PartialView(procs);
        }

        private List<ProcessViewModel> CheckForExpectedProcess(string userName, string password, string machineName, string logonDomain, string PartialProcessname)
        {
            var ReturnItems = new List<ProcessViewModel>();
            string ReturnValue = String.Empty;
            ManagementScope managementScope;
            try
            {
                ConnectionOptions connOptions = new ConnectionOptions();

                connOptions.Impersonation = ImpersonationLevel.Impersonate;
                connOptions.EnablePrivileges = true;
                connOptions.Username = string.Format("{0}\\{1}", logonDomain, userName);
                connOptions.Password = password;
                managementScope = new ManagementScope(string.Format(@"\\{0}\ROOT\CIMV2", machineName), connOptions);

                managementScope.Connect();
                ManagementObjectSearcher objSearcher = new ManagementObjectSearcher(string.Format("SELECT * FROM Win32_Process WHERE NAME LIKE'%{0}%'", PartialProcessname));
                ManagementOperationObserver opsObserver = new ManagementOperationObserver();
                objSearcher.Scope = managementScope;
                string[] sep = { "\n", "\t" };

                ManagementObjectCollection objects = objSearcher.Get();
                foreach (var item in objects)
                {
                    ReturnItems.Add(new ProcessViewModel
                    {
                        ExecutablePath = item["ExecutablePath"].ToString().Substring(9),
                        CreationDate = DateTime.ParseExact(item["CreationDate"].ToString().Substring(0, 14), "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                        ProcessId = item["ProcessId"].ToString(),
                        PageFileUsage = item["PageFileUsage"].ToString(),
                    });
                    //ReturnValue += String.Format("{1} | {2} | {3} | {4} | {5} | {6} | {0}", Environment.NewLine, item["ProcessId"], item["Caption"], item["Name"], item["ExecutablePath"], item["SessionId"], item["Status"]);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ReturnValue += ex.Message;
                // handle error - we log it to our own system, but it's up to you.
            }

            return ReturnItems;
        }
    }
}