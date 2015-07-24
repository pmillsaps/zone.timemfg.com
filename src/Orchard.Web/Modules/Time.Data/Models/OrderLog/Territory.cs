using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.OrderLog
{
    [MetadataType(typeof(TerritoryMetadata))]
    public partial class Territory
    {
    }

    public class TerritoryMetadata
    {
        [DisplayName("Territory")]
        public string TerritoryName { get; set; }
    }
}