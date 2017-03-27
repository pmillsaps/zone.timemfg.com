using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.Install
{
    [MetadataType(typeof(InstallDetailsForWordDocMetadata))]
    public partial class InstallDetailsForWordDoc
    {
    }

    public class InstallDetailsForWordDocMetadata
    {
        [Required]
        [Display(Name = "Detail Line")]
        public string DetailLine { get; set; }
    }
}