using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.Configurator.Models
{
    public class ConfigPricingViewModel
    {
        public string option { get; set; }
        public decimal price { get; set; }
        public decimal altPrice { get; set; }
    }
}