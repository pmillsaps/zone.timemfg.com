//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Time.Invoicing.EntityModels.PcInvoice
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrderOption
    {
        public OrderOption()
        {
            this.OrderDetailsOrderOptionsLinks = new HashSet<OrderDetailsOrderOptionsLink>();
        }
    
        public int Id { get; set; }
        public int OrderHeaderId { get; set; }
        public int Quantity { get; set; }
        public string OptionNum { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool Print { get; set; }
        public int OptionSeq { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public bool MiscCharge { get; set; }
        public Nullable<int> MiscChargeTypeId { get; set; }
    
        public virtual MiscChargeType MiscChargeType { get; set; }
        public virtual ICollection<OrderDetailsOrderOptionsLink> OrderDetailsOrderOptionsLinks { get; set; }
        public virtual OrderHeader OrderHeader { get; set; }
    }
}