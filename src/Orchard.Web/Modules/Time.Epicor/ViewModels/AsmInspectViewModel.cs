using System;
using System.Collections.Generic;
using System.ComponentModel;
using Time.Data.EntityModels.TimeMFG;

namespace Time.Epicor.ViewModels
{
    public class AsmInspectViewModel
    {
        public AsmInspectViewModel()
        {
            EndDate = DateTime.Now.AddDays(11);
            //Claimed = false;
            //Tested = true;
            //Posted = true;
            //Green = true;
        }

        [DisplayName("End Date")]
        public DateTime EndDate { get; set; }

        [DisplayName("Claim")]
        public bool Claimed { get; set; }

        [DisplayName("Test")]
        public bool Tested { get; set; }

        [DisplayName("Post")]
        public bool Posted { get; set; }

        [DisplayName("Green")]
        public bool Green { get; set; }

        public List<LoadListJob> JobList { get; set; }
    }
}