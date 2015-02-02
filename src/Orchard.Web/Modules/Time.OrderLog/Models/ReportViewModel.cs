using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.OrderLog.Models
{
    public class ReportViewModel
    {
        public string ReportName { get; set; }
        public int? DealerId { get; set; }
        public int? TerritoryId { get; set; }
        public int? RegionId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Special { get; set; }
        public bool Stock { get; set; }
        public bool Demo { get; set; }
        public bool RTG { get; set; }
    }
}