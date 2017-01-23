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
    
    public partial class V_PartCostStd
    {
        public string PartNum { get; set; }
        public decimal MfgLotSize { get; set; }
        public decimal CostingLotSize { get; set; }
        public string OpCode { get; set; }
        public string RevisionNum { get; set; }
        public bool Approved { get; set; }
        public decimal EstSetHours { get; set; }
        public decimal ProdStandard { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public decimal EstProdHours { get; set; }
        public string opcode2 { get; set; }
        public string ResourceGrpID { get; set; }
        public decimal ProdBurRate { get; set; }
        public decimal ProdLabRate { get; set; }
        public decimal SetupBurRate { get; set; }
        public decimal SetupLabRate { get; set; }
        public Nullable<decimal> LaborCost { get; set; }
        public Nullable<decimal> BurdenCost { get; set; }
        public System.Guid ID { get; set; }
        public bool SubContract { get; set; }
        public decimal EstUnitCost { get; set; }
        public int OprSeq { get; set; }
        public string AltMethod { get; set; }
    }
}
