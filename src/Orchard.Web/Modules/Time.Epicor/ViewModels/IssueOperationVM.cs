using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Time.Epicor.ViewModels
{
    public class IssueOperationVM
    {
        public string JobNumber { get; set; }
        public IEnumerable<SelectListItem> Operations { get; set; }
        public string Operation { get; set; }
    }
}