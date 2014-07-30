using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.OrderLog.EntityModels
{
    [MetadataType(typeof(OrderLineMetadata))]
    public partial class OrderLine
    {
    }

    public class OrderLineMetadata
    {
        [DisplayName("Qty")]
        public string NewQty { get; set; }

        [DisplayName("Cancel")]
        public string CancelQty { get; set; }
    }
}