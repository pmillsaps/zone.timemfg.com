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
    
    public partial class V_YardReport
    {
        public string JobNum { get; set; }
        public string OpCode { get; set; }
        public Nullable<int> OrderNum { get; set; }
        public Nullable<int> OrderLine { get; set; }
        public Nullable<int> OrderRelNum { get; set; }
        public int OprSeq { get; set; }
        public Nullable<System.DateTime> ClockInDate { get; set; }
        public string SerialNumber { get; set; }
        public Nullable<System.DateTime> ShipDate { get; set; }
        public string location { get; set; }
        public string Name { get; set; }
        public string PartNum { get; set; }
        public Nullable<System.DateTime> TestDate { get; set; }
        public Nullable<System.DateTime> GreenDotDate { get; set; }
        public System.Guid ID { get; set; }
    }
}
