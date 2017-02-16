using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.Install
{
    [MetadataType(typeof(OptionGroupMetadata))]
    public partial class OptionGroup
    {
    }

    public class OptionGroupMetadata
    {
        [Display(Name = "Group Name")]
        [Required(ErrorMessage = "Group Name is Required")]
        public string GroupName { get; set; }
    }
}