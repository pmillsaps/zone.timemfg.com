using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Time.Drawings.EntityModels;

namespace Time.Drawings.ViewModels
{
    public class AdditionalDrawingViewModel
    {
        public int? SourceId { get; set; }

        [Display(Name = "Additional Drawing")]
        [Required(ErrorMessage = "Additional Drawing is Required")]
        public int TargetId { get; set; }

        public Drawings_PDF pdf { get; set; }
    }
}