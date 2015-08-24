using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Time.Configurator.Helpers
{
    public class ExporttoExcelResult : ActionResult
    {
        private readonly object _data;
        private readonly string _filename;

        public ExporttoExcelResult(string fileName, object data)
        {
            _filename = fileName;
            _data = data;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var grid = new GridView();
            grid.DataSource = _data;
            grid.DataBind();

            System.Web.HttpContext.Current.Response.ClearContent();
            System.Web.HttpContext.Current.Response.AddHeader("content-disposition", String.Format("attachment; filename={0}.xls", _filename));

            System.Web.HttpContext.Current.Response.ContentType = "application/excel";

            var sw = new StringWriter();

            var htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            System.Web.HttpContext.Current.Response.Write(sw.ToString());
            System.Web.HttpContext.Current.Response.End();
        }
    }
}