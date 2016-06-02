using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.Install
{
    [MetadataType(typeof(LiftFamilyMetadata))]
    public partial class LiftFamily
    {
    }

    public class LiftFamilyMetadata
    {
        [Display(Name = "Lift Family Name")]
        [Required(ErrorMessage = "Lift Family Name is Required")]
        public string FamilyName { get; set; }

        [Display(Name = "Base Labor Hours")]
        [Required(ErrorMessage = "Hours are Required")]
        public decimal LaborHours { get; set; }
    }
}