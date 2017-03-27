using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.Install;

namespace Time.Install.ViewModels
{
    public class LiftFamilyViewModel
    {
        public LiftFamily Lift { get; set; }
        public ChassisSpecsForWordDoc ChassisSpecs { get; set; }
    }
}