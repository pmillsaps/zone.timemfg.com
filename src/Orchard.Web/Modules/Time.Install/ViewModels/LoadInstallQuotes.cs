using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.Install.ViewModels
{
    public class LoadInstallQuotes
    {
        public int Id { get; set; }
        public string LiftName { get; set; }
        public int LiftQuoteNumber { get; set; }
        public int LiftQuoteLine { get; set; }
        public int LiftInstallLine { get; set; }
        public string InstallQuotedBy { get; set; }
        public string QuoteDate { get; set; }
        public decimal TotalPriceLabor { get; set; }
        public decimal TotalPriceMaterial { get; set; }
        public decimal TotalInstallPrice { get; set; }
        public decimal TotalInstallHours { get; set; }
        public decimal TotalPaintHours { get; set; }
        public int LiftFamilyId { get; set; }
    }
}