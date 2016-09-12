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
        [DisplayName("Phone")]
        public string PhoneNumber { get; set; }

        [DisplayName("Purch Date")]
        public DateTime PurchaseDate { get; set; }

        [DisplayName("Build Date")]
        public DateTime LastBuildDate { get; set; }

        [DisplayName("Edit Date")]
        public DateTime LastDateEdited { get; set; }
    }
}