using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.DataPlates
{
    [MetadataType(typeof(CA_OptionsMetadata))]
    public partial class CA_Options
    {
    }

    public class CA_OptionsMetadata
    {
        [Required(ErrorMessage = "Option is Required")]
        [RegularExpression(@"^CA-.*$", ErrorMessage = "The Option must start with CA-")]
        public string Option { get; set; }
        [Required(ErrorMessage = "Enter Capacity in Pounds")]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "Enter a number for Capacity")]
        public string Cap_LBS { get; set; }
        [Required(ErrorMessage = "Enter Capacity in Kilograms")]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "Enter a number for Capacity")]
        public string Cap_KG { get; set; }
    }
}