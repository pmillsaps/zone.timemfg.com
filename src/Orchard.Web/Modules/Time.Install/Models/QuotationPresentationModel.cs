using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.Install;

namespace Time.Install.Models
{
    public class QuotationPresentationModel
    {
        public string LiftDescription { get; set; }
        public TitlesAndDescriptions TitlesAndDesc { get; set; }
        public ChassisSpecsForWordDoc ChassisSpecs { get; set; }
        public List<InstallDetailsForWordDoc> InstallDetails { get; set; }
        public QuoteDeptUsersForWordDoc QuoteUser { get; set; }

        public QuotationPresentationModel()
        {
            TitlesAndDesc = new TitlesAndDescriptions();
            InstallDetails = new List<InstallDetailsForWordDoc>();
        }
    }
}