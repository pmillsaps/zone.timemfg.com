using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.Epicor.ViewModels
{
    public class IssueOperationVM
    {
        public string JobNumber { get; set; }
        public List<string> Operations { get; set; }
        public string Operation { get; set; }
    }
}