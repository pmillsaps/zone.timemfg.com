using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.Configurator
{
    [MetadataType(typeof(PartOverrideMetadata))]
    public partial class PartOverride
    {
    }

    public class PartOverrideMetadata
    {
        [Required(ErrorMessage = "PartNum is required")]
        public string PartNum { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
    }
}