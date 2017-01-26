using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.IT.Models
{
    public class LicensesViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string LicenseKey { get; set; }
        public int QuantityAssigned { get; set; }
        public string ComputerId { get; set; }// This field is in the DB but is not used
        public string CompOrUserName { get; set; }// Added this field to store Computrer or User names
        public string LicenseType { get; set; }
        public string PO { get; set; }
        public string PurchaseDate { get; set; }
        public string Note { get; set; }
    }
}