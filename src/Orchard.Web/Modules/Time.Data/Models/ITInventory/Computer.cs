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
        [DisplayName("Computer Name")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Note { get; set; }

        [DataType(DataType.MultilineText)]
        [DisplayName("Additional HW")]
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

        [DisplayName("Status")]
        public int StatusId { get; set; }

        [DisplayName("Model")]
        public int ModelId { get; set; }

        [DisplayName("Windows Key")]
        public string WindowsKey { get; set; }

        [DisplayName("Memory")]
        public Nullable<int> MemoryId { get; set; }

        [DisplayName("Processor")]
        public Nullable<int> ProcessorId { get; set; }

        [DisplayName("Device Type")]
        public Nullable<int> DeviceTypeId { get; set; }

        [DisplayName("OS")]
        public Nullable<int> OSId { get; set; }

        [DisplayName("Video Card")]
        public Nullable<int> VideoCardId { get; set; }

        [DisplayName("Sound")]
        public Nullable<int> SoundId { get; set; }

        [DisplayName("Last Edited By")]
        public string LastEditedBy { get; set; }

        [DisplayName("WTY Expiration Date")]
        public Nullable<System.DateTime> WarrantyExpirationDate { get; set; }

        [DisplayName("Serial Number")]
        public string SerialNumber { get; set; }
    }
}