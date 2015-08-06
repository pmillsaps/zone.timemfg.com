using System.Web.Mvc;
using Orchard.Localization;
using Orchard;
using Orchard.Themes;

namespace Time.Admin.Controllers {
    [Authorize]
    [Themed]
    public class HomeController : Controller {
        public IOrchardServices Services { get; set; }

        public HomeController(IOrchardServices services) {
            Services = services;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Equipment()
        {
            return View();
        }
    }
}
