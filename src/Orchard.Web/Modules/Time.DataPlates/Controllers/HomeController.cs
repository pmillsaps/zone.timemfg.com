using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Time.DataPlates.Controllers
{
    [Themed]
    public class HomeController : Controller
    {
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
    }
}