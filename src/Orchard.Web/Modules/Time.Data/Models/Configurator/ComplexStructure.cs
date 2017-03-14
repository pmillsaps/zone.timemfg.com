using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.Configurator
{
    [MetadataType(typeof(ComplexStructureMetadata))]
    public partial class ComplexStructure
    {
    }

    public class ComplexStructureMetadata
    {
        [Display(Name = "Description")]
        public string Notes { get; set; }
    }
}