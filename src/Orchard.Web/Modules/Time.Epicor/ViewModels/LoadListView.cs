using System;
using System.Collections.Generic;
using System.Linq;
using Time.Data.EntityModels.TimeMFG;

namespace Time.Epicor.ViewModels
{
    public class LoadListView
    {
        public LoadList LoadList { get; set; }
        public bool Complete { get; set; }
        public bool MakeReady { get; set; }
    }
}