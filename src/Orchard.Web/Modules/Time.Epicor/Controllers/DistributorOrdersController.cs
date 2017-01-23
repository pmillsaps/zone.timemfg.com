using MoreLinq;
using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System.Linq;
using System.Web.Mvc;
using Time.Data.EntityModels.Production;
using Time.Epicor.ViewModels;

namespace Time.Epicor.Controllers
{
    [Authorize]
    [Themed]
    public class DistributorOrdersController : Controller
    {
        public IOrchardServices Services { get; set; }

        public DistributorOrdersController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ActionResult Index(DistributorOrderListVM vm)
        {
            using (var db = new ProductionEntities())
            {
                vm.Orders = db.V_DistributorOrderList.OrderBy(x => x.OrderDate);
                if (vm.Distributor != 0)
                {
                    vm.Orders = vm.Orders.Where(x => x.CustNum == vm.Distributor);
                }
                vm.Orders = vm.Orders.ToList();

                return View(vm);
            }
        }

        public ActionResult _DistributorList(int id)
        {
            using (var db = new ProductionEntities())
            {
                var dist = new SelectList(db.V_DistributorOrderList.OrderBy(x => x.Name).DistinctBy(x => x.CustNum), "CustNum", "Name", id);

                ViewBag.Distributors = dist.ToList();

                return PartialView();
            }
        }
    }
}