using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.Data.Models.MessageQueue
{
    public class EmailMessage
    {
        public string From { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool HTML { get; set; }
        public bool Priority { get; set; }
    }
}