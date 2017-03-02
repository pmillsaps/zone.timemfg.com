using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.TimeMFG;
using Time.Epicor.Helpers;

namespace Time.Epicor.ViewModels
{
    public class ValuedInventoryVM
    {
        public DateTime ComparisonDate { get; set; }
        public SelectList ComparisonDates { get; set; }
        public List<ValuedInventory> ValuedInventoryItems { get; set; }
    }
}