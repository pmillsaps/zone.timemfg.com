using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.ITInventory
{
    [MetadataType(typeof(Ref_LocationMetadata))]
    public partial class Ref_Location
    {
    }

    public class Ref_LocationMetadata
    {
        [DisplayName("Location")]
        public string Name { get; set; }
    }
}