//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Time.Data.EntityModels.CustomManuals
{
    using System;
    using System.Collections.Generic;
    
    public partial class LiftGroup
    {
        public LiftGroup()
        {
            this.Formattings = new HashSet<Formatting>();
            this.Lifts = new HashSet<Lift>();
        }
    
        public string Lift_Group { get; set; }
    
        public virtual ICollection<Formatting> Formattings { get; set; }
        public virtual ICollection<Lift> Lifts { get; set; }
    }
}
