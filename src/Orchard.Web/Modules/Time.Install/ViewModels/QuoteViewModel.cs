﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.Install;
using Time.Install.Models;

namespace Time.Install.ViewModels
{
    public class QuoteViewModel
    {
        public int QuoteNum { get; set; }
        public string InstallDescr { get; set; }
        public int LiftFamilyId { get; set; }
        public List<VSWOption> Options { get; set; }
        public List<AerialOption> AerialOptions { get; set; }
        public List<AddVSWOptionManually> AddOptnMnlly { get; set; }
    }
}