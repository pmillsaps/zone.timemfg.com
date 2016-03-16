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
    
    public partial class TicketDepartment
    {
        public TicketDepartment()
        {
            this.TicketProjects = new HashSet<TicketProject>();
        }
    
        public int DepartmentID { get; set; }
        public string Prefix { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<int> SupervisorID { get; set; }
        public Nullable<bool> ITOnly { get; set; }
    
        public virtual TicketEmployee TicketEmployee { get; set; }
        public virtual ICollection<TicketProject> TicketProjects { get; set; }
    }
}