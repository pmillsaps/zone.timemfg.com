using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Time.Epicor.Controllers
{
    [Themed]
    public class QuoteOrderSearchController : Controller
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public QuoteOrderSearchController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
        }

        // GET: QuoteOrderSearch
        public ActionResult Index()
        {
            return View();
        }
    }
}