//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Time.Data.EntityModels.Install
{
    using System;
    using System.Collections.Generic;
    
    public partial class InstallDetail
    {
        public int Id { get; set; }
        public int InstallQuoteId { get; set; }
        public int GroupId { get; set; }
        public int OptionId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Nullable<decimal> ExtendedPrice { get; set; }
        public decimal InstallHours { get; set; }
    
        public virtual OptionGroup OptionGroup { get; set; }
        public virtual InstallQuote InstallQuote { get; set; }
        public virtual VSWOption VSWOption { get; set; }
    }
}