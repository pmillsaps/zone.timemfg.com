using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Time.Epicor.Helpers;
using Time.Epicor.ViewModels;

namespace Time.Epicor.Controllers
{
    [Themed]
    public class PartSearchController : Controller
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public PartSearchController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
        }

        // GET: PartSearch
        public ActionResult Index()
        {
            PartSearchVM vm = new PartSearchVM("");
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index(PartSearchVM vm, string submitButton)
        {
            if (UserIsVSW()) vm.RestrictData = true;

            if (!String.IsNullOrEmpty(vm.Query) && (vm.Query.Length >= 3 ||
                (vm.Query.Length > 0 && vm.Type == PartSearchVM.SearchType.VendorNumber)))
                vm.refreshData();
            if (vm.ExportToExcel)
            {
                return new ExporttoExcelResult("PartSearch", vm.PartData.Cast<object>().ToList());
            }
            else
                return View(vm);
        }

        private bool UserIsVSW()
        {
            bool ret = false;
            if (User.Identity.Name.ToUpper().Contains("VERSALIFTSOUTHW")) ret = true;
            if (User.Identity.Name.ToUpper().Contains("TIMEMFG")) ret = true;
            return ret;
        }
    }
}