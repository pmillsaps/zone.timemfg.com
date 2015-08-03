using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using VersaliftDataServices.EntityModels.TimeMfg;

namespace Time.Epicor.ViewModels
{
    public class MoveLoadListJobVM
    {
        public int LoadListJobId { get; set; }
        // public LoadListJob Job { get; set; }
        public SelectList LoadLists { get; set; }
        public int LoadList { get; set; }
    }
}