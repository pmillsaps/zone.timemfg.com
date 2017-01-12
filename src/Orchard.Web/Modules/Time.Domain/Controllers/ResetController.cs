using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Time.Domain.Controllers
{
    [Themed]
    public class ResetController : Controller
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ResetController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
        }
        
        // GET: Reset
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string User)
        {
            if (User != null)
            {
                return View("ResetConfirmed");
            }

            return View(User);
        }
    }
}