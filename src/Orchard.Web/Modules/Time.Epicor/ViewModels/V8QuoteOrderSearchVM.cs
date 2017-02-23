using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.Epicor;

namespace Time.Epicor.ViewModels
{
    public class V8QuoteOrderSearchVM
    {
        public string Search { get; set; }
        public bool Export { get; set; }
        public List<v_QuoteOrderInformation> Details { get; set; }
    }
}