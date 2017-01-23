using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.DataPlates
{
    [MetadataType(typeof(LiftDataMetadata))]
    public partial class LiftData
    {
    }

    public class LiftDataMetadata
    {
        [Required(ErrorMessage = "Lift is Required")]
        public string Lift { get; set; }
    }
}