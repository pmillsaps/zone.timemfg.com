using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.ITInventory
{
    [MetadataType(typeof(Ref_VideoCardMetadata))]
    public partial class Ref_VideoCard
    {
    }

    public class Ref_VideoCardMetadata
    {
        [DisplayName("Video Card")]
        public string VideoCard { get; set; }
    }
}