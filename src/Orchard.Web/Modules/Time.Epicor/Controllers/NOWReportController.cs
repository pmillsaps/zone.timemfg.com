using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.Production;

namespace Time.Epicor.Controllers
{
    [Themed]
    public class NOWReportController : Controller
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        private ProductionEntities db;

        public NOWReportController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
            db = new ProductionEntities();
        }

        // GET: NOWReport
        public ActionResult Index()
        {
            var nowReport = db.V_NowReport1.Where(x => x.AssemblySeq > 100).ToList();
            return View(nowReport);
        }
    }
}