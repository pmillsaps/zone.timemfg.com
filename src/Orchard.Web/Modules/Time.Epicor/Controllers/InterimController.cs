using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System.Linq;
using System.Web.Mvc;
using Time.Epicor.EntityModels.Epicor;
using Time.Epicor.ViewModels;

namespace Time.Epicor.Controllers
{
    [Themed]
    public class InterimController : Controller
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public InterimController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
        }

        // GET: Interim
        public ActionResult Index()
        {
            if (!Services.Authorizer.Authorize(Permissions.EpicorAccess, T("You do not have access to this area. Please log in")))
                return new HttpUnauthorizedResult();
            using (var db = new EpicorEntities())
            {
                db.Database.CommandTimeout = 600;
                IMViewModel vm = new IMViewModel()
                {
                    ImJobOper = db.imjobopers.ToList(),
                    ImPartBin = db.impartbins.ToList()
                };
                return View(vm);
            }
        }
    }
}