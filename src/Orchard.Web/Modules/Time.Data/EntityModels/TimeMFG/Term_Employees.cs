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
    
    public partial class Term_Employees
    {
        public Term_Employees()
        {
            this.Term_ITInfo = new HashSet<Term_ITInfo>();
            this.Term_Property = new HashSet<Term_Property>();
        }
    
        public int Id { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        public bool Terminated { get; set; }
        public Nullable<System.DateTime> TerminatedDate { get; set; }
    
        public virtual ICollection<Term_ITInfo> Term_ITInfo { get; set; }
        public virtual ICollection<Term_Property> Term_Property { get; set; }
    }
}