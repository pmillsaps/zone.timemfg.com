using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.Configurator.Models
{
    public class SpecialConfigViewModel
    {
        public int SpecialConfigId { get; set; }
        public int SpecialDataTypeId { get; set; }
        public string Part { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public int RelatedOpId { get; set; }
    }
}