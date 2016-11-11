using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.Legacy;

namespace Time.Legacy.Models
{
    public class InsertWarranty
    {
        //public WarrantyInformation Information { get; set; }
        //public WarrantyInvoice Invoice { get; set; }

        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public string EndUserName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string LiftOrderNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string PoNumber { get; set; }
        public string Comments { get; set; }
    }
}