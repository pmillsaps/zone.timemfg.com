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
    
    public partial class V_PartInfo
    {
        public string Part_PartNum { get; set; }
        public string Part_PartDescription { get; set; }
        public Nullable<decimal> Calculated_Available { get; set; }
        public string Part_TypeCode { get; set; }
        public decimal PartPlant_SafetyQty { get; set; }
        public bool Part_InActive { get; set; }
        public string Part_ClassID { get; set; }
        public bool Part_NonStock { get; set; }
        public bool Part_QtyBearing { get; set; }
        public string PartPlant_BuyerID { get; set; }
        public int PartPlant_LeadTime { get; set; }
        public Nullable<System.DateTime> PartPlant_MRPLastRunDate { get; set; }
        public string PartPlant_SourceType { get; set; }
        public string Vendor_VendorID { get; set; }
        public string Vendor_Name { get; set; }
        public string PartRev_RevisionNum { get; set; }
        public Nullable<System.DateTime> PartRev_ApprovedDate { get; set; }
        public bool PartRev_Approved { get; set; }
        public Nullable<System.DateTime> PartRev_EffectiveDate { get; set; }
        public Nullable<decimal> PartWhse_OnHandQty { get; set; }
        public Nullable<decimal> PartWhse_DemandQty { get; set; }
        public string PartWhse_WarehouseCode { get; set; }
        public Nullable<decimal> AvgBurdenCost { get; set; }
        public Nullable<decimal> AvgLaborCost { get; set; }
        public Nullable<decimal> AvgMaterialCost { get; set; }
        public Nullable<decimal> AvgSubContCost { get; set; }
        public Nullable<decimal> AvgMtlBurCost { get; set; }
        public Nullable<decimal> Calculated_TotalAvgCost { get; set; }
    }
}
