using Orchard.Themes;
using System.Web.Mvc;

namespace Time.HelloWorld.Controllers
{
    [Themed]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}