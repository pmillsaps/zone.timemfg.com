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
    
    public partial class v_JobInformation
    {
        public Nullable<int> ordernum { get; set; }
        public string jobnum { get; set; }
        public string partnum { get; set; }
        public string serialnumber { get; set; }
        public string snstatus { get; set; }
        public Nullable<byte> jobclosed { get; set; }
        public Nullable<byte> jobcomplete { get; set; }
        public Nullable<byte> jobengineered { get; set; }
        public Nullable<byte> jobreleased { get; set; }
        public Nullable<byte> jobheld { get; set; }
        public Nullable<byte> jobfirm { get; set; }
        public long ID { get; set; }
        public Nullable<System.DateTime> reqduedate { get; set; }
        public Nullable<int> orderline { get; set; }
        public Nullable<int> orderrelnum { get; set; }
        public string shiptonum { get; set; }
        public Nullable<int> custnum { get; set; }
        public string ShipToname { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string country { get; set; }
        public string CustName { get; set; }
        public string ponum { get; set; }
        public string custid { get; set; }
    }
}