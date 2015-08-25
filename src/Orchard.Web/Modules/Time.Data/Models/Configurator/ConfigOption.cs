using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.Configurator
{
    [MetadataType(typeof(ConfigOptionMetadata))]
    public partial class ConfigOption
    {
    }

    public class ConfigOptionMetadata
    {
        [Required(ErrorMessage = "ConfigName is required")]
        public string ConfigName { get; set; }

        [Required(ErrorMessage = "ConfigData is required")]
        public string ConfigData { get; set; }

        [Required(ErrorMessage = "Key01 is required")]
        public string Key01 { get; set; }

        [Required(ErrorMessage = "PartNum is required")]
        public string PartNum { get; set; }
    }
}