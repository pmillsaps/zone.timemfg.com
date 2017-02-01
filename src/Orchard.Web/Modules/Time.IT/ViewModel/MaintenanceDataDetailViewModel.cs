using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.IT.ViewModel
{
    public class MaintenanceDataDetailViewModel
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string BudgetItem { get; set; }
        public string Supplier { get; set; }
        public string OriginalPurchDate { get; set; }
        public string PurchaseDate { get; set; }
        public string ExpirationDate { get; set; }
        public string AccountNumber { get; set; }
        public string Duration { get; set; }
        public string Cost { get; set; }
        public string Monthly { get; set; }
        public string Explanation { get; set; }
        public string AlternateInfo { get; set; }
    }
}