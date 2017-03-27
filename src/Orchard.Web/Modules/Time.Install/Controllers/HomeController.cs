using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Time.Install.Controllers
{
    [Themed]
    [Authorize]
    public class HomeController : Controller
    {
        private string ErrorMessage { get; set; }
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public HomeController(IOrchardServices services)
        {
            Services = services;
        }

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MoreInfo(string documentSection)
        {
            switch (documentSection)
            {
                case "vswTimeliftFamily":
                    return PartialView("_vswTimeliftFamily");
                case "vswOptionGroup":
                    return PartialView("_vswOptionGroup");
                case "vswOptions":
                    return PartialView("_vswOptions");
                case "vswHourPaintRates":
                    return PartialView("_vswHourPaintRates");
                case "wordOptionsAndDescr":
                    return PartialView("_wordOptionsAndDescr");
                case "wordInstallDetails":
                    return PartialView("_wordInstallDetails");
                case "quoteDeptUsers":
                    return PartialView("_quoteDeptUsers");
                default:
                    return PartialView();
            }
            
        }
    }
}