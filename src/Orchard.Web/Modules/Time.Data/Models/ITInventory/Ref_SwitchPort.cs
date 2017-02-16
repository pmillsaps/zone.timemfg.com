using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.ITInventory
{
    [MetadataType(typeof(Ref_SwitchPortMetadata))]
    public partial class Ref_SwitchPort
    {
    }

    public class Ref_SwitchPortMetadata
    {
        [DisplayName("Switch Port")]
        public string SwitchPort { get; set; }
    }
}