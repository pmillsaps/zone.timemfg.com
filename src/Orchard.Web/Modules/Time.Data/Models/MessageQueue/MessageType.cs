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
        public static MessageType BuildDLS { get { return new MessageType("BuildDLS"); } }
        public static MessageType BuildValuedInventory { get { return new MessageType("BuildValuedInventory"); } }
        public static MessageType CheckWaterReports { get { return new MessageType("CheckWaterReports"); } }
        public static MessageType CustomManualCheckProblemJobs { get { return new MessageType("CustomManualCheckProblemJobs"); } }
        public static MessageType CustomManualJobQueue { get { return new MessageType("CustomManualJobQueue"); } }
        public static MessageType E10EmailSOAckPdf { get { return new MessageType("E10EmailSOAckPdf"); } }
        public static MessageType E10EmailInvoices { get { return new MessageType("E10EmailInvoices"); } }
        public static MessageType EmailMessage { get { return new MessageType("EmailMessage"); } }
        public static MessageType EmailProblemJobs { get { return new MessageType("EmailProblemJobs"); } }
        public static MessageType MoveFile { get { return new MessageType("MoveFile"); } }
        public static MessageType MoveQueuedDataPlates { get { return new MessageType("MoveQueuedDataPlates"); } }
        public static MessageType MoveQueuedManuals { get { return new MessageType("MoveQueuedManuals"); } }
        public static MessageType PCInvoiceMessage { get { return new MessageType("PCInvoiceMessage"); } }
        public static MessageType ProcessPCInvoices { get { return new MessageType("ProcessPCInvoices"); } }
        public static MessageType Refresh_Drawings_DB { get { return new MessageType("Refresh_Drawings_DB"); } }
        public static MessageType SendAssignedTickets { get { return new MessageType("SendAssignedTickets"); } }
        public static MessageType SendMonthlyPDFAttachments { get { return new MessageType("SendMonthlyPDFAttachments"); } }
        public static MessageType SendRTIMessage { get { return new MessageType("SendRTIMessage"); } }
        public static MessageType SendTicketReminders { get { return new MessageType("SendTicketReminders"); } }
        public static MessageType TicketsNotApprovedTimer { get { return new MessageType("TicketsNotApprovedTimer"); } }
        public static MessageType TicketNotification { get { return new MessageType("TicketNotification"); } }
        public static MessageType UpdatePartsDWG { get { return new MessageType("UpdatePartsDWG"); } }
        public static MessageType UpdatePartsPDF { get { return new MessageType("UpdatePartsPDF"); } }
        public static MessageType UpdateSerialsInEpicor10 { get { return new MessageType("UpdateSerialsInEpicor10"); } }
        public static MessageType WOL { get { return new MessageType("WOL"); } }
    }
}