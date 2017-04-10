using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.Configurator
{
    [MetadataType(typeof(ConfigPricingMetadata))]
    public partial class ConfigPricing 
    {
    }

    public class ConfigPricingMetadata
    {
        [Required(ErrorMessage = "Select a ConfigId")]
        public string ConfigID { get; set; }

        [Required(ErrorMessage = "ConfigOption is required")]
        public string ConfigOption { get; set; }

        [Required(ErrorMessage = "Price is required")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Alternate Price is required. Enter same value as Price if is unknown")]
        public decimal AltPrice { get; set; }
    }
}