using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.Production;
using Time.Epicor.ViewModels;

namespace Time.Epicor.Controllers
{
    [Authorize]
    [Themed]
    public class WhereUsedBomController : Controller
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        private ProductionEntities db;

        public WhereUsedBomController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
            db = new ProductionEntities();
        }

        // GET: WhereUsedBom
        public ActionResult Index(WhereUsedBomVM vm, string search = "")
        {
            if (vm == null)
                vm = new WhereUsedBomVM();
            if (!string.IsNullOrEmpty(search)) vm.Search = search;
            return View(vm);
        }

        public ActionResult _BOM(WhereUsedBomVM vm)
        {
            var bom = new List<V_BillOfMaterials>();
            if (!String.IsNullOrEmpty(vm.Search)) bom = db.V_BillOfMaterials.Where(x => x.PartNum == vm.Search).ToList();
            return PartialView(bom);
        }

        public ActionResult _WhereUsed(WhereUsedBomVM vm)
        {
            var wu = new List<V_WhereUsed>();
            if (!String.IsNullOrEmpty(vm.Search)) wu = db.V_WhereUsed.Where(x => x.MtlPartNum == vm.Search && x.RevisionNum == x.Latest_Rev).ToList();
            return PartialView(wu);
        }
    }
}