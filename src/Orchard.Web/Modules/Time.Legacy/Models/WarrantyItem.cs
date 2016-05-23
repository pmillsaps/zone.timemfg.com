using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.Legacy.Models
{
    public class WarrantyItem
    {
        public string SerialNumber { get; set; }
        public string EndUserName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string LiftOrderNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string Comments { get; set; }

        // Initializes a new instance of the WarrantyItem class
        public WarrantyItem()
        {
        }
    }
}