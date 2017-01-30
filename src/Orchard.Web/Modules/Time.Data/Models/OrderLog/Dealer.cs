using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.OrderLog
{
    [MetadataType(typeof(DealerMetadata))]
    public partial class Dealer
    {
    }

    public class DealerMetadata
    {
        [DisplayName("Dealer")]
        [Required]
        public string DealerName { get; set; }
        [DisplayName("Territory")]
        public int TerritoryId { get; set; }
    }
}