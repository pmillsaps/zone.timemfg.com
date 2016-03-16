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
    
    public partial class TicketTask
    {
        public int ID { get; set; }
        public int TicketID { get; set; }
        public Nullable<int> AssignedTo { get; set; }
        public string Task { get; set; }
        public bool Completed { get; set; }
        public Nullable<System.DateTime> CompletionDate { get; set; }
        public string Notes { get; set; }
        public string Requestor { get; set; }
        public System.DateTime RequestDate { get; set; }
    
        public virtual TicketEmployee TicketEmployee { get; set; }
        public virtual TicketProject TicketProject { get; set; }
    }
}