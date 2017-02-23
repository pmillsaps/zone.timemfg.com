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
    
    public partial class OrderDetail
    {
        public OrderDetail()
        {
            this.InvoiceHistories = new HashSet<InvoiceHistory>();
            this.MiscInstalls = new HashSet<MiscInstall>();
            this.OrderDetailsOrderOptionsLinks = new HashSet<OrderDetailsOrderOptionsLink>();
        }
    
        public int Id { get; set; }
        public int Sequence { get; set; }
        public int OrderHeaderId { get; set; }
        public string PurchaseOrder { get; set; }
        public string VehicleNumber { get; set; }
        public string VIN { get; set; }
        public string DropShipCode { get; set; }
        public Nullable<System.DateTime> ScheduledBuildDate { get; set; }
        public Nullable<System.DateTime> ActualBuildDate { get; set; }
        public Nullable<System.DateTime> EstimatedShipDate { get; set; }
        public Nullable<System.DateTime> ActualShipDate { get; set; }
        public string BodyType { get; set; }
        public string BodyPO { get; set; }
        public Nullable<System.DateTime> BodyPoIssueDate { get; set; }
        public Nullable<decimal> BodyPrice { get; set; }
        public string BodySerial { get; set; }
        public Nullable<System.DateTime> ReqBodyShipDate { get; set; }
        public Nullable<System.DateTime> EstBodyShipDate { get; set; }
        public Nullable<System.DateTime> BodyShipDate { get; set; }
        public Nullable<System.DateTime> BodyReceivedDate { get; set; }
        public string BodyCompany { get; set; }
        public string LiftModel { get; set; }
        public string LiftOrder { get; set; }
        public Nullable<int> SalesOrder { get; set; }
        public Nullable<int> SalesOrderLine { get; set; }
        public Nullable<int> SalesOrderRelease { get; set; }
        public string LiftSerial { get; set; }
        public Nullable<System.DateTime> LiftATSDate { get; set; }
        public string InstallPO { get; set; }
        public Nullable<System.DateTime> InstallPODateIssued { get; set; }
        public Nullable<decimal> InstallPrice { get; set; }
        public Nullable<System.DateTime> PrelimInvoiceDate { get; set; }
        public string PCInvoiceNumber { get; set; }
        public Nullable<decimal> BaseLiftPrice { get; set; }
        public string Notes { get; set; }
        public Nullable<int> UpfitterId { get; set; }
        public Nullable<int> ShipToId { get; set; }
        public Nullable<int> ChassisId { get; set; }
        public Nullable<decimal> ChassisPrice { get; set; }
        public string InvoiceNumber { get; set; }
        public Nullable<decimal> LastInvoiceAmount { get; set; }
        public Nullable<decimal> FreightDelivery { get; set; }
        public Nullable<decimal> FreightLift { get; set; }
        public Nullable<decimal> FreightPlatform { get; set; }
        public Nullable<decimal> FreightBody { get; set; }
        public Nullable<decimal> InServiceAcct { get; set; }
        public Nullable<decimal> ExtWarrAcct { get; set; }
    
        public virtual Chassis Chassis { get; set; }
        public virtual ICollection<InvoiceHistory> InvoiceHistories { get; set; }
        public virtual ICollection<MiscInstall> MiscInstalls { get; set; }
        public virtual OrderHeader OrderHeader { get; set; }
        public virtual ShipTo ShipTo { get; set; }
        public virtual Upfitter Upfitter { get; set; }
        public virtual ICollection<OrderDetailsOrderOptionsLink> OrderDetailsOrderOptionsLinks { get; set; }
    }
}
