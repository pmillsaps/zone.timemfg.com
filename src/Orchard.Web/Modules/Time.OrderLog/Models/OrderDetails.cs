using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.OrderLog.Models
{
    public class OrderDetails
    {
        public string PONum { get; set; }
        public string OrderDate { get; set; }
        public string LiftModel { get; set; }
        public string DealerName { get; set; }
        public string InstallType { get; set; }
        public string InstallerName { get; set; }
        public int OrderQty { get; set; }
        public int? Price { get; set; }
        public int? ExtPrice { get; set; }
        public bool Special { get; set; }
        public bool Stock { get; set; }
        public bool Demo { get; set; }
        public bool RTG { get; set; }
        public bool TruGuard { get; set; }
        public string Customer { get; set; }
        public string CityStateZip { get; set; }
        public List<OrderTransactions> OrderT { get; set; }
    }
}