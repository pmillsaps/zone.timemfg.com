using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.Configurator
{
    [MetadataType(typeof(StructureMetadata))]
    public partial class Structure
    {
    }

    public class StructureMetadata
    {
        [Required(ErrorMessage = "ConfigName is required")]
        public string ConfigName { get; set; }

        [Required(ErrorMessage = "ConfigData is required")]
        public string ConfigData { get; set; }
    }
}