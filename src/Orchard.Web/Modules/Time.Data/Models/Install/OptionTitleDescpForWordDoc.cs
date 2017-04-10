using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.Install
{
    [MetadataType(typeof(OptionTitleDescpForWordDocMetadata))]
    public partial class OptionTitleDescpForWordDoc
    {

    }

    public class OptionTitleDescpForWordDocMetadata
    {
        [Display(Name = "Lift Family Name")]
        [Required(ErrorMessage = "This field is required.")]
        public int LiftFamilyId { get; set; }

        [Display(Name = "Option Name")]
        [Required(ErrorMessage = "This field is required.")]
        public int OptionTitlesId { get; set; }
    }
}