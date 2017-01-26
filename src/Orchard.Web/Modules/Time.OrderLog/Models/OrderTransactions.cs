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
        public string AsOfDate { get; set; }
        public string LiftModel { get; set; }
        public int NewQty { get; set; }
        public int CancelQty { get; set; }
        public int UnitPrice { get; set; }
        public int ExtPrice { get; set; }
        public bool Special { get; set; }
        public bool Stock { get; set; }
        public bool Demo { get; set; }
        public bool RTG { get; set; }
        public bool TruGuard { get; set; }
        public string Comment { get; set; }
    }
}