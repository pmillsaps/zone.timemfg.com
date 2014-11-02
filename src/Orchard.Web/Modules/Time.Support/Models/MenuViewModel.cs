using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.TimeMFG;

namespace Time.Support.Models
{
    public class MenuViewModel
    {
        public bool Admin { get; set; }
        public bool IT { get; set; }

        public List<Department> OpenTicketsbyDepartment { get; set; }
        public List<Status> OpenTicketsbyStatus { get; set; }
        public List<Status> MyOpenTicketsbyStatus { get; set; }
        
        public List<Category> OpenTicketsbyCategory { get; set; }
        public List<AssignedTo> OpenTicketsbyAssignment { get; set; }
    }

    public class AssignedTo
    {
        public TicketEmployee Employee { get; set; }
        public int Count { get; set; }
    }

    public class Status
    {
        public TicketStatus TicketStatus { get; set; }
        public int Count { get; set; }
    }

    public class Department
    {
        public TicketDepartment TicketDepartment { get; set; }
        public int Count { get; set; }
    }

    public class Category
    {
        public TicketCategory TicketCategory { get; set; }
        public int Count { get; set; }
    }
}