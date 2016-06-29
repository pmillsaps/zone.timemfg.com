using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.Install
{
    [MetadataType(typeof(TimeOptionMetadata))]
    public partial class TimeOption
    {
    }

    public class TimeOptionMetadata
    {
        [Display(Name = "Lift Family Name")]
        [Required(ErrorMessage = "Lift Family Name is Required")]
        public int LiftFamilyId { get; set; }

        [Display(Name = "Time Option")]
        [Required(ErrorMessage = "Time Option is Required")]
        public string Option { get; set; }

        [Display(Name = "Labor Hours")]
        [Required(ErrorMessage = "Labor Hours are Required")]
        public decimal LaborHours { get; set; }

        [Display(Name = "Paint Rate?")]
        public bool PaintFlag { get; set; }
    }
}