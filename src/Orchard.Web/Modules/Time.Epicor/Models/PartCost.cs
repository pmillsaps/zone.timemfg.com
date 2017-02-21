using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.Epicor.Models
{
    public class PartCost
    {
        public string PartNum { get; set; }
        public decimal CostinglotSize { get; set; }
        public decimal EstSetHrs { get; set; }
        public decimal ProdStandard { get; set; }
        public decimal EstProdHrs { get; set; }
        public decimal ProdBurRate { get; set; }
        public decimal ProdLbrRate { get; set; }
        public decimal SetupBurRate { get; set; }
        public decimal SetupLbrRate { get; set; }
        public decimal LaborCost { get; set; }
        public decimal BurdenCost { get; set; }
        public bool SubContract { get; set; }
        public decimal EstUnitCost { get; set; }
    }
}