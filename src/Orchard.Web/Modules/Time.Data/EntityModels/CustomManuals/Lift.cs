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
    
    public partial class Lift
    {
        public string Lift_ID { get; set; }
        public string Lift_Group { get; set; }
    
        public virtual LiftGroup LiftGroup { get; set; }
    }
}
