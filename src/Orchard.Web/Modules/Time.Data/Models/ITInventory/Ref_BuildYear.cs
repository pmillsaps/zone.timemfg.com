using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.ITInventory
{
    [MetadataType(typeof(Ref_BuildYearMetadata))]
    public partial class Ref_BuildYear
    {
    }

    public class Ref_BuildYearMetadata
    {
        [DisplayName("Build Year")]
        public string Name {get; set;}
    }
}