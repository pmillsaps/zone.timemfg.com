using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.Install
{
    [MetadataType(typeof(VSWOptionMetadata))]
    public partial class VSWOption
    {
    }

    public class VSWOptionMetadata
    {
        [Display(Name = "Lift Family Name")]
        [Required(ErrorMessage = "Lift Family is Required")]
        public int LiftFamilyId { get; set; }

        [Display(Name = "Group Name")]
        [Required(ErrorMessage = "Group Name is Required")]
        public int GroupId { get; set; }

        [Display(Name = "Option Name")]
        [Required(ErrorMessage = "Option Name is Required")]
        public string OptionName { get; set; }

        //[Required(ErrorMessage = "Quantity is Required")]
        //public int Quantity { get; set; }

        [Required(ErrorMessage = "Price is Required")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Price { get; set; }

        [Display(Name = "Install Hours")]
        [Required(ErrorMessage = "Hours are Required")]
        public decimal InstallHours { get; set; }

        [Display(Name = "Paint Rate?")]
        public bool PaintFlag { get; set; }
    }
}