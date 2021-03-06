//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Time.Data.EntityModels.Production
{
    using System;
    using System.Collections.Generic;
    
    public partial class V_NowReportClaim
    {
        public string Model { get; set; }
        public string JobNumber { get; set; }
        public Nullable<System.DateTime> LastPrinted { get; set; }
        public string ULPart { get; set; }
        public string Part { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> QtyShortage { get; set; }
        public Nullable<decimal> QtyOnHand { get; set; }
        public Nullable<decimal> QtyAvailable { get; set; }
        public string Bin { get; set; }
        public string BuyerID { get; set; }
        public string Name { get; set; }
        public string SupplierId { get; set; }
        public string VendorName { get; set; }
        public decimal RequiredQty { get; set; }
        public decimal IssuedQty { get; set; }
        public bool IssuedCompleted { get; set; }
        public Nullable<System.DateTime> OperationStartDate { get; set; }
        public int RelatedOperation { get; set; }
        public string RelOpDescription { get; set; }
        public bool BackFlush { get; set; }
        public int AssemblySeq { get; set; }
        public Nullable<System.DateTime> JobStartDate { get; set; }
        public int MtlSeq { get; set; }
        public bool Firm { get; set; }
        public bool Released { get; set; }
        public bool Completed { get; set; }
        public bool Closed { get; set; }
        public Nullable<System.DateTime> CompletedDate { get; set; }
        public Nullable<System.DateTime> JobAsmblStartDate { get; set; }
        public string PartType { get; set; }
        public string PartClass { get; set; }
        public Nullable<decimal> OnHand { get; set; }
        public string SerialNumber { get; set; }
        public Nullable<System.DateTime> DrawStepDate { get; set; }
        public string PrimBin { get; set; }
        public Nullable<System.Guid> BinID { get; set; }
        public System.Guid Id { get; set; }
    }
}
