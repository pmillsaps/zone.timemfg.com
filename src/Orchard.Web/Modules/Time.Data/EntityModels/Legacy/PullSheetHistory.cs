//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Time.Data.EntityModels.Legacy
{
    using System;
    using System.Collections.Generic;
    
    public partial class PullSheetHistory
    {
        public int ID { get; set; }
        public string Company { get; set; }
        public string SerialNumber { get; set; }
        public string ProductCode { get; set; }
        public string ItemCode { get; set; }
        public string ReqCode { get; set; }
        public string PrimaryReqCode { get; set; }
        public string Description { get; set; }
        public string UnitOfMeasure { get; set; }
        public string PartType { get; set; }
        public string PullDate { get; set; }
        public string QtyPulled { get; set; }
        public string LiftNumber { get; set; }
        public string UpperLevelItemCode { get; set; }
        public Nullable<int> Qty { get; set; }
    }
}