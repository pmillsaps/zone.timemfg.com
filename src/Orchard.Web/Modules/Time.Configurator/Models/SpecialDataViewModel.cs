using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.Configurator.Models
{
    public class SpecialDataViewModel
    {
        public string part { get; set; }
        public decimal quantity { get; set; }
        public decimal price { get; set; }
        public int specialDataTypeId { get; set; }
        public int? relatedOpId { get; set; }
    }
}