using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.ITInventory
{
    [MetadataType(typeof(Ref_NICMetadata))]
    public partial class Ref_NIC
    {
    }

    public class Ref_NICMetadata
    {
        [DisplayName("NIC Speed")]
        public Nullable<int> SpeedId { get; set; }

        [DisplayName("License Type")]
        public string Type { get; set; }

        [DisplayName("Switch Port")]
        public Nullable<int> SwitchPortId { get; set; }

        [DisplayName("Cable")]
        public Nullable<int> CableId { get; set; }

        [DisplayName("Computer")]
        public Nullable<int> ComputerId { get; set; }
    }
}