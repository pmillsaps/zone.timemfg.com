using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Install.ViewModels
{
    public class OptionTitlesAndDescrVM
    {
        public int Id { get; set; }
        public int OptionTitlesId { get; set; }

        [Display(Name = "Lift Family Name")]
        public int LiftFamilyId { get; set; }

        public string FamilyName { get; set; }

        [Display(Name = "Option Name")]
        [Required(ErrorMessage = "This field is required.")]
        public string OptionName { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public string Description { get; set; }
    }
}