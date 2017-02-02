using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.Epicor;
using Time.Epicor.ViewModels;

namespace Time.Epicor.Controllers
{
    [Authorize]
    [Themed]
    public class V8WhereUsedBomController : Controller
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        private EpicorEntities db;

        public V8WhereUsedBomController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
            db = new EpicorEntities();
        }

        // GET: V8WhereUsedBom
        public ActionResult Index(V8WhereUsedBomVM vm, string search = "")
        {
            if (vm == null)
                vm = new V8WhereUsedBomVM();
            if (!string.IsNullOrEmpty(search)) vm.Search = search;
            return View(vm);
        }

        public ActionResult _BOM(V8WhereUsedBomVM vm)
        {
            var bom = new List<v_BillOfMaterials>();
            if (!string.IsNullOrEmpty(vm.Search)) bom = db.v_BillOfMaterials.Where(x => x.partnum == vm.Search).OrderBy(x => x.mtlpartnum).ToList();
            return PartialView(bom);
        }

        public ActionResult _WhereUsed(V8WhereUsedBomVM vm)
        {
            var wu = new List<v_WhereUsed>();
            if (!String.IsNullOrEmpty(vm.Search)) wu = db.v_WhereUsed.Where(x => x.mtlpartnum == vm.Search && x.revisionnum == x.Latest_Rev).OrderBy(x => x.mtlpartnum).ToList();
            return PartialView(wu);
        }
    }
}