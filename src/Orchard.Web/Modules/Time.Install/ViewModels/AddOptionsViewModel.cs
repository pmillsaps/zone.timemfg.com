using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Install.ViewModels
{
    // This model aids with adding VSW options to the db 
    public class AddOptionsViewModel
    {
        [Display(Name = "Lift Family Name")]
        [Required(ErrorMessage = "Group Name is required")]
        public int LiftFamilyId { get; set; }
        
        [Display(Name = "Group Name")]
        [Required(ErrorMessage = "Lift Family Name is required")]
        public int GroupId { get; set; }

        [Display(Name = "Options to Import")]
        [Required(ErrorMessage = "Options are required")]
        public string OptionsToParse { get; set; }
    }
}