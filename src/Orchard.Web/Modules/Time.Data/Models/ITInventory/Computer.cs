using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.ITInventory
{
    [MetadataType(typeof(ComputerMetadata))]
    public partial class Computer
    {
    }

    public class ComputerMetadata
    {
        [Required]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Note { get; set; }
        [DataType(DataType.MultilineText)]
        public string AdditionalHW { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
    }
}