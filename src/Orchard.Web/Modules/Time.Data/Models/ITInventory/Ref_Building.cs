using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.ITInventory
{
    [MetadataType(typeof(Ref_BuildingMetadata))]
    public partial class Ref_Building
    {
    }

    public class Ref_BuildingMetadata
    {
        [DisplayName("Building")]
        public string Name { get; set; }
    }
}