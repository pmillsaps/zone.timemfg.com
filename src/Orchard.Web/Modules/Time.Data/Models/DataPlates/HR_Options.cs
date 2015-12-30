using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.DataPlates
{
    [MetadataType(typeof(HR_OptionsMetadata))]
    public partial class HR_Options
    {
    }

    public class HR_OptionsMetadata
    {
        [RegularExpression(@"^SC-.*", ErrorMessage = "The Option must start with SC-")]
        [Required(ErrorMessage = "Option is Required")]
        public string Option { get; set; }
    }
}