using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.ITInventory
{
    [MetadataType(typeof(ScheduledTaskMetadata))]
    public partial class ScheduledTask
    {
    }

    public class ScheduledTaskMetadata
    {
        [Required]
        [DisplayName("Task Name")]
        public string Name { get; set; }
        [DisplayName("Days Of Month")]
        public string DOM { get; set; }
        [DisplayName("Su")]
        public string Sunday { get; set; }
        [DisplayName("M")]
        public string Monday { get; set; }
        [DisplayName("Tu")]
        public string Tuesday { get; set; }
        [DisplayName("W")]
        public string Wednesday { get; set; }
        [DisplayName("Th")]
        public string Thursday { get; set; }
        [DisplayName("F")]
        public string Friday { get; set; }
        [DisplayName("Sa")]
        public string Saturday { get; set; }
        [DisplayName("Security Run As")]
        public string SecurityRunAs { get; set; }
        [DisplayName("Shares and Locations")]
        public string SharesLocations { get; set; }
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
    }
}