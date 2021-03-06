//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Time.Data.EntityModels.OrderLog
{
    using System;
    using System.Collections.Generic;
    
    public partial class Order
    {
        public Order()
        {
            this.OrderLineUnits = new HashSet<OrderLineUnit>();
            this.OrderTrans = new HashSet<OrderTran>();
        }
    
        public int OrderId { get; set; }
        public string PO { get; set; }
        public int DealerId { get; set; }
        public System.DateTime Date { get; set; }
        public string Customer { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public int TerritoryId { get; set; }
        public Nullable<int> InstallId { get; set; }
        public Nullable<int> InstallerId { get; set; }
        public bool Special { get; set; }
        public bool Stock { get; set; }
        public bool Demo { get; set; }
        public bool RTG { get; set; }
        public bool TruGuard { get; set; }
        public bool GSA { get; set; }
        public Nullable<int> Price { get; set; }
    
        public virtual ICollection<OrderLineUnit> OrderLineUnits { get; set; }
        public virtual ICollection<OrderTran> OrderTrans { get; set; }
        public virtual Dealer Dealer { get; set; }
        public virtual Install Install { get; set; }
        public virtual Installer Installer { get; set; }
        public virtual Territory Territory { get; set; }
    }
}
