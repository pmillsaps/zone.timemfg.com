using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.Data.Models.MessageQueue
{
    public class TicketNotificationMessage
    {
        public enum NotificationType
        {
            NewTicket,
            Approved,
            SupervisorNewTicket,
            Assignment,
            Update,
            UpdateAssigned,
            CompletionPending,
            Completed,
            ResourceChange,
            ResourceComplete,
            RequestedByChange,

            TaskAssignment,
            TaskCompleted
        }

        public NotificationType Notification { get; set; }
        public string Sender { get; set; }
        public int TicketId { get; set; }
        public int NoteId { get; set; }
        public int TaskId { get; set; }
    }
}