using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Time.Data.EntityModels.TimeMFG
{
    [MetadataType(typeof(LoadListJobMetadata))]
    public partial class LoadListJob
    {
        [DisplayName("Claim")]
        public bool Claimed
        {
            get
            {
                var _claimed = false;
                var tmp = this.LoadListJobStatus.FirstOrDefault(x => x.OpCode.ToUpper() == "CLAIM");
                if (tmp != null) _claimed = tmp.OpComplete == 1;
                return _claimed;
            }
        }

        [DisplayName("Test")]
        public bool Tested
        {
            get
            {
                var _tested = false;
                var tmp = this.LoadListJobStatus.FirstOrDefault(x => x.OpCode.ToUpper() == "TEST");
                if (tmp != null) _tested = tmp.OpComplete == 1;
                return _tested;
            }
        }

        [DisplayName("Post")]
        public bool Posted
        {
            get
            {
                var _posted = false;
                var tmp = this.LoadListJobStatus.FirstOrDefault(x => x.OpCode.ToUpper() == "POST");
                if (tmp != null) _posted = tmp.OpComplete == 1;
                return _posted;
            }
        }

        [DisplayName("Green")]
        public bool Green
        {
            get
            {
                var _green = false;
                var tmp = this.LoadListJobStatus.FirstOrDefault(x => x.OpCode.ToUpper() == "GREEN");
                if (tmp != null) _green = tmp.OpComplete == 1;
                return _green;
            }
        }

        [DisplayName("LShip")]
        public bool LShip
        {
            get
            {
                var _lship = false;
                var tmp = this.LoadListJobStatus.FirstOrDefault(x => x.OpCode.ToUpper() == "LSHIP");
                if (tmp != null) _lship = tmp.OpComplete == 1;
                return _lship;
            }
        }
    }



    public class LoadListJobMetadata
    {
        [DisplayName("Job Number")]
        public string JobNumber { get; set; }
        [DisplayName("Serial No")]
        public string SerialNo { get; set; }
        [DisplayName("Distributor PO")]
        public string DistributorPO { get; set; }
        [DisplayName("ATS Date")]
        public DateTime DateATS { get; set; }
        public string Destination { get; set; }
        [UIHint("MultiLineText")]
        [DisplayName("Job Comments")]
        public string Comments { get; set; }
        [DisplayName("Customer Name")]
        public string CustomerName { get; set; }
    }
}
