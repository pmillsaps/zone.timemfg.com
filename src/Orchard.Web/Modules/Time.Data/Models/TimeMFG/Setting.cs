using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.TimeMFG
{
    [MetadataType(typeof(SettingMetadata))]
    public partial class Setting
    {
    }

    public class SettingMetadata
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Value { get; set; }
        [DisplayName("DateType: String")]
        public string String { get; set; }
        [DisplayName("DateType: Integer")]
        public string Int { get; set; }
    }
}