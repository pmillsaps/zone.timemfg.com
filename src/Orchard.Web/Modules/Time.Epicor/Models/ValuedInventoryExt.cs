using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.Epicor.Models
{
    public class ValuedInventoryExt
    {
        public int Id { get; set; }
        public string PartNum { get; set; }
        public string Description { get; set; }
        public decimal BOH { get; set; }
        public System.DateTime CalculationDate { get; set; }
        public System.DateTime TransactionDate { get; set; }
        public decimal TransactionCost { get; set; }
        public decimal CurrentCost { get; set; }
        public System.DateTime ComparisonDate { get; set; }
        public int PeriodYear { get; set; }
        public string ClassId { get; set; }

        public decimal ExtCost
        {
            get { return this.BOH * this.CurrentCost; }
        }
    }
}