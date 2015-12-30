using System;
using System.Collections.Generic;
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
        [RegularExpression(@"^PS-.*$", ErrorMessage = "The Option must start with PS-")]
        public string Option { get; set; }
        [Required(ErrorMessage = "Enter Platform quantity")]
        [RegularExpression("([0-9])", ErrorMessage = "Enter a number for quantity (single digit)")]  
        public short PlatformQty { get; set; }
    }
}