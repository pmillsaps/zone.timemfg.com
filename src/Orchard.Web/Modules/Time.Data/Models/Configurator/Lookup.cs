using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.Configurator
{
    [MetadataType(typeof(LookupMetadata))]
    public partial class Lookup
    {
    }

    public class LookupMetadata
    {
        [Required(ErrorMessage = "ConfigName is required")]
        public string ConfigName { get; set; }

        [Required(ErrorMessage = "ConfigData is required")]
        public string ConfigData { get; set; }

        [Required(ErrorMessage = "Sequence is required")]
        public int Sequence { get; set; }

        [Required(ErrorMessage = "Data is required")]
        public string Data { get; set; }
    }
}