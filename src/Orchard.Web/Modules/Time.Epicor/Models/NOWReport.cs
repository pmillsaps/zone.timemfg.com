using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.Epicor.Models
{
    public class NOWReport
    {
        public string SerialNumber { get; set; }
        public string Model { get; set; }
        public string JobNumber { get; set; }
        public string ULPart { get; set; }
        public String Part { get; set; }
        public string Description { get; set; }
        public int AssemblySeq { get; set; }
        public int MtlSeq { get; set; }
        public string RelOpDescription { get; set; }
        public Decimal QtyShortage { get; set; }
        public Decimal OnHand { get; set; }
        public Decimal QtyAvailable { get; set; }
        public string Bin { get; set; }
        public string BuyerId { get; set; }
        public string Name { get; set; }
        public DateTime LastPrinted { get; set; }
        public string SupplierId { get; set; }
        public string VendorName { get; set; }
        public Decimal RequiredQty { get; set; }
        public Decimal IssuedQty { get; set; }
        public bool IssueCompleted { get; set; }
        public DateTime OperationStartDate { get; set; }
        public int RelatedOperation { get; set; }
        public Boolean BackFlush { get; set; }
        public DateTime DrawStepDate { get; set; }
        public DateTime JobStartDate { get; set; }
        public bool Firm { get; set; }
        public bool Released { get; set; }
        public bool Closed { get; set; }
        public bool Completed { get; set; }
        public DateTime CompletedDate { get; set; }
        public DateTime JobAsmblStartDate { get; set; }
        public Decimal QtyOnHand { get; set; }
        public string PartType { get; set; }
        public string PartClass { get; set; }
        public Guid Id { get; set; }
        public string PrimBin { get; set; }
        public string RowColor { get; set; }
    }
}