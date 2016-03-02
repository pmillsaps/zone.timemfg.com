using System.Web.Mvc;
using Orchard.Localization;
using Orchard;
using Orchard.Themes;

namespace Time.Epicor.Controllers {
    [Themed]
    public class InventoryController : Controller {
        public IOrchardServices Services { get; set; }

        public InventoryController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ActionResult ReturnTags()
        {
            return View();
        }
    }
}
