using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.OrderLog.Models
{
    public class ReportViewModel
    {
        public int? DealerId { get; set; }
        public int? TerritoryId { get; set; }
        public int? RegionId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}