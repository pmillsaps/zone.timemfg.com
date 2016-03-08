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
using Time.Epicor.ViewModels;

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
            NOWReportViewModel vm = new NOWReportViewModel(""); 
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index(NOWReportViewModel vm, string Search)
        {
            vm.GetData();

            // If the Export to Excel box is checked
            if (vm.ExportToExcel)
            {
                return new ExporttoExcelResult("NowReport", vm.Report.Cast<object>().ToList());
            }

            // Else return the view
            return View(vm);
        }
    }
}