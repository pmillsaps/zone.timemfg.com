using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System.Web.Mvc;

namespace Time.Epicor.Controllers
{
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

        public ActionResult Index()
        {
            return View();
        }
    }
}