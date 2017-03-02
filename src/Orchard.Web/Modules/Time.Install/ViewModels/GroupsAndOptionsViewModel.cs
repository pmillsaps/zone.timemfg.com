using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.Install;

namespace Time.Install.ViewModels
{
    public class GroupsAndOptionsViewModel
    {
        public List<VSWOption> Options { get; set; }
        public List<OptionGroup> OptionGroups { get; set; }
    }
}