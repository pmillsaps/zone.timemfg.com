using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.Install
{
    [MetadataType(typeof(InstallQuoteMetadata))]
    public partial class InstallQuote
    {
    }

    public class InstallQuoteMetadata
    {
        [Display(Name = "Lift Name")]
        public string LiftName { get; set; }

        [Display(Name = "Quote Number")]
        public int LiftQuoteNumber { get; set; }

        [Display(Name = "Quote Line")]
        public int LiftQuoteLine { get; set; }

        [Display(Name = "Install Line")]
        public int LiftInstallLine { get; set; }

        [Display(Name = "Quoted By")]
        public string InstallQuotedBy { get; set; }

        [Display(Name = "Quote Date")]
        public System.DateTime QuoteDate { get; set; }

        [Display(Name = "Total Price Labor")]
        public decimal TotalPriceLabor { get; set; }

        [Display(Name = "Total Price Material")]
        public decimal TotalPriceMaterial { get; set; }

        [Display(Name = "Total Install Price")]
        public Nullable<decimal> TotalInstallPrice { get; set; }

        [Display(Name = "Total Install Hours")]
        public decimal TotalInstallHours { get; set; }

        [Display(Name = "Total Paint Hours")]
        public decimal TotalPaintHours { get; set; }
    }
}