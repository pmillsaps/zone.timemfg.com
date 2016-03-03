using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.Data.Models.MessageQueue
{
    public class PCInvoiceMessage
    {
        public int TaskQueueId { get; set; }
        public int OrderDetailId { get; set; }
        public string Status { get; set; }
    }
}