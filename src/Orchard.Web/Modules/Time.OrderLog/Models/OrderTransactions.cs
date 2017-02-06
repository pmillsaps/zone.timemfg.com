using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.OrderLog.Models
{
    public class OrderTransactions
    {
        public string PO { get; set; }
        public string Date { get; set; }

        //public string AsOfDate { get; set; }
        public string DealerName { get; set; }
        public string LiftModel { get; set; }
        public string InstallType { get; set; }
        public int NewQty { get; set; }
        public int CancelQty { get; set; }
        public int UnitPrice { get; set; }
        public int ExtPrice { get; set; }
        public string Special { get; set; }
        public string Stock { get; set; }
        public string Demo { get; set; }
        public string RTG { get; set; }
        public string TruGuard { get; set; }
        public string Customer { get; set; }
        public string Comment { get; set; }
    }
}