using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Time.Support.Helpers
{
    public class CrystalReportPdfResult : ActionResult
    {
        private const string _db_logon = "TimeMFGApp";
        private const string _db_password = "Tm@Time$!";
        private readonly byte[] _contentBytes;

        public CrystalReportPdfResult(string reportPath)
        {
            ReportDocument reportDocument = new ReportDocument();
            reportDocument.Load(reportPath);
            reportDocument.SetDatabaseLogon(_db_logon, _db_password);

            SetDBLogonForReport(reportDocument);
            _contentBytes = StreamToBytes(reportDocument.ExportToStream(ExportFormatType.PortableDocFormat));
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.ApplicationInstance.Response;
            response.Clear();
            response.Buffer = false;
            response.ClearContent();
            response.ClearHeaders();
            response.Cache.SetCacheability(HttpCacheability.Public);
            response.ContentType = "application/pdf";

            using (var stream = new MemoryStream(_contentBytes))
            {
                stream.WriteTo(response.OutputStream);
                stream.Flush();
            }
        }

        private static byte[] StreamToBytes(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private void SetDBLogonForReport(ReportDocument reportDocument)
        {
            ConnectionInfo info = new ConnectionInfo()
            {
                ServerName = "Aruba-Sql",
                DatabaseName = "TimeMFG",
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
    }
}