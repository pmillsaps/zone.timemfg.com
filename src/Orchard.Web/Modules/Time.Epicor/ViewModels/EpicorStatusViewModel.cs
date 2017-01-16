using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.Epicor.ViewModels
{
    public class EpicorStatusViewModel
    {
        public string MRPStatus { get; set; }
        public IMViewModel imViewModel { get; set; }
        public List<TaskVM> tasks { get; set; }
    }
}