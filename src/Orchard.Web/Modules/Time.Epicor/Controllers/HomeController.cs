using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System.Web.Mvc;

namespace Time.Epicor.Controllers
{
    [Themed]
    public class HomeController : Controller
    {
        public IOrchardServices Services { get; set; }

        public HomeController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ActionResult SendRTI()
        {
            return View();
        }

        public ActionResult ChangePrimBin()
        {
            return View();
        }
    }
}