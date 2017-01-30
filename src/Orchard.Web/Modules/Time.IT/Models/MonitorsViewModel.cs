using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.IT.Models
{
    public class MonitorsViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string MFRName { get; set; }
        public string Model { get; set; }
        public string SerialNo { get; set; }
        public string AssetId { get; set; }
        public string Size { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string PurchasedFrom { get; set; }
        public string PO { get; set; }
        public decimal? Cost { get; set; }
        public string Notes { get; set; }
    }
}