using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.Install.ViewModels
{
    // This model aids with adding VSW options to the db 
    public class AddOptionsViewModel
    {
        public int GroupId { get; set; }
        public int LiftFamilyId { get; set; }
        public string OptionsToParse { get; set; }
    }
}