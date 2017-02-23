using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.OrderLog.Models
{
    public class MenuViewModel
    {
        public bool ViewOrders { get; set; }
        public bool EditOrders { get; set; }
        public bool OrderLogReporting { get; set; }
    }
}