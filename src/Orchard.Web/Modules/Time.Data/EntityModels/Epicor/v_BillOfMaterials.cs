//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Time.Data.EntityModels.Epicor
{
    using System;
    using System.Collections.Generic;
    
    public partial class v_BillOfMaterials
    {
        public string partnum { get; set; }
        public string mtlpartnum { get; set; }
        public string partdescription { get; set; }
        public string typecode { get; set; }
        public Nullable<byte> inactive { get; set; }
        public Nullable<decimal> qtyper { get; set; }
        public Nullable<decimal> baseprice { get; set; }
        public Nullable<int> vendornum { get; set; }
        public string personid { get; set; }
        public string buyerid { get; set; }
        public Nullable<decimal> mfglotsize { get; set; }
        public string vendorid { get; set; }
        public string revisionnum { get; set; }
        public Nullable<byte> fixedqty { get; set; }
        public Nullable<int> relatedoperation { get; set; }
        public Nullable<int> mtlseq { get; set; }
        public string name { get; set; }
        public Nullable<byte> phantombom { get; set; }
        public Nullable<decimal> lastmaterialcost { get; set; }
        public Nullable<byte> viewasasm { get; set; }
        public string classid { get; set; }
        public string altmethod { get; set; }
        public Nullable<decimal> lastsubcontcost { get; set; }
        public Nullable<decimal> lastmtlburcost { get; set; }
        public Nullable<decimal> lastlaborcost { get; set; }
        public Nullable<decimal> lastburdencost { get; set; }
        public Nullable<int> leadtime { get; set; }
        public string opcode { get; set; }
        public Nullable<decimal> binqtyonhand { get; set; }
        public Nullable<decimal> onhandqty { get; set; }
        public Nullable<decimal> AllocQty { get; set; }
        public Nullable<decimal> UnfirmAllocQty { get; set; }
        public string Latest_Rev { get; set; }
        public string Current_Rev { get; set; }
        public string VenPartNum { get; set; }
        public Nullable<int> OpenJobCount { get; set; }
        public long Id { get; set; }
        public Nullable<int> OpenJobs { get; set; }
    }
}