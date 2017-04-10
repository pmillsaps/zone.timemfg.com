using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Time.IT.ViewModel
{
    public class UploadAttachmentViewModel
    {
        public int ComputerId { get; set; }
        public int ModelId { get; set; }
        public string ComputerModel { get; set; }
        public string FileName { get; set; }

        [Required]
        public string Description { get; set; }

        [Display(Name = "For Model or Computer?")]
        public string ModelOrComputer { get; set; }
    }
}