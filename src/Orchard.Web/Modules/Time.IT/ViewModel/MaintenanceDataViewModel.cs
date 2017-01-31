using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.IT.ViewModel
{
    public class MaintenanceDataViewModel
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string BudgetItem { get; set; }
        public string Supplier { get; set; }
        public string AccountNumber { get; set; }
        public string OriginalPurchDate { get; set; }
        public string ComputerName { get; set; }
        public string LicenseName { get; set; }
    }
}