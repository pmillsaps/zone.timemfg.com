using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.ITInventory;
using Time.IT.ViewModel;

namespace Time.IT.Controllers
{
    [Themed]
    [Authorize]
    public class ServersController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: Servers
        public ActionResult Index()
        {
            List<ServerStatusViewModel> serverstatus = new List<ServerStatusViewModel>();
            var servers = db.Computers.Where(x => x.Ref_DeviceType.DeviceType.ToLower().Contains("server")).OrderBy(x => x.Name);
            foreach (var server in servers)
            {
                serverstatus.Add(new ServerStatusViewModel
                {
                    Name = server.Name,
                    //IPAddress = nic.IP,
                    //PingResult = PingTest(server.Name)
                });
            }

            return View(serverstatus);
        }

        public bool PingTest(string IP)
        {
            Ping ping = new Ping();
            string pingAddress = IP;

            PingReply pingreply = ping.Send(pingAddress);

            if (pingreply.Status == IPStatus.Success)
                return true;
            else
                return false;
        }

        public ActionResult _PingResult(string servername)
        {
            string result = "";
            try
            {
                Ping ping = new Ping();
                string pingAddress = servername;
                PingReply pingreply = ping.Send(pingAddress);

                if (pingreply.Status == IPStatus.Success)
                    result = "Alive";
                else
                    result = "No Reply";
            }
            catch (Exception ex)
            {
                result = "Error";
                //throw;
            }

            return PartialView("_PingResult", result);
        }
    }
}