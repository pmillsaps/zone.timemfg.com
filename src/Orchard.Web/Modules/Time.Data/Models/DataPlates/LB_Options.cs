using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.DataPlates
{
    [MetadataType(typeof(LB_OptionsMetadata))]
    public partial class LB_Options
    {
    }

    public class LB_OptionsMetadata
    {
        [Required(ErrorMessage = "Option is Required")]
        [RegularExpression(@"^[DL]+[B]+[-].*(?<![Ee][Nn][Gg])$", ErrorMessage = "The Option must start with DB- or LB- and not be an ENG")]
        public string Option { get; set; }
    }
}