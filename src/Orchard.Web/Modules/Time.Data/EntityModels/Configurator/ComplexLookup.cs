//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Time.Data.EntityModels.Configurator
{
    using System;
    using System.Collections.Generic;
    
    public partial class ComplexLookup
    {
        public ComplexLookup()
        {
            this.ComplexLinks = new HashSet<ComplexLink>();
        }
    
        public int Id { get; set; }
        public string ConfigName { get; set; }
        public string ConfigData { get; set; }
        public string LookupData { get; set; }
        public Nullable<bool> Hold { get; set; }
        public string LookupDataHold { get; set; }
        public Nullable<bool> Verified { get; set; }
    
        public virtual ICollection<ComplexLink> ComplexLinks { get; set; }
    }
}
