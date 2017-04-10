using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.ITInventory
{
    [MetadataType(typeof(Ref_NICSpeedMetadata))]
    public partial class Ref_NICSpeed
    {
    }

    public class Ref_NICSpeedMetadata
    {
        [DisplayName("NIC Speed")]
        public string NIC_Speed { get; set; }
    }
}