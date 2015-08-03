using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.TimeMFG;

namespace Time.Support.Models
{
    public enum UpdateType
    {
        Updated,
        CompletionPending,
        WaitingSupervisorApproval,

    }

    public class TicketProjectUpdate
    {
        public TicketProject OldTicket { get; set; }
        public TicketProject NewTicket { get; set; }
        public string ChangeUser { get; set; }
        public UpdateType updateType { get; set; }
    }
}