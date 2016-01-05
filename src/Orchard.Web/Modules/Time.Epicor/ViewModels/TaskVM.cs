using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.Epicor;

namespace Time.Epicor.ViewModels
{
    public class TaskVM
    {
        public sysagenttask task { get; set; }
        public sysagentsched tasksched { get; set; }
    }
}
