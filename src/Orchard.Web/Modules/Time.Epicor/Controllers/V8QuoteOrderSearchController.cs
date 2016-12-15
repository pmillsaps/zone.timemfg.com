using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.Epicor;
using Time.Epicor.ViewModels;

namespace Time.Epicor.Controllers
{
    [Themed]
    public class V8QuoteOrderSearchController : Controller
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        private EpicorEntities db;

        public V8QuoteOrderSearchController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
            db = new EpicorEntities();
        }

        // GET: QuoteOrderSearch
        public ActionResult Index(V8QuoteOrderSearchVM vm)
        {
            if (vm == null) vm = new V8QuoteOrderSearchVM();
            vm.Details = new List<v_QuoteOrderInformation>();
            if (!String.IsNullOrEmpty(vm.Search)) vm.Details = db.v_QuoteOrderInformation.Where(x => x.Comment.IndexOf(vm.Search) > 0).ToList();

            return View(vm);
        }

        public ActionResult Comments(Int32 id, string type)
        {
            if (id == null || type == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            v_QuoteOrderInformation quoteorder = db.v_QuoteOrderInformation.Find(type, id);
            if (quoteorder == null)
            {
                return HttpNotFound();
            }
            return View(quoteorder);
        }
    }
}