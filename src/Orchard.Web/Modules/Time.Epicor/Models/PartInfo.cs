using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.Epicor.Models
{
    public class PartInfo
    {
        public string PartNumber { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public string Draw { get; set; }

        public decimal? OnHand { get; set; }

        public decimal? Price { get; set; }

        public string VendorID { get; set; }

        public bool InActive { get; set; }

        public bool isPhantom { get; set; }

        public string VendorName { get; set; }

        public string VendorPartNumber { get; set; }

        public decimal? Cost { get; set; }

        public string Revision { get; set; }

        public string ClassID { get; set; }

        public string PartLocation { get; set; }

        public string BuyerID { get; set; }

        public string BuyerName { get; set; }

        public string SubPart { get; set; }

        public string Eco { get; set; }
    }
}