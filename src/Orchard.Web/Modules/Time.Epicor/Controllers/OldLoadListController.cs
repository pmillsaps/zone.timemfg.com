using System.Web.Mvc;
using Orchard.Localization;
using Orchard;
using Orchard.Themes;

namespace Time.Epicor.Controllers {
    [Themed]
    public class OldLoadListController : Controller {
        public IOrchardServices Services { get; set; }

        public OldLoadListController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AsmInspect()
        {
            return View();
        }
    }
}
