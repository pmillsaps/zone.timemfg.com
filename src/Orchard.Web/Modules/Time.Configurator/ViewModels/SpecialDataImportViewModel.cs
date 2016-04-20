using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Configurator.ViewModels
{
    public class SpecialDataImportViewModel
    {
        [Required(ErrorMessage = "Select a Special Config Name")]
        public int SpecialConfigId { get; set; }
        [Required(ErrorMessage = "Import Data Required")]
        public string ImportData { get; set; }
    }
}