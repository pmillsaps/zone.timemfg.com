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
    
    public partial class MiscInstall
    {
        public int Id { get; set; }
        public int OrderHeaderId { get; set; }
        public int OrderDetailId { get; set; }
        public string Note { get; set; }
        public string InstallPO { get; set; }
        public decimal InstallPrice { get; set; }
    
        public virtual OrderDetail OrderDetail { get; set; }
        public virtual OrderHeader OrderHeader { get; set; }
    }
}