using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Time.Legacy.EntityModels.Legacy;

namespace Time.Legacy.Controllers
{
    //sets theme and requires you to log in to go to the page
    [Themed]
    [Authorize]
    public class WarrantyInformationsController : Controller
    {
        private LegacyEntities db = new LegacyEntities();

        // GET: WarrantyInformations
        public ActionResult Index(string search = "")
        {
            var warranties = db.WarrantyInformations.Where(x => x.SerialNumber != null);
            var invoices = db.WarrantyInvoices.Where(x => x.SerialNumber != null);

            if (!String.IsNullOrEmpty(search))
            {
                foreach (var item in search.Split(' '))
                {
                    warranties = warranties.Where(x => x.SerialNumber.Contains(item) || x.EndUserName.Contains(item) || x.Phone.Contains(item) || x.Address.Contains(item) || x.Comments.Contains(item));
                }

                var final = warranties.OrderBy(x => x.SerialNumber).ToList();

                return View(final);
            }
            else
            {
                return View();
            }
        }

        // GET: WarrantyInformations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WarrantyInformation warrantyInformation = db.WarrantyInformations.Find(id);
            if (warrantyInformation == null)
            {
                return HttpNotFound();
            }
            return View(warrantyInformation);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
