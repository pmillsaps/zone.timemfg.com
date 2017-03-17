using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.IT.ViewModel
{
    public class ProcessViewModel
    {
        public string ProcessId { get; set; }
        public DateTime CreationDate { get; set; }
        public string ExecutablePath { get; set; }
        public string PageFileUsage { get; set; }
    }
}