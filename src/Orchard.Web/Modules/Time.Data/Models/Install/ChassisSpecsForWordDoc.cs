using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.Install
{
    [MetadataType(typeof(ChassisSpecsForWordDocMetadata))]
    public partial class ChassisSpecsForWordDoc
    {
    }

    public class ChassisSpecsForWordDocMetadata
    {
        [Display(Name = "Cab To Axle Dimension")]
        [Required(ErrorMessage = "This field is required.")]
        public string CbToAxDimension { get; set; }

        [Display(Name = "Frame Section Modulus")]
        [Required(ErrorMessage = "This field is required.")]
        public string FrmSctnModulus { get; set; }

        [Display(Name = "Frame Resisting Bending Moment")]
        [Required(ErrorMessage = "This field is required.")]
        public string FrmRsistngBndngMmnt { get; set; }

        [Display(Name = "Front GAWR")]
        [Required(ErrorMessage = "This field is required.")]
        public string FrontGAWR { get; set; }

        [Display(Name = "Rear GAWR")]
        [Required(ErrorMessage = "This field is required.")]
        public string RearGAWR { get; set; }

        [Display(Name = "GVWR")]
        [Required(ErrorMessage = "This field is required.")]
        public string GVWR { get; set; }

        [Display(Name = "Aprox. Curb Weight Stability")]
        [Required(ErrorMessage = "This field is required.")]
        public string AprxCrbWghtStblty { get; set; }
    }
}