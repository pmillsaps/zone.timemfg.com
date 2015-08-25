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
        [Required(ErrorMessage = "Data is required")]
        public string Data { get; set; }
    }
}