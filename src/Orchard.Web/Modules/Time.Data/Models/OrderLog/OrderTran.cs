using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.OrderLog
{
    [MetadataType(typeof(OrderTranMetadata))]
    public partial class OrderTran
    {
    }

    public class OrderTranMetadata
    {
        [DisplayName("New Qty")]
        public string NewQty { get; set; }

        [DisplayName("Cancel Qty")]
        public string CancelQty { get; set; }

        [DisplayName("As Of Date")]
        public DateTime AsOfDate { get; set; }
    }
}