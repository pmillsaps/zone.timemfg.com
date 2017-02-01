using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.Production;

namespace Time.Epicor.ViewModels
{
    public class QuoteOrderSearchVM
    {
        public string Search { get; set; }
        public bool Export { get; set; }
        public List<V_QuoteOrderInformation> Details { get; set; }
    }
}