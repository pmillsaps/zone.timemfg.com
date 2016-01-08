using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Time.Data.EntityModels.TimeMFG;
using Time.Epicor.Models;

namespace Time.Epicor.Helpers
{
    public class ValuedInventoryExcelResult : ActionResult
    {
        private List<ValuedInventoryExt> data;
        private List<V_ValuedInventoryByPeriod> data2;
        private List<IGrouping<object, ValuedInventoryExt>> data21;
        private List<V_ClassIdSummary> data3;
        private string filename;
        private List<ValuedInventoryExt> _data;
        private readonly string _filename;
        private readonly Type _type;

        public ValuedInventoryExcelResult(string filename, List<ValuedInventoryExt> _data, List<IGrouping<object, ValuedInventoryExt>> data21, List<V_ClassIdSummary> data3)
        {
            this.filename = filename;
            this._filename = filename;
            this._data = _data;
            this.data21 = data21;
            this.data3 = data3;
        }

        public ValuedInventoryExcelResult(string filename, List<ValuedInventoryExt> data, List<V_ValuedInventoryByPeriod> data2, List<V_ClassIdSummary> data3)
        {
            this.filename = filename;
            this._filename = filename;
            this.data = data;
            this.data2 = data2;
            this.data3 = data3;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var grid = new GridView();
            //List<object> _dataList = (_data as IEnumerable<object>).Cast<object>().ToList();
            grid.DataSource = data;
            grid.DataBind();

            ExcelPackage excel = new ExcelPackage();
            GetWorkSheet(grid, excel, filename);
            grid.DataSource = data2;
            grid.DataBind();
            GetWorkSheet(grid, excel, "Summary");

            grid.DataSource = data3;
            grid.DataBind();
            GetWorkSheet(grid, excel, "Summary2");

            using (var memoryStream = new MemoryStream())
            {
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Current.Response.AddHeader("content-disposition", String.Format("attachment;  filename={0}.xlsx", _filename));
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();
            }
        }

        private void GetWorkSheet(GridView grid, ExcelPackage excel, string sheetName)
        {
            var workSheet = excel.Workbook.Worksheets.Add(sheetName);
            //workSheet.Cells["A1"].LoadFromDataTable(((DataTable)grid.DataSource), true);
            //workSheet.Cells.AutoFitColumns();

            DataTable dt = new DataTable("GridView_Data");
            foreach (TableCell cell in grid.HeaderRow.Cells)
            {
                dt.Columns.Add(cell.Text);
            }
            foreach (GridViewRow row in grid.Rows)
            {
                dt.Rows.Add();
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    dt.Rows[dt.Rows.Count - 1][i] = row.Cells[i].Text;
                }
            }
            workSheet.Cells["A1"].LoadFromDataTable(dt, true);
            workSheet.Cells.AutoFitColumns();

            //var totalCols = grid.Rows[0].Cells.Count;
            //var totalRows = grid.Rows.Count;
            //var headerRow = grid.HeaderRow;
            //for (var i = 1; i <= totalCols; i++)
            //{
            //    workSheet.Cells[1, i].Value = headerRow.Cells[i - 1].Text;
            //}
            //for (var j = 1; j <= totalRows; j++)
            //{
            //    for (var i = 1; i <= totalCols; i++)
            //    {
            //        var data = _data.ElementAt(j - 1);
            //        workSheet.Cells[j + 1, i].Value = data.GetType().GetProperty(headerRow.Cells[i - 1].Text).GetValue(data, null);
            //    }
            //}
            //workSheet.Cells.AutoFitColumns();
            //Format the header
            using (ExcelRange rng = workSheet.Cells["A1:BZ1"])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
                rng.Style.Font.Color.SetColor(Color.White);
            }
        }
    }
}