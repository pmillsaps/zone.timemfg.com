using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.IT.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Building { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }
        public string LastDateEdited { get; set; }
        public string LastEditedBy { get; set; }
        public string ComputerName { get; set; }
        public int ComputerId { get; set; }
    }
}