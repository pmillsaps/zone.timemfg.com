using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.TimeMFG;

namespace Time.Support.Models
{
    public class TicketViewModel
    {
        public bool Admin { get; set; }
        public bool IT { get; set; }
        public TicketProject Ticket { get; set; }
        public IEnumerable<TicketTask> Tasks { get; set; }
        public SelectList CategoryID { get; set; }
        public SelectList DepartmentID { get; set; }
        public SelectList AssignedEmployeeID { get; set; }
        public SelectList ResourceEmployeeID { get; set; }
        public SelectList PriorityID { get; set; }
        public SelectList Status { get; set; }
    }
}