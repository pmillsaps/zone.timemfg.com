using System.Web.Mvc;
using Orchard.Localization;
using Orchard;
using Orchard.Themes;

namespace Time.Invoicing.Controllers {
    [Authorize]
    [Themed]
    public class AdminsController : Controller {
        public IOrchardServices Services { get; set; }
                
        public Localizer T { get; set; }

        public AdminsController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}
