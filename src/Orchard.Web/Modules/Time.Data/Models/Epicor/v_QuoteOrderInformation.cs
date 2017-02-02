using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.Epicor
{
    [MetadataType(typeof(v_QuoteOrderInformationMetadata))]
    public partial class v_QuoteOrderInformation
    {
    }

    public class v_QuoteOrderInformationMetadata
    {
        public string Type { get; set; }

        [Display(Name = "PO Num")]
        public string ponum { get; set; }

        [Display(Name = "PartNum")]
        public string partnum { get; set; }

        [Display(Name = "LineDesc")]
        public string linedesc { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "DueDate")]
        public Nullable<System.DateTime> duedate { get; set; }

        [Display(Name = "EntryPage")]
        public Nullable<System.DateTime> entrydate { get; set; }

        [Display(Name = "ChangedBy")]
        public string changedby { get; set; }
        public Nullable<int> QuoteReference { get; set; }
        public Nullable<long> progress_recid { get; set; }
    }
}