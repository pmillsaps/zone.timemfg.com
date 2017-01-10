using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.Production;
using Time.Epicor.Helpers;

namespace Time.Epicor.Controllers
{
    [Authorize]
    [Themed]
    public class YardReportController : Controller
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        private ProductionEntities db;

        public YardReportController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
            db = new ProductionEntities();
        }

        // GET: YardReport
        public ActionResult Index(string action = "")
        {
            var data = db.V_YardReport.ToList();
            if (!String.IsNullOrEmpty(action) && action == "export")
                return new ExporttoExcelResult("YardReport", data.Cast<object>().ToList());
            return View(data);
        }
    }
}