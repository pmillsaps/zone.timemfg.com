using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.OrderLog.EntityModels
{
    [MetadataType(typeof(LiftModelMetadata))]
    public partial class LiftModel
    {
    }

    public class LiftModelMetadata
    {
        [DisplayName("Lift Model")]
        public string LiftModelName { get; set; }
    }
}