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
    
    public partial class Order
    {
        public Order()
        {
            this.OrderLines = new HashSet<OrderLine>();
        }
    
        public int OrderId { get; set; }
        public string PO { get; set; }
        public int DealerId { get; set; }
        public System.DateTime Date { get; set; }
    
        public virtual Dealer Dealer { get; set; }
        public virtual ICollection<OrderLine> OrderLines { get; set; }
    }
}
