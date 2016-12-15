using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.Production;
using Time.Epicor.ViewModels;

namespace Time.Epicor.Controllers
{
    [Authorize]
    [Themed]
    public class QuoteOrderSearchController : Controller
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        private ProductionEntities db;

        public QuoteOrderSearchController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
            db = new ProductionEntities();
        }

        // GET: QuoteOrderSearch
        public ActionResult Index(QuoteOrderSearchVM vm)
        {
            if (vm == null) vm = new QuoteOrderSearchVM();
            vm.Details = new List<V_QuoteOrderInformation>();
            if (!String.IsNullOrEmpty(vm.Search)) vm.Details = db.V_QuoteOrderInformation.Where(x => x.Comment.IndexOf(vm.Search) > 0).ToList();

            return View(vm);
        }
        
        public ActionResult Comments(int id, string type)
        {
            if (id == null || type == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            V_QuoteOrderInformation quoteorder = db.V_QuoteOrderInformation.Find(id, type);
            if (quoteorder == null)
            {
                return HttpNotFound();
            }
            return View(quoteorder);
        }
    }
}