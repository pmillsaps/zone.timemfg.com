using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Time.Drawings.EntityModels;

namespace Time.Drawings.ViewModels
{
    public class SearchDrawingsViewModel
    {
        [Required]
        [StringLength(40, MinimumLength = 3, ErrorMessage = "Must be at least 3 digits.")]
        public string Search { get; set; }

        public List<Drawings_PDF> DrawingsList { get; set; }
    }
}