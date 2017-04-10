using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.Install
{
    [MetadataType(typeof(QuoteDeptUsersForWordDocMetadata))]
    public partial class QuoteDeptUsersForWordDoc
    {
    }

    public class QuoteDeptUsersForWordDocMetadata
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Position Title")]
        public string PositionTitle { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Email { get; set; }
    }
}