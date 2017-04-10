using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.Production;

namespace Time.Epicor.ViewModels
{
    public class E10_TaskVM
    {
        public SysAgentTask task { get; set; }
        public SysAgentSched tasksched { get; set; }
    }
}