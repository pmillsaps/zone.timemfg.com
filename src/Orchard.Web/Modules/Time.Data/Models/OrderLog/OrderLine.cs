using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.OrderLog
{
    [MetadataType(typeof(OrderLineMetadata))]
    public partial class OrderLine
    {
    }

    public class OrderLineMetadata
    {
        [DisplayName("New Qty")]
        public string NewQty { get; set; }

        [DisplayName("Cancel Qty")]
        public string CancelQty { get; set; }
    }
}