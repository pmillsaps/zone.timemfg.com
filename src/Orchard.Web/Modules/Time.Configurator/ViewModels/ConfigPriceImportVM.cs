using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Configurator.ViewModels
{
    public class ConfigPriceImportVM
    {
         [Required(ErrorMessage = "Select a ConfigId")]
        public string ConfigId { get; set; }
         [Required(ErrorMessage = "Option and Price Required")]
        public string OptionAndPrice { get; set; }
    }
}