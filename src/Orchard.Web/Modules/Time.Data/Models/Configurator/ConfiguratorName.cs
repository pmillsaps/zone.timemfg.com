using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.Configurator
{
    [MetadataType(typeof (ConfiguratorNameMetadata))]
    public partial class ConfiguratorName
    {
    }

    public class ConfiguratorNameMetadata
    {
        [Required(ErrorMessage = "Enter the Configurator Name")]
        public string ConfigName { get; set; }
    }
}