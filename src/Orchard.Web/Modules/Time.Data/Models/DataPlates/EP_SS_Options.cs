using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.DataPlates
{
    [MetadataType(typeof(EP_SS_OptionsMetadata))]
    public partial class EP_SS_Options
    {
    }

    public class EP_SS_OptionsMetadata
    {
        [Required(ErrorMessage = "Option is Required")]
        [RegularExpression(@"^[ES]+[PS]+[-].*", ErrorMessage = "The Option must start with EP- or SS-")]
        public string Option { get; set; }
        [Required(ErrorMessage = "Enter Voltage")]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "Enter a number for Voltage")]
        public string Voltage { get; set; }
    }
}