using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.DataPlates
{
    [MetadataType(typeof(PS_OptionMetadata))]
    public partial class PS_Option
    {
    }

    public class PS_OptionMetadata
    {
        [Required(ErrorMessage = "Option is Required")]
        [RegularExpression(@"^PS-.*(?<![Ee][Nn][Gg])$", ErrorMessage = "The Option must start with PS- and not be an ENG")]
        public string Option { get; set; }
        [Required(ErrorMessage = "Enter Platform quantity")]
        [RegularExpression("([0-2])", ErrorMessage = "Enter a number for quantity (0, 1, or 2)")]  
        public short PlatformQty { get; set; }
        [DisplayName("TruGuard/HRUpperControls")]
        public bool HRUpperControls { get; set; }
    }
}