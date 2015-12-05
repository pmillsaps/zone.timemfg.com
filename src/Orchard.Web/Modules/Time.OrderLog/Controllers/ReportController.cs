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
            model.ReportName = "ExecutiveSummary";
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

            if (model.ReportName == "DailyOrders")
            {
                rptH.SetParameterValue("Special", model.Special);
                rptH.SetParameterValue("Stock", model.Stock);
                rptH.SetParameterValue("Demo", model.Demo);
                rptH.SetParameterValue("RTG", model.RTG);
                rptH.SetParameterValue("TruGuard", model.TruGuard);
            }

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
            model.ReportName = "DealerSummary";
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
            model.ReportName = "ModelSummary";
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
            model.ReportName = "DailyOrders";
            model = SetDates(model);
            if (CheckMultipleFlags(model)) ModelState.AddModelError("", "Only One Lift Type Flag can be selected at a time");
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

        // Checking that no multiple flags are selected
        private bool CheckMultipleFlags(ReportViewModel model)
        {
            if (model.Demo) return (model.RTG || model.Stock || model.Special || model.TruGuard);
            if (model.RTG) return (model.Demo || model.Stock || model.Special || model.TruGuard);
            if (model.Stock) return (model.Demo || model.RTG || model.Special || model.TruGuard);
            if (model.Special) return (model.Demo || model.RTG || model.Stock || model.TruGuard);
            if (model.TruGuard) return (model.Demo || model.RTG || model.Stock || model.Special);

            return false;
        }

        private ReportViewModel SetDates(ReportViewModel model)
        {
            int year = DateTime.Now.Year;
            if (model.StartDate == null) model.StartDate = new DateTime(year, 1, 1);
            if (model.EndDate == null) model.EndDate = new DateTime(year, 12, 31);
            model.EndDate = model.EndDate.Value.Date.AddDays(1).AddSeconds(-1);
            return model;
        }

        private bool CheckReportData(ReportViewModel model)
        {
            var qry = db.OrderTrans.AsQueryable();

            if (model.StartDate != null) qry = qry.Where(x => x.AsOfDate >= model.StartDate);
            if (model.EndDate != null)
            {
                model.EndDate = model.EndDate.Value.Date.AddDays(1).AddSeconds(-1);
                qry = qry.Where(x => x.AsOfDate < model.EndDate);
            }
            if (model.DealerId != null) qry = qry.Where(x => x.Order.DealerId == model.DealerId);
            if (model.RegionId != null) qry = qry.Where(x => x.Order.Territory.RegionId == model.RegionId);
            if (model.TerritoryId != null) qry = qry.Where(x => x.Order.TerritoryId == model.TerritoryId);

            if (model.ReportName == "DailyOrders")
            {
                if (model.Demo) qry = qry.Where(x => x.Demo);
                if (model.RTG) qry = qry.Where(x => x.RTG);
                if (model.Special) qry = qry.Where(x => x.Special);
                if (model.Stock) qry = qry.Where(x => x.Stock);
            }

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
                //ServerName = "Aruba-Sql",
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