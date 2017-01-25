using System.Web.Mvc;
using Orchard.Localization;
using Orchard;
using Orchard.Themes;

namespace Time.CustomManuals.Controllers {
    [Themed]
    public class HomeController : Controller {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public HomeController(IOrchardServices services) {
            Services = services;
            T = NullLocalizer.Instance;
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}
