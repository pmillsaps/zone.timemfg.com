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
    public class WhereUsedBomController : Controller
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public WhereUsedBomController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
        }

        // GET: WhereUsedBom
        public ActionResult Index()
        {
            return View();
        }
    }
}