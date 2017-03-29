using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [RegularExpression(@"^SC-.*(?<![Ee][Nn][Gg]|[\s]|[\.])$", ErrorMessage = "The Option must start with SC- and not end with ENG, a dot, or a blank space.")]
        [Required(ErrorMessage = "Option is Required")]
        public string Option { get; set; }
        [DisplayName("TruGuard/HRUpperControls")]
        public bool HRUpperControls { get; set; }
    }
}