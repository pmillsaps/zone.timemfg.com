using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.ITInventory
{
    [MetadataType(typeof(Ref_CableNoMetadata))]
    public partial class Ref_CableNo
    {
    }

    public class Ref_CableNoMetadata
    {
        [DisplayName("Cable")]
        public string Name { get; set; }
    }
}