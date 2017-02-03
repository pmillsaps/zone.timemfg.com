using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.Data.Models.MessageQueue
{
    public class MessageView
    {
        public String Id { get; set; }

        public DateTime ArrivedTime { get; set; }

        //public System.Messaging.MessageType Type { get; set; }
        public string Label { get; set; }
    }
}