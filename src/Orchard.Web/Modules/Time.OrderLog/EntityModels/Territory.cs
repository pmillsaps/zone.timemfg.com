//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Time.OrderLog.EntityModels
{
    using System;
    using System.Collections.Generic;
    
    public partial class Territory
    {
        public Territory()
        {
            this.Dealers = new HashSet<Dealer>();
        }
    
        public int TerritoryId { get; set; }
        public string TerritoryName { get; set; }
    
        public virtual ICollection<Dealer> Dealers { get; set; }
    }
}
