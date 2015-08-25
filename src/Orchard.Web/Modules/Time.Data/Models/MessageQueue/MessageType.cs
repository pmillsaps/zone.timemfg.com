using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.Data.Models.MessageQueue
{
    public class MessageType
    {
        private MessageType(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public static MessageType BuildComplexLinks { get { return new MessageType("BuildComplexLinks"); } }
        public static MessageType BuildComplexLinksFull { get { return new MessageType("BuildComplexLinksFull"); } }
        public static MessageType BuildComplexLookups { get { return new MessageType("BuildComplexLookups"); } }
        public static MessageType BuildConfigOptions { get { return new MessageType("BuildConfigOptions"); } }
        public static MessageType CheckWaterReports { get { return new MessageType("CheckWaterReports"); } }
        public static MessageType CustomManualCheckProblemJobs { get { return new MessageType("CustomManualCheckProblemJobs"); } }
        public static MessageType CustomManualJobQueue { get { return new MessageType("CustomManualJobQueue"); } }
        public static MessageType EmailMessage { get { return new MessageType("EmailMessage"); } }
        public static MessageType EmailProblemJobs { get { return new MessageType("EmailProblemJobs"); } }
        public static MessageType MoveFile { get { return new MessageType("MoveFile"); } }
        public static MessageType Refresh_Drawings_DB { get { return new MessageType("Refresh_Drawings_DB"); } }
        public static MessageType SendRTIMessage { get { return new MessageType("SendRTIMessage"); } }
        public static MessageType TicketsNotApprovedTimer { get { return new MessageType("TicketsNotApprovedTimer"); } }
        public static MessageType WOL { get { return new MessageType("WOL"); } }
    }
}