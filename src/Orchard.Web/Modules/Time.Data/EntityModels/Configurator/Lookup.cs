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
    
    public partial class Lookup
    {
        public Lookup()
        {
            this.ComplexLinks = new HashSet<ComplexLink>();
        }
    
        public int Id { get; set; }
        public string ConfigName { get; set; }
        public string ConfigData { get; set; }
        public int Sequence { get; set; }
        public string Data { get; set; }
        public bool PickDefault { get; set; }
        public bool Inactive { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<ComplexLink> ComplexLinks { get; set; }
    }
}
