using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.Versalift;

namespace Time.Epicor.Controllers
{
    [Authorize]
    [Themed]
    public class DLSController : Controller
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        private VersaliftEntities db;

        public DLSController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
            db = new VersaliftEntities();
        }

        // GET: DLS
        public ActionResult Index()
        {
            var dls = db.E10_DLS.OrderBy(x => x.CustNum).ThenBy(x => x.Model).ThenBy(x => x.Sales);
            return View(dls);
        }
    }
}