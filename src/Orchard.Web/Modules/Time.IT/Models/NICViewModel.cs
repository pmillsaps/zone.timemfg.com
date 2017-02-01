using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.IT.Models
{
    public class NICViewModel
    {
        public int Id { get; set; }
        public string MAC { get; set; }
        public string IP { get; set; }
        public string NICSpeed { get; set; }
        public string Type { get; set; }
        public string SwitchPort { get; set; }
        public string CableName { get; set; }
        public int ComputerId { get; set; }
        public string ComputerName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}