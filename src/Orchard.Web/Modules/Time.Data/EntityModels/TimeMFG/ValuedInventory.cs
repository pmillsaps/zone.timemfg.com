//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Time.Data.EntityModels.TimeMFG
{
    using System;
    using System.Collections.Generic;
    
    public partial class ValuedInventory
    {
        public int Id { get; set; }
        public string PartNum { get; set; }
        public string Description { get; set; }
        public decimal BOH { get; set; }
        public System.DateTime CalculationDate { get; set; }
        public System.DateTime TransactionDate { get; set; }
        public decimal TransactionCost { get; set; }
        public decimal CurrentCost { get; set; }
        public System.DateTime ComparisonDate { get; set; }
        public int PeriodYear { get; set; }
        public string ClassId { get; set; }
    }
}
