using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Time.Data.EntityModels.OrderLog;
using Time.OrderLog.Models;

namespace Time.OrderLog.Controllers
{
    [Themed]
    public class ReportController : Controller
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        private OrderLogEntities db;
        private const string _db_logon = "TimeMFGApp";
        private const string _db_password = "Tm@Time$!";

        public ReportController(IOrchardServices services)
        {
            Services = services;
            T = NullLocalizer.Instance;
            db = new OrderLogEntities();
            Setup();
        }

        public ReportController(IOrchardServices services, OrderLogEntities _db)
        {
            Services = services;
            T = NullLocalizer.Instance;
            db = _db;
            Setup();
        }

        public void Setup()
        {
            var vm = new MenuViewModel();
            if (Services.Authorizer.Authorize(Permissions.ViewOrders)) vm.ViewOrders = true;
            if (Services.Authorizer.Authorize(Permissions.EditOrders)) vm.EditOrders = true;
            if (Services.Authorizer.Authorize(Permissions.OrderLogReporting)) vm.OrderLogReporting = true;
            ViewBag.Permissions = vm;
        }

        // GET: /OrderLog/
        public ActionResult Index()
        {
            if (!Services.Authorizer.Authorize(Permissions.OrderLogReporting, T("You Do Not Have Permission to View Reports")))
                return new HttpUnauthorizedResult();
            return View();
        }

        [HttpGet]
        public ActionResult ExecutiveSummary()
        {
            if (!Services.Authorizer.Authorize(Permissions.OrderLogReporting, T("You Do Not Have Permission to View Reports")))
                return new HttpUnauthorizedResult();
            getDropDowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "OrderId,PO,DealerId,TerritoryID,Date")] Order order)
        public ActionResult ExecutiveSummary(ReportViewModel model)
        {
            if (!Services.Authorizer.Authorize(Permissions.OrderLogReporting, T("You Do Not Have Permission to View Reports")))
                return new HttpUnauthorizedResult();
            model = SetDates(model);
            if (!CheckReportData(model)) ModelState.AddModelError("", "This Report has no Data... Please check your parameters");
            if (ModelState.IsValid)
            {
                string reportPath = Server.MapPath("~/Modules/Time.OrderLog/Content/Reports/ExecutiveSummary.rpt");
                Stream stream = GetReport(model, reportPath);
                return File(stream, "application/pdf");
            }
            getDropDowns();

            return View(model);
        }

        private Stream GetReport(ReportViewModel model, string reportPath)
        {
            //return new CrystalReportPdfResult(reportPath);
            ReportClass rptH = new ReportClass();
            rptH.FileName = reportPath;
            rptH.Load();
            rptH.SetDatabaseLogon(_db_logon, _db_password);
            SetDBLogonForReport(rptH);

            rptH.SetParameterValue("StartDate", model.StartDate ?? DateTime.Now.AddYears(-99));
            rptH.SetParameterValue("EndDate", model.EndDate ?? DateTime.Now.AddYears(99));
            rptH.SetParameterValue("DealerId", model.DealerId ?? 0);
            rptH.SetParameterValue("TerritoryId", model.TerritoryId ?? 0);
            rptH.SetParameterValue("RegionId", model.RegionId ?? 0);

            Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
            return stream;
        }


        [HttpGet]
        public ActionResult DealerSummary()
        {
            if (!Services.Authorizer.Authorize(Permissions.OrderLogReporting, T("You Do Not Have Permission to View Reports")))
                return new HttpUnauthorizedResult();
            getDropDowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "OrderId,PO,DealerId,TerritoryID,Date")] Order order)
        public ActionResult DealerSummary(ReportViewModel model)
        {
            if (!Services.Authorizer.Authorize(Permissions.OrderLogReporting, T("You Do Not Have Permission to View Reports")))
                return new HttpUnauthorizedResult();
            model = SetDates(model);
            if (!CheckReportData(model)) ModelState.AddModelError("", "This Report has no Data... Please check your parameters");
            if (ModelState.IsValid)
            {
                string reportPath = Server.MapPath("~/Modules/Time.OrderLog/Content/Reports/DealerSummary.rpt");
                Stream stream = GetReport(model, reportPath);
                return File(stream, "application/pdf");
            }
            getDropDowns();

            return View(model);
        }

        [HttpGet]
        public ActionResult ModelSummary()
        {
            if (!Services.Authorizer.Authorize(Permissions.OrderLogReporting, T("You Do Not Have Permission to View Reports")))
                return new HttpUnauthorizedResult();
            getDropDowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "OrderId,PO,DealerId,TerritoryID,Date")] Order order)
        public ActionResult ModelSummary(ReportViewModel model)
        {
            if (!Services.Authorizer.Authorize(Permissions.OrderLogReporting, T("You Do Not Have Permission to View Reports")))
                return new HttpUnauthorizedResult();
            model = SetDates(model);
            if (!CheckReportData(model)) ModelState.AddModelError("", "This Report has no Data... Please check your parameters");
            if (ModelState.IsValid)
            {
                string reportPath = Server.MapPath("~/Modules/Time.OrderLog/Content/Reports/ModelSummary.rpt");
                Stream stream = GetReport(model, reportPath);
                return File(stream, "application/pdf");
            }

            getDropDowns();
            return View(model);
        }

        [HttpGet]
        public ActionResult DailyOrders()
        {
            if (!Services.Authorizer.Authorize(Permissions.OrderLogReporting, T("You Do Not Have Permission to View Reports")))
                return new HttpUnauthorizedResult();
            getDropDowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "OrderId,PO,DealerId,TerritoryID,Date")] Order order)
        public ActionResult DailyOrders(ReportViewModel model)
        {
            if (!Services.Authorizer.Authorize(Permissions.OrderLogReporting, T("You Do Not Have Permission to View Reports")))
                return new HttpUnauthorizedResult();
            model = SetDates(model);
            if (!CheckReportData(model)) ModelState.AddModelError("", "This Report has no Data... Please check your parameters");
            if (ModelState.IsValid)
            {
                string reportPath = Server.MapPath("~/Modules/Time.OrderLog/Content/Reports/DailyReports.rpt");
                Stream stream = GetReport(model, reportPath);
                return File(stream, "application/pdf");
            }
            getDropDowns();

            return View(model);
        }

        private ReportViewModel SetDates(ReportViewModel model)
        {
            int year = DateTime.Now.Year;
            if (model.StartDate == null) model.StartDate = new DateTime(year, 1, 1);
            if (model.EndDate == null) model.EndDate = new DateTime(year, 12, 31);
            return model;
        }

        private bool CheckReportData(ReportViewModel model)
        {
            var qry = db.OrderTrans.AsQueryable();

            if (model.StartDate != null) qry = qry.Where(x => x.AsOfDate >= model.StartDate);
            if (model.EndDate != null) qry = qry.Where(x => x.AsOfDate <= model.EndDate);
            if (model.DealerId != null) qry = qry.Where(x => x.Order.DealerId == model.DealerId);
            if (model.RegionId != null) qry = qry.Where(x => x.Order.Territory.RegionId == model.RegionId);
            if (model.TerritoryId != null) qry = qry.Where(x => x.Order.TerritoryId == model.TerritoryId);

            return qry.Count() > 0;

        }
        private void getDropDowns()
        {
            ViewBag.DealerId = new SelectList(db.Dealers.OrderBy(x => x.DealerName), "DealerId", "DealerName");
            ViewBag.TerritoryId = new SelectList(db.Territories.OrderBy(x => x.TerritoryName), "TerritoryId", "TerritoryName");
            ViewBag.RegionId = new SelectList(db.Regions.OrderBy(x => x.RegionName), "RegionId", "RegionName");
            ViewBag.InstallId = new SelectList(db.Installs.OrderBy(x => x.InstallName), "InstallId", "InstallName");
            ViewBag.InstallerId = new SelectList(db.Installers.OrderBy(x => x.InstallerName), "InstallerId", "InstallerName");
        }

        private void SetDBLogonForReport(ReportDocument reportDocument)
        {
            ConnectionInfo info = new ConnectionInfo()
            {
                //ServerName = "Aruba-Sql1",
                //DatabaseName = "TimeMFG",
                UserID = _db_logon,
                Password = _db_password
            };
            Tables tables = reportDocument.Database.Tables;
            foreach (Table table in tables)
            {
                TableLogOnInfo tableLogonInfo = table.LogOnInfo;
                tableLogonInfo.ConnectionInfo = info;
                table.ApplyLogOnInfo(tableLogonInfo);
            }
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