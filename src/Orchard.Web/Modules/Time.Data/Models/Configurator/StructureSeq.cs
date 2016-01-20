using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.Configurator
{
    [MetadataType(typeof(StructureSeqMetadata))]
    public partial class StructureSeq
    {
    }

    public class StructureSeqMetadata
    {
        [Required(ErrorMessage = "ConfigName is required")]
        public string ConfigName { get; set; }

        [Required(ErrorMessage = "ConfigData is required")]
        public string ConfigData { get; set; }

        [Required(ErrorMessage = "Sequence is required")]
        public int Sequence { get; set; }

        //[Required(ErrorMessage = "Lookup is required")]
        //public string Lookup { get; set; }

        //[Required(ErrorMessage = "LookupSequence is required")]
        //public Nullable<int> LookupSequence { get; set; }

        [Display(Name = "Description")]
        //[Required(ErrorMessage = "Description is required")]
        public string Notes { get; set; }
    }
}