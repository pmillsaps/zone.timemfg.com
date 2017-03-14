using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.ITInventory
{
    [MetadataType(typeof(Ref_DeviceTypeMetadata))]
    public partial class Ref_DeviceType
    {
    }

    public class Ref_DeviceTypeMetadata
    {
        [DisplayName("Device Type")]
        public string DeviceType { get; set; }
    }
}