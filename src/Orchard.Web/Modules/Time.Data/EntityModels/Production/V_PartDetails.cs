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
    
    public partial class V_PartDetails
    {
        public string PartNum { get; set; }
        public string PartDescription { get; set; }
        public string TypeCode { get; set; }
        public bool InActive { get; set; }
        public bool PhantomBOM { get; set; }
        public Nullable<decimal> BasePrice { get; set; }
        public Nullable<int> VendorNum { get; set; }
        public string PersonID { get; set; }
        public string BuyerID { get; set; }
        public string VendorID { get; set; }
        public string RevisionNum { get; set; }
        public string ECO { get; set; }
        public string DrawNum { get; set; }
        public Nullable<decimal> OnHandQty { get; set; }
        public string Name { get; set; }
        public string ClassID { get; set; }
        public string SubPart { get; set; }
        public string ProdCode { get; set; }
        public Nullable<decimal> LastMaterialCost { get; set; }
        public string VenPartNum { get; set; }
        public string BinNum { get; set; }
        public Nullable<decimal> binqty { get; set; }
        public string buyername { get; set; }
        public Nullable<decimal> binqtyonhand { get; set; }
        public Nullable<int> OpenJobCount { get; set; }
        public System.Guid Id { get; set; }
        public string PrimBin { get; set; }
        public Nullable<decimal> AvgLaborCost { get; set; }
        public Nullable<decimal> AvgBurdenCost { get; set; }
        public Nullable<decimal> AvgMaterialCost { get; set; }
        public Nullable<decimal> AvgSubContCost { get; set; }
        public Nullable<decimal> AvgMtlBurCost { get; set; }
    }
}
