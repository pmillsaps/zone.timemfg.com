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
    
    public partial class FobLookup
    {
        public FobLookup()
        {
            this.OrderHeaders = new HashSet<OrderHeader>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
    
        public virtual ICollection<OrderHeader> OrderHeaders { get; set; }
    }
}
