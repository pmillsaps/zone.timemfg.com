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
        public string CbToAxDimension { get; set; }

        [Display(Name = "Frame Section Modulus")]
        public string FrmSctnModulus { get; set; }

        [Display(Name = "Frame Resisting Bending Moment")]
        public string FrmRsistngBndngMmnt { get; set; }

        [Display(Name = "Front GAWR")]
        public string FrontGAWR { get; set; }

        [Display(Name = "Rear GAWR")]
        public string RearGAWR { get; set; }

        [Display(Name = "GVWR")]
        public string GVWR { get; set; }

        [Display(Name = "Aprox. Curb Weight Stability")]
        public string AprxCrbWghtStblty { get; set; }
    }
}