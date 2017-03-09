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
using Time.Data.EntityModels.Legacy;
using Time.Legacy.Models;

namespace Time.Legacy.Controllers
{
    //sets theme and requires you to log in to go to the page
    [Themed]
    [Authorize]
    public class WarrantyController : Controller
    {
        private LegacyEntities db = new LegacyEntities();

        // GET: Warranty
        public ActionResult Index(string search = "", string ddl = "")
        {
            var combined = from o in db.WarrantyInformations
                           join o2 in db.WarrantyInvoices on o.SerialNumber equals o2.SerialNumber
                           where o.SerialNumber.Equals(o2.SerialNumber)
                           select new { Information = o, Invoice = o2 };
            //select new InsertWarranty { Information = o, Invoice = o2 };
            if (search.Length == 0)
            {
                return View();
            }
            else if (search.Length > 0 && ddl == "BeginsWith")
            {
                combined = combined.Where(x => x.Information.SerialNumber.StartsWith(search));

                if (combined != null)
                {
                    List<InsertWarranty> final2 = new List<InsertWarranty>();
                    var final = combined.Select(x => new
                    {
                        x.Information.Id,
                        x.Information.SerialNumber,
                        x.Information.EndUserName,
                        x.Information.Phone,
                        x.Information.Address,
                        x.Invoice.LiftOrderNumber,
                        x.Invoice.InvoiceNumber,
                        x.Invoice.PoNumber,
                        x.Information.Comments
                    }).Distinct().OrderBy(x => x.SerialNumber).ToList();

                    string temp = "";
                    foreach (var item in final)
                    {
                        InsertWarranty iw = new InsertWarranty();
                        var lift = "";
                        if (item.LiftOrderNumber != "" && item.LiftOrderNumber != null)
                            lift = db.InvoiceHeaders.Where(x => x.LiftOrderNumber == item.LiftOrderNumber && x.LiftModel != null && x.LiftModel != "").Select(x => x.LiftModel).FirstOrDefault();
                        else
                            lift = "";

                        iw.Id = item.Id;
                        iw.SerialNumber = item.SerialNumber;
                        iw.LiftModel = lift;
                        iw.EndUserName = item.EndUserName;
                        iw.Phone = item.Phone;
                        iw.Address = item.Address;
                        iw.LiftOrderNumber = item.LiftOrderNumber;
                        iw.InvoiceNumber = item.InvoiceNumber;
                        iw.PoNumber = item.PoNumber;
                        iw.Comments = item.Comments;

                        if (final2.Count == 0)
                        {
                            temp = iw.SerialNumber;
                            final2.Add(iw);
                        }
                        if (item.SerialNumber != temp)
                        {
                            temp = iw.SerialNumber;
                            final2.Add(iw);
                        }
                    }
                    return View(final2);
                }
                else
                {
                    return View();
                }
            }
            else if (!String.IsNullOrEmpty(search) && search.Length >= 3 && ddl != "BeginsWith")
            {
                foreach (var item in search.Split(' '))
                {
                    if (ddl == "All")
                    {
                        combined = combined.Where(x => x.Information.SerialNumber.Contains(item) || x.Information.EndUserName.Contains(item)
                                   || x.Invoice.LiftOrderNumber.Contains(item) || x.Invoice.InvoiceNumber.Contains(item) || x.Invoice.CustomerId.Contains(item)
                                   || x.Invoice.PoNumber.Contains(item));
                    }
                    else if (ddl == "Serial")
                    {
                        combined = combined.Where(x => x.Information.SerialNumber.Contains(item));
                    }
                    else if (ddl == "EndUserName")
                    {
                        combined = combined.Where(x => x.Information.EndUserName.Contains(item));
                    }
                    else if (ddl == "LiftOrder")
                    {
                        combined = combined.Where(x => x.Invoice.LiftOrderNumber.Contains(item));
                    }
                    else if (ddl == "Invoice")
                    {
                        combined = combined.Where(x => x.Invoice.InvoiceNumber.Contains(item));
                    }
                    else if (ddl == "CustomerID")
                    {
                        combined = combined.Where(x => x.Invoice.CustomerId.Contains(item));
                    }
                    else if (ddl == "PO")
                    {
                        combined = combined.Where(x => x.Invoice.PoNumber.Contains(item));
                    }
                }
                if (combined != null)
                {
                    List<InsertWarranty> final2 = new List<InsertWarranty>();
                    var final = combined.Select(x => new
                    {
                        x.Information.Id,
                        x.Information.SerialNumber,
                        x.Information.EndUserName,
                        x.Information.Phone,
                        x.Information.Address,
                        x.Invoice.LiftOrderNumber,
                        x.Invoice.InvoiceNumber,
                        x.Invoice.PoNumber,
                        x.Information.Comments
                    }).Distinct().OrderBy(x => x.SerialNumber).ToList();

                    string temp = "";
                    foreach (var item in final)
                    {
                        InsertWarranty iw = new InsertWarranty();

                        var lift = "";
                        if (item.LiftOrderNumber != "" && item.LiftOrderNumber != null)
                            lift = db.InvoiceHeaders.Where(x => x.LiftOrderNumber == item.LiftOrderNumber && x.LiftModel != null && x.LiftModel != "").Select(x => x.LiftModel).FirstOrDefault();
                        else
                            lift = "";

                        iw.Id = item.Id;
                        iw.SerialNumber = item.SerialNumber;
                        iw.LiftModel = lift;
                        iw.EndUserName = item.EndUserName;
                        iw.Phone = item.Phone;
                        iw.Address = item.Address;
                        iw.LiftOrderNumber = item.LiftOrderNumber;
                        iw.InvoiceNumber = item.InvoiceNumber;
                        iw.PoNumber = item.PoNumber;
                        iw.Comments = item.Comments;

                        if (final2.Count == 0)
                        {
                            temp = iw.SerialNumber;
                            final2.Add(iw);
                        }
                        if (item.SerialNumber != temp)
                        {
                            temp = iw.SerialNumber;
                            final2.Add(iw);
                        }
                    }
                    return View(final2);
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        // GET: Warranty/Details/5
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

            //pulls in data for the end user history information in the details of each serial number warranty
            var endUser = db.WarrantyEndUsers.Where(x => x.SerialNumber == warrantyInformation.SerialNumber).OrderBy(x => x.Id).ToList();
            ViewBag.EndUser = endUser;

            //pulls in data for the invoice history information in the details of each serial number warranty
            var invoiceHistory = db.WarrantyInvoices.Where(x => x.SerialNumber == warrantyInformation.SerialNumber).OrderBy(x => x.Id).ToList();
            ViewBag.InvoiceHistory = invoiceHistory;

            //pulls in data for the memos in the details of each serial number warranty
            var memos = db.SerNoMemoes.Where(x => x.SerialNumber == warrantyInformation.SerialNumber).OrderBy(x => x.Id).ToList();
            ViewBag.Memos = memos;

            return View(warrantyInformation);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////InvoiceDetails//////////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult InvoiceDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WarrantyInvoice warrantyInvoice = db.WarrantyInvoices.Find(id);
            if (warrantyInvoice == null)
            {
                return HttpNotFound();
            }

            var warrantyReturn = db.WarrantyInformations.Where(x => x.SerialNumber == warrantyInvoice.SerialNumber).Select(x => x.Id);
            ViewBag.WarrantyReturn = warrantyReturn;

            //pulls in data for the customer information in the details of each invoice
            var customerInfo = db.InvoiceHeaders.FirstOrDefault(x => x.InvoiceNumber == warrantyInvoice.InvoiceNumber && x.CompanyId == warrantyInvoice.CompanyId);
            ViewBag.CustomerInfo = customerInfo;

            //pulls in data for the ship to information in the destails of each invoice
            var shipTo = db.InvoiceShipToes.FirstOrDefault(x => x.InvoiceNumber == warrantyInvoice.InvoiceNumber && x.CompanyId == warrantyInvoice.CompanyId);
            ViewBag.ShipTo = shipTo;

            //pulls in data for the invoice lines in the details of each invoice
            var invoiceLines = db.InvoiceLineItems.Where(x => x.InvoiceNumber == warrantyInvoice.InvoiceNumber && x.CompanyId == warrantyInvoice.CompanyId);
            ViewBag.InvoiceLines = invoiceLines;

            return View(warrantyInvoice);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////////////////////////Add_Memo//////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // GET: Warranty/Add_Memo
        public ActionResult Add_Memo(int id)
        {
            WarrantyInformation warrantyInformation = db.WarrantyInformations.Find(id);

            //var serialNumber = db.SerNoMemoes.Where(x => x.SerialNumber == warrantyInformation.SerialNumber).Select(x => x.SerialNumber).Single().ToString();
            var serialNumber = db.WarrantyInformations.Where(x => x.Id == id).Select(x => x.SerialNumber).Single().ToString();
            ViewBag.SerialNumber = serialNumber;
            ViewBag.MemoDate = DateTime.Now.ToString();
            ViewBag.EnteredBy = User.Identity.Name;

            return View();
        }

        // POST: Warranty/Add_Memo
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add_Memo([Bind(Exclude = "Id")] SerNoMemo serNoMemo, WarrantyInformation warrantyInformation)
        {
            if (ModelState.IsValid)
            {
                db.SerNoMemoes.Add(serNoMemo);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = warrantyInformation.Id });
            }

            return View(serNoMemo);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////////Edit_Memo//////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // GET: Warranty/Edit_Memo
        public ActionResult Edit_Memo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SerNoMemo serNoMemo = db.SerNoMemoes.Find(id);
            if (serNoMemo == null)
            {
                return HttpNotFound();
            }
            return View(serNoMemo);
        }

        // POST: Warranty/Edit_Memo
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Memo([Bind(Include = "Id,SerialNumber,MemoDate,EnteredBy,Memo")] SerNoMemo serNoMemo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(serNoMemo).State = EntityState.Modified;
                db.SaveChanges();
                var detailsId = db.WarrantyInformations.Where(x => x.SerialNumber == serNoMemo.SerialNumber).Select(x => x.Id).Single();
                return RedirectToAction("Details", new { id = detailsId });
            }
            return View(serNoMemo);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult _WarrantyFullDetails(int id = 0)
        {
            IList<WarrantyInformation> comments = new List<WarrantyInformation>();
            if (id != 0)
            {
                comments = db.WarrantyInformations.Where(x => x.Id == id).ToList();
            }

            return PartialView(comments);
        }
    }
}