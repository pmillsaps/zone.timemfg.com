using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.Production;
using Time.Epicor.Models;

namespace Time.Epicor.ViewModels
{
    public class NOWReportViewModel
    {
        private ProductionEntities db;

        // Three enums to display the data in the drop down list in the view
        public enum SearchFilter
        {
            PartNumber,
            SerialNumber,
            JobNumber
        }

        public enum RelatedOps
        {
            AllParts,
            Box_Ship_Post,
            Non_Box_Ship_Post
        }

        public enum BasedOperation
        {
            Draw_9015,
            Claim_9160
        }

        // Fields to be display in the view
        public string Query { get; set; }

        public List<NOWReport> Report { get; set; }

        [DisplayName("Search By:")]
        public SearchFilter Filter { get; set; }

        [DisplayName("Related Ops:")]
        public RelatedOps Ops { get; set; }

        [DisplayName("Based on Operation:")]
        public BasedOperation BasedOp { get; set; }

        [DisplayName("Export To Excel")]
        public bool ExportToExcel { get; set; }

        // Two constructors
        public NOWReportViewModel(string search = "")
        {
            db = new ProductionEntities();

            Query = search;
            Filter = SearchFilter.PartNumber;
            Ops = RelatedOps.AllParts;
            BasedOp = BasedOperation.Draw_9015;
        }

        public NOWReportViewModel()
        {
            db = new ProductionEntities();

            Query = "";
            Filter = SearchFilter.PartNumber;
            Ops = RelatedOps.AllParts;
            BasedOp = BasedOperation.Draw_9015;
        }

        // Method called in the view to fetch and filter the data for the NowReport and the NowReportClaim
        public void GetData()
        {
            using(var db = new ProductionEntities())
            {
                var _totalUsed = new Dictionary<string, decimal>(); // Helps with coloring the rows in the view

                var model = GetNowReport(); // Storing the results of the query

                // Choosing between the NowReport and the NowReportClaim
                switch (BasedOp)
                {
                    case BasedOperation.Draw_9015:
                        break;
                    case BasedOperation.Claim_9160:
                        model = GetNowReportClaim();
                        break;
                    default:
                        break;
                }

                // Filtering the model by Part #, Serial #, or Job #
                if (!String.IsNullOrEmpty(Query))
                {
                    switch (Filter)
                    {
                        case SearchFilter.PartNumber:
                            model = model.Where(x => x.Part.Contains(Query));
                            break;
                        case SearchFilter.SerialNumber:
                            model = model.Where(x => x.SerialNumber.Contains(Query));
                            break;
                        case SearchFilter.JobNumber:
                            model = model.Where(x => x.JobNumber.Contains(Query));
                            break;
                        default:
                            break;
                    } 
                }
                
                // Filtering the model based on Related Operations
                switch (Ops)    
                {
                    case RelatedOps.AllParts:
                        break;
                    case RelatedOps.Box_Ship_Post:
                        model = model.Where(x => x.RelOpDescription == "BOX" || x.RelOpDescription == "SHIP" || x.RelOpDescription == "POST");
                        break;
                    case RelatedOps.Non_Box_Ship_Post:
                        model = model.Where(x => x.RelOpDescription != "BOX" && x.RelOpDescription != "SHIP" && x.RelOpDescription != "POST");
                        break;
                    default:
                        break;
                }

                // Assigning values to the Dictionary to color the rows in the view
                _totalUsed = (from col in model group col by col.Part into g select new { part = g.Key, Total = g.Sum(x => x.QtyShortage) }).ToDictionary(x => x.part, x => x.Total);

                List<NOWReport> filteredReport = model.ToList();

                foreach (var item in filteredReport)
                {
                    if (item.QtyAvailable >= 0)
                    {
                        if (item.QtyOnHand < _totalUsed.Where(x => x.Key == item.Part).First().Value)
                            item.RowColor = "Orange"; //Orange (Partial)
                        else
                            item.RowColor = "Green"; //Green (Covered)
                    }
                    else
                        item.RowColor = "Blue";   
                }                

                // Sending the report back to the view
                Report = filteredReport.ToList(); 
            }
        }

        // This method fetches the data for the NowReport
        public IQueryable<NOWReport> GetNowReport()
        {
            db.Database.CommandTimeout = 300;
            var qry = db.V_NowReport.Select(x => new NOWReport
            {
                Model = x.Model,
                JobNumber = x.JobNumber,
                LastPrinted = (x.LastPrinted) ?? new DateTime(1900, 1, 1),
                ULPart = x.ULPart,
                Part = x.Part,
                Description = x.Description,
                QtyShortage = x.QtyShortage ?? 0,
                QtyOnHand = x.QtyOnHand ?? 0,
                QtyAvailable = x.QtyAvailable ?? -(x.QtyShortage ?? 0),
                Bin = x.Bin ?? x.PrimBin,
                BuyerId = x.BuyerID,
                Name = x.Name,
                SupplierId = x.SupplierId,
                VendorName = x.VendorName,
                RequiredQty = x.RequiredQty,
                IssuedQty = x.IssuedQty,
                IssueCompleted = x.IssuedCompleted,
                OperationStartDate = x.OperationStartDate ?? new DateTime(1900, 1, 1),
                RelatedOperation = x.RelatedOperation,
                RelOpDescription = x.RelOpDescription,
                BackFlush = x.BackFlush,
                AssemblySeq = x.AssemblySeq,
                JobStartDate = x.JobStartDate ?? new DateTime(1900, 1, 1),
                MtlSeq = x.MtlSeq,
                Firm = x.Firm,
                Released = x.Released,
                Completed = x.Completed,
                Closed = x.Closed,
                CompletedDate = x.CompletedDate ?? new DateTime(1900, 1, 1),
                JobAsmblStartDate = x.JobAsmblStartDate ?? new DateTime(1900, 1, 1),
                PartType = x.PartType,
                PartClass = x.PartClass,
                OnHand = x.OnHand ?? 0,
                SerialNumber = x.SerialNumber,
                DrawStepDate = x.DrawStepDate ?? new DateTime(1900, 1, 1),
                Id = x.Id,
                RowColor = ""
            }).Where(x => x.PartClass != "PNT" && x.BackFlush != true);

            return qry;
        }

        // This method fetches the data for the NowReportClaim
        public IQueryable<NOWReport> GetNowReportClaim()
        {
            db.Database.CommandTimeout = 300;
            var qry = db.V_NowReportClaim.Select(x => new NOWReport
            {
                Model = x.Model,
                JobNumber = x.JobNumber,
                LastPrinted = (x.LastPrinted) ?? new DateTime(1900, 1, 1),
                ULPart = x.ULPart,
                Part = x.Part,
                Description = x.Description,
                QtyShortage = x.QtyShortage ?? 0,
                QtyOnHand = x.QtyOnHand ?? 0,
                QtyAvailable = x.QtyAvailable ?? -(x.QtyShortage ?? 0),
                Bin = x.Bin ?? x.PrimBin,
                BuyerId = x.BuyerID,
                Name = x.Name,
                SupplierId = x.SupplierId,
                VendorName = x.VendorName,
                RequiredQty = x.RequiredQty,
                IssuedQty = x.IssuedQty,
                IssueCompleted = x.IssuedCompleted,
                OperationStartDate = x.OperationStartDate ?? new DateTime(1900, 1, 1),
                RelatedOperation = x.RelatedOperation,
                RelOpDescription = x.RelOpDescription,
                BackFlush = x.BackFlush,
                AssemblySeq = x.AssemblySeq,
                JobStartDate = x.JobStartDate ?? new DateTime(1900, 1, 1),
                MtlSeq = x.MtlSeq,
                Firm = x.Firm,
                Released = x.Released,
                Completed = x.Completed,
                Closed = x.Closed,
                CompletedDate = x.CompletedDate ?? new DateTime(1900, 1, 1),
                JobAsmblStartDate = x.JobAsmblStartDate ?? new DateTime(1900, 1, 1),
                PartType = x.PartType,
                PartClass = x.PartClass,
                OnHand = x.OnHand ?? 0,
                SerialNumber = x.SerialNumber,
                DrawStepDate = x.DrawStepDate ?? new DateTime(1900, 1, 1),
                Id = x.Id,
                RowColor = ""
            }).Where(x => x.PartClass != "PNT" && x.BackFlush != true);

            return qry;
        }
    }
}