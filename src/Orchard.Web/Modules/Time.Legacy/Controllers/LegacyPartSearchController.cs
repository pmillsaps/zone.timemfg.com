using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Data;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.Legacy;

namespace Time.Legacy.Controllers
{
    [Themed]
    public class LegacyPartSearchController : Controller
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        private LegacyEntities db = new LegacyEntities();

        public LegacyPartSearchController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
        }

        // GET: LegacyPartSearch
        public ActionResult Index(string search = "")
        {
            if (search.Length < 3)
            {
                return View();
            }
            else if (!String.IsNullOrEmpty(search))
            {
                var inventory = db.Inventories.Where(x => x.PartNumber.Contains(search) || x.Description.Contains(search) || x.VendorPartNumber.Contains(search) || x.AdditionalDescription.Contains(search)
                || x.AdditionalDescription2.Contains(search)).ToList();

                return View(inventory);
            }
            else
            {
                return View();
            }
        }

        public ActionResult Details(string partnumber = "")
        {
            if (partnumber == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory inventory = db.Inventories.Find(partnumber);
            if (inventory == null)
            {
                return HttpNotFound();
            }

            var steps = from p in db.PartStepDetails
                        join p2 in db.PartSteps on p.CostStepListingId equals p2.Id
                        join p3 in db.Inventories on p2.PartNumber equals p3.PartNumber
                        where p.CostStepListingId.Equals(p2.Id) && p2.PartNumber.Equals(p3.PartNumber)
                        select new { PartStepDetails = p, PartStep = p2, Inventory = p3 };

            var step = db.PartSteps.Where(x => x.PartNumber == partnumber).ToList();
            ViewBag.Step = step;

            var stepDetails = db.PartStepDetails.Where(x => x.CostStepListingId == x.PartStep.Id && x.PartStep.PartNumber == partnumber).ToList();
            ViewBag.StepDetails = stepDetails;

            var purchasedBom = db.PurchaseBOMItems.Where(x => x.ULPartNumber == partnumber).OrderBy(x => x.VendorNumber).ToList();
            ViewBag.PurchasedBOMItem = purchasedBom;

            var partxref = db.PartXRefs.Where(x => x.NewPart == partnumber || x.OldPart == partnumber).ToList();
            ViewBag.PartXRef = partxref;

            var redrill = db.RedrillPartXRefs.Where(x => x.ReedrillPartNumber == partnumber).ToList();
            ViewBag.Redrill = redrill;

            var purchaseHistory = db.PurchaseHistories.Where(x => x.PartNumber == partnumber).ToList();
            ViewBag.PurchaseHistory = purchaseHistory;

            return View(inventory);
        }





        public ActionResult _PartStepDetails(string partnumber = "")
        {
            IList<PartStepDetail> partstep = new List<PartStepDetail>();
            if (partnumber != null)
            {
                partstep = db.PartStepDetails.Where(x => x.PartStep.PartNumber == partnumber && x.CostStepListingId == x.PartStep.Id).ToList();
            }

            return PartialView(partstep);
        }
    }
}