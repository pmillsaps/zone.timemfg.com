using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System.Web.Mvc;

namespace Time.Domain.Controllers
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

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Reset()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Reset(string User)
        {
            if (User != null)
            {
                return View("ResetConfirmed");
            }

            return View(User);
        }
    }
}