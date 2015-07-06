using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Time.Configurator.Controllers
{
    [Themed]
    public class ConfiguratorController : Controller
    {
        // GET: Configurator
        public ActionResult Index()
        {
            return View();
        }
    }
}