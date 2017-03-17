using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.Install
{
    [MetadataType(typeof(InstallHourlyRateMetadata))]
    public partial class InstallHourlyRate
    {
    }

    public class InstallHourlyRateMetadata
    {
        [Display(Name = "Rate Type")]
        [Required(ErrorMessage = "Rate Type is Required")]
        public string RateType { get; set; }

        [Display(Name = "Rate")]
        [Required(ErrorMessage = "Rate is Required")]
        public decimal Rate { get; set; }
    }
}