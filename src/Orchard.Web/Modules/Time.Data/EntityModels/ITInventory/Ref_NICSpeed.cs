//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Time.Data.EntityModels.ITInventory
{
    using System;
    using System.Collections.Generic;
    
    public partial class Ref_NICSpeed
    {
        public Ref_NICSpeed()
        {
            this.Ref_NIC = new HashSet<Ref_NIC>();
        }
    
        public int Id { get; set; }
        public string NIC_Speed { get; set; }
    
        public virtual ICollection<Ref_NIC> Ref_NIC { get; set; }
    }
}