using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using MoreLinq;
using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Time.Data.EntityModels.TimeMFG;
using Time.Support.Helpers;
using Time.Support.Models;

namespace Time.Support.Controllers
{
    [Authorize]
    [Themed]
    public class ReportController : Controller
    {
        private const string _db_logon = "TimeMFGApp";
        private const string _db_password = "Tm@Time$!";
        private const string _connF7Chevy = "";
        public IOrchardServices Services { get; set; }

        public Localizer T { get; set; }
        private readonly TimeMFGEntities _db;

        public ReportController(IOrchardServices services)
        {
            Services = services;
            _db = new TimeMFGEntities();
            //Setup();
        }

        public ReportController(IOrchardServices services, TimeMFGEntities db)
        {
            Services = services;
            _db = db;
            //Setup();
        }

        public CrystalReportPdfResult CPTITickets()
        {
            string reportPath = Server.MapPath("~/Modules/Time.Support/Views/Report/CPTIReport.rpt");
            return new CrystalReportPdfResult(reportPath);
        }

        public ActionResult TicketsByCategory()
        {
            if (!Services.Authorizer.Authorize(Permissions.SupportAdmin) && !Services.Authorizer.Authorize(Permissions.SupportIT))
            {
                if (!Services.Authorizer.Authorize(Permissions.SupportIT, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
                if (!Services.Authorizer.Authorize(Permissions.SupportAdmin, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
            }
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("~/Modules/Time.Support/Views/Report/ITRequests_ByCategory.rpt");
            rptH.Load();
            rptH.SetDatabaseLogon(_db_logon, _db_password);
            SetDBLogonForReport(rptH);
            Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        public ActionResult TicketsByDepartment()
        {
            if (!Services.Authorizer.Authorize(Permissions.SupportAdmin) && !Services.Authorizer.Authorize(Permissions.SupportIT))
            {
                if (!Services.Authorizer.Authorize(Permissions.SupportIT, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
                if (!Services.Authorizer.Authorize(Permissions.SupportAdmin, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
            }
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("~/Modules/Time.Support/Views/Report/ITRequests-ByDept.rpt");
            rptH.Load();
            rptH.SetDatabaseLogon(_db_logon, _db_password);
            SetDBLogonForReport(rptH);
            Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        public ActionResult TicketsByDepartmentXLS()
        {
            if (!Services.Authorizer.Authorize(Permissions.SupportAdmin) && !Services.Authorizer.Authorize(Permissions.SupportIT))
            {
                if (!Services.Authorizer.Authorize(Permissions.SupportIT, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
                if (!Services.Authorizer.Authorize(Permissions.SupportAdmin, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
            }
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("~/Modules/Time.Support/Views/Report/ITRequests-ByDept.rpt");
            rptH.Load();
            rptH.SetDatabaseLogon(_db_logon, _db_password);
            Stream stream = rptH.ExportToStream(ExportFormatType.Excel);
            return File(stream, "application/vnd.ms-excel");
        }

        public ActionResult TicketsByResource()
        {
            if (!Services.Authorizer.Authorize(Permissions.SupportAdmin) && !Services.Authorizer.Authorize(Permissions.SupportIT))
            {
                if (!Services.Authorizer.Authorize(Permissions.SupportIT, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
                if (!Services.Authorizer.Authorize(Permissions.SupportAdmin, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
            }
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("~/Modules/Time.Support/Views/Report/ITRequests_ByResource.rpt");
            rptH.Load();
            rptH.SetDatabaseLogon(_db_logon, _db_password);
            Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        public ActionResult MyTickets()
        {
            if (!Services.Authorizer.Authorize(Permissions.SupportAdmin) && !Services.Authorizer.Authorize(Permissions.SupportIT))
            {
                if (!Services.Authorizer.Authorize(Permissions.SupportIT, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
                if (!Services.Authorizer.Authorize(Permissions.SupportAdmin, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
            }
            ReportClass rptH = new ReportClass();
            var empID = _db.TicketEmployees.FirstOrDefault(x => x.NTLogin == HttpContext.User.Identity.Name).EmployeeID;
            rptH.FileName = Server.MapPath("~/Modules/Time.Support/Views/Report/ITRequests_ByResource_Personal.rpt");
            rptH.Load();
            rptH.SetDatabaseLogon(_db_logon, _db_password);
            rptH.SetParameterValue("UserId", empID);
            Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        public ActionResult MyTicketsITZone()
        {
            if (!Services.Authorizer.Authorize(Permissions.SupportAdmin) && !Services.Authorizer.Authorize(Permissions.SupportIT))
            {
                if (!Services.Authorizer.Authorize(Permissions.SupportIT, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
                if (!Services.Authorizer.Authorize(Permissions.SupportAdmin, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
            }
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("~/Modules/Time.Support/Views/Report/ZoneRequests_ByResource_Personal.rpt");
            rptH.Load();
            rptH.SetDatabaseLogon(_db_logon, _db_password);
            rptH.SetParameterValue("UserId", HttpContext.User.Identity.Name);
            Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        public ActionResult TicketsByRequestor()
        {
            if (!Services.Authorizer.Authorize(Permissions.SupportAdmin) && !Services.Authorizer.Authorize(Permissions.SupportIT))
            {
                if (!Services.Authorizer.Authorize(Permissions.SupportIT, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
                if (!Services.Authorizer.Authorize(Permissions.SupportAdmin, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
            }
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("~/Modules/Time.Support/Views/Report/ITRequests_ByRequestor.rpt");
            rptH.Load();
            rptH.SetDatabaseLogon(_db_logon, _db_password);
            Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        public ActionResult TicketsByResourceClosed()
        {
            if (!Services.Authorizer.Authorize(Permissions.SupportAdmin) && !Services.Authorizer.Authorize(Permissions.SupportIT))
            {
                if (!Services.Authorizer.Authorize(Permissions.SupportIT, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
                if (!Services.Authorizer.Authorize(Permissions.SupportAdmin, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
            }
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("~/Modules/Time.Support/Views/Report/ITRequests_ByResource_Closed.rpt");
            rptH.Load();
            rptH.SetDatabaseLogon(_db_logon, _db_password);
            Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        public ActionResult TicketsByResourceCompleteDateClosed()
        {
            if (!Services.Authorizer.Authorize(Permissions.SupportAdmin) && !Services.Authorizer.Authorize(Permissions.SupportIT))
            {
                if (!Services.Authorizer.Authorize(Permissions.SupportIT, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
                if (!Services.Authorizer.Authorize(Permissions.SupportAdmin, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
            }
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("~/Modules/Time.Support/Views/Report/ITRequests_ByResource_CompDate_Closed.rpt");
            rptH.Load();
            rptH.SetDatabaseLogon(_db_logon, _db_password);
            Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        public ActionResult TicketsByResourceCompleteDateClosedLimited()
        {
            if (!Services.Authorizer.Authorize(Permissions.SupportAdmin) && !Services.Authorizer.Authorize(Permissions.SupportIT))
            {
                if (!Services.Authorizer.Authorize(Permissions.SupportIT, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
                if (!Services.Authorizer.Authorize(Permissions.SupportAdmin, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
            }
            DateTime startDate = DateTime.Now.AddYears(-1);
            DateTime endDate = DateTime.Now;

            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("~/Modules/Time.Support/Views/Report/ITRequests_ByResource_CompDate_Closed_Limited.rpt");
            rptH.Load();
            rptH.SetDatabaseLogon(_db_logon, _db_password);
            rptH.SetParameterValue("StartDate", startDate);
            rptH.SetParameterValue("EndDate", endDate);

            Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        [HttpGet]
        public ActionResult ITMeetingReport()
        {
            if (!Services.Authorizer.Authorize(Permissions.SupportAdmin) && !Services.Authorizer.Authorize(Permissions.SupportIT))
            {
                if (!Services.Authorizer.Authorize(Permissions.SupportIT, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
                if (!Services.Authorizer.Authorize(Permissions.SupportAdmin, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
            }
            return View();
        }

        [HttpPost]
        public ActionResult ITMeetingReport(string StartDate)
        {
            if (!Services.Authorizer.Authorize(Permissions.SupportAdmin) && !Services.Authorizer.Authorize(Permissions.SupportIT))
            {
                if (!Services.Authorizer.Authorize(Permissions.SupportIT, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
                if (!Services.Authorizer.Authorize(Permissions.SupportAdmin, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
            }
            DateTime startDate = DateTime.Parse(StartDate);

            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("~/Modules/Time.Support/Views/Report/ITMeeting.rpt");
            rptH.Load();
            rptH.SetDatabaseLogon(_db_logon, _db_password);
            rptH.SetParameterValue("StartDate", startDate);
            rptH.SetParameterValue("UserId", HttpContext.User.Identity.Name);

            Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        public ActionResult TicketsByResourcePriority()
        {
            if (!Services.Authorizer.Authorize(Permissions.SupportAdmin) && !Services.Authorizer.Authorize(Permissions.SupportIT))
            {
                if (!Services.Authorizer.Authorize(Permissions.SupportIT, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
                if (!Services.Authorizer.Authorize(Permissions.SupportAdmin, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
            }
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("~/Modules/Time.Support/Views/Report/ITRequests_ByResource_Priority.rpt");
            rptH.Load();
            rptH.SetDatabaseLogon(_db_logon, _db_password);
            Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        public ActionResult TicketsByResourceRatingPriority()
        {
            if (!Services.Authorizer.Authorize(Permissions.SupportAdmin) && !Services.Authorizer.Authorize(Permissions.SupportIT))
            {
                if (!Services.Authorizer.Authorize(Permissions.SupportIT, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
                if (!Services.Authorizer.Authorize(Permissions.SupportAdmin, T("You do not have access to this report. Please log in.")))
                    return new HttpUnauthorizedResult();
            }
            ReportClass rptH = new ReportClass();
            rptH.FileName = Server.MapPath("~/Modules/Time.Support/Views/Report/ITRequests_ByResource_Rating_Priority.rpt");
            rptH.Load();
            rptH.SetDatabaseLogon(_db_logon, _db_password);
            Stream stream = rptH.ExportToStream(ExportFormatType.PortableDocFormat);
            return File(stream, "application/pdf");
        }

        private void SetDBLogonForReport(ReportDocument reportDocument)
        {
            ConnectionInfo info = new ConnectionInfo();
            info.UserID = _db_logon;
            info.Password = _db_password;
            Tables tables = reportDocument.Database.Tables;
            foreach (Table table in tables)
            {
                TableLogOnInfo tableLogonInfo = table.LogOnInfo;
                tableLogonInfo.ConnectionInfo = info;
                table.ApplyLogOnInfo(tableLogonInfo);
            }
        }
    }
}