using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.Install.ViewModels
{
    public class LoadAerialQuotes
    {
        public int QuoteNum { get; set; }
        public int QuoteLine { get; set; }
        public string PartNum { get; set; }
        public string LineDesc { get; set; }
        public string LastUpdate { get; set; }
        public string LastDcdUserId { get; set; }
        public decimal OrderQty { get; set; }
        public string ChangedBy { get; set; }
        public string ChangeDate { get; set; }
    }
}