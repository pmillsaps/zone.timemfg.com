using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.Install;

namespace Time.Install.Models
{
    public class InstallQuoteSummary
    {
        public InstallQuote InstallQuotes { get; set; }
        public List<InstallDetail> InstallDetails { get; set; }
        public List<InstallDetailsManuallyAddedOption> InstallDetailsMnllyAdded { get; set; }
        public List<AerialOption> AerialOptions { get; set; }
    }
}