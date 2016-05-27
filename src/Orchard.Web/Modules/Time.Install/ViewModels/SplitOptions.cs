using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.Install.ViewModels
{
    // This class handles the VSW options when imported form EXCEL
    public class SplitOptions
    {
        public string Option { get; set; }
        public decimal Price { get; set; }
        public double Hours { get; set; }
    }
}