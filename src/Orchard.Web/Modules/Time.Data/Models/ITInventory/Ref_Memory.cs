using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.ITInventory
{
    [MetadataType(typeof(Ref_MemoryMetadata))]
    public partial class Ref_Memory
    {
    }

    public class Ref_MemoryMetadata
    {
        [DisplayName("Memory")]
        public string Name { get; set; }
    }
}