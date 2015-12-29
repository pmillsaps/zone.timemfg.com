using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Time.Epicor.Helpers
{
    public class ExporttoExcelResult : ActionResult
    {
        private readonly List<object> _data;
        private readonly string _filename;
        private readonly Type _type;

        public ExporttoExcelResult(string fileName, List<object> data)
        {
            _filename = fileName;
            _type = data.GetType().GetGenericArguments()[0];
            _data = data;
        }

        //public override void ExecuteResult(ControllerContext context)
        //{
        //    using (ExcelPackage pck = new ExcelPackage())
        //    {
        //        //IList query = (IList)Activator.CreateInstance(_type);
        //        var query = ListToExcel(_data);

        //        //Create the worksheet
        //        ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Result");
        //        //get our column headings

        //        var Headings = _type.GetProperties();
        //        for (int i = 0; i < Headings.Count(); i++)
        //        {
        //            ws.Cells[1, i + 1].Value = Headings[i].Name;
        //        }
        //        //populate our Data
        //        if (query.Count() > 0)
        //        {
        //            ws.Cells["A2"].LoadFromCollection(query);
        //        }

        //        //Format the header
        //        using (ExcelRange rng = ws.Cells["A1:BZ1"])
        //        {
        //            rng.Style.Font.Bold = true;
        //            rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
        //            rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
        //            rng.Style.Font.Color.SetColor(Color.White);
        //        }

        //        //Write it back to the client
        //        HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        HttpContext.Current.Response.AddHeader("content-disposition", "attachment;  filename=ExcelDemo.xlsx");
        //        HttpContext.Current.Response.BinaryWrite(pck.GetAsByteArray());
        //        HttpContext.Current.Response.End();
        //    }
        //}

        private List<T> ListToExcel<T>(List<T> query)
        {
            var t = typeof(T);
            return query;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var grid = new GridView();
            List<object> _dataList = (_data as IEnumerable<object>).Cast<object>().ToList();
            grid.DataSource = _data;
            grid.DataBind();

            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add(_filename);
            //workSheet.Cells["A1"].LoadFromDataTable(((DataTable)grid.DataSource), true);
            //workSheet.Cells.AutoFitColumns();
            var totalCols = grid.Rows[0].Cells.Count;
            var totalRows = grid.Rows.Count;
            var headerRow = grid.HeaderRow;
            for (var i = 1; i <= totalCols; i++)
            {
                workSheet.Cells[1, i].Value = headerRow.Cells[i - 1].Text;
            }
            for (var j = 1; j <= totalRows; j++)
            {
                for (var i = 1; i <= totalCols; i++)
                {
                    var data = _data.ElementAt(j - 1);
                    workSheet.Cells[j + 1, i].Value = data.GetType().GetProperty(headerRow.Cells[i - 1].Text).GetValue(data, null);
                }
            }
            workSheet.Cells.AutoFitColumns();
            //Format the header
            using (ExcelRange rng = workSheet.Cells["A1:BZ1"])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
                rng.Style.Font.Color.SetColor(Color.White);
            }
            using (var memoryStream = new MemoryStream())
            {
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Current.Response.AddHeader("content-disposition", String.Format("attachment;  filename={0}.xlsx", _filename));
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();
            }

            //System.Web.HttpContext.Current.Response.ClearContent();
            //System.Web.HttpContext.Current.Response.AddHeader("content-disposition", String.Format("attachment; filename={0}.xls", _filename));

            //System.Web.HttpContext.Current.Response.ContentType = "application/excel";

            //var sw = new StringWriter();

            //var htw = new HtmlTextWriter(sw);

            //grid.RenderControl(htw);

            //System.Web.HttpContext.Current.Response.Write(sw.ToString());
            //System.Web.HttpContext.Current.Response.End();
        }
    }
}