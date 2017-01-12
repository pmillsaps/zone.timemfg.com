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
                if (tmp != null) _claimed = tmp.OpComplete;
                return _claimed;
            }
        }

        [DisplayName("RT")]
        public bool ReadyToTest
        {
            get
            {
                var _rt = false;
                var tmp = this.LoadListJobStatus.FirstOrDefault(x => x.OpCode.ToUpper() == "RT");
                if (tmp == null) tmp = this.LoadListJobStatus.FirstOrDefault(x => x.OpCode.ToUpper() == "MISC");
                if (tmp != null) _rt = tmp.OpComplete;
                return _rt;
            }
        }

        [DisplayName("Test")]
        public bool Tested
        {
            get
            {
                var _tested = false;
                var tmp = this.LoadListJobStatus.FirstOrDefault(x => x.OpCode.ToUpper() == "TEST");
                if (tmp != null) _tested = tmp.OpComplete;
                return _tested;
            }
        }

        [DisplayName("Blue")]
        public bool Blue
        {
            get
            {
                var _blue = false;
                var tmp = this.LoadListJobStatus.FirstOrDefault(x => x.OpCode.ToUpper() == "BLUE");
                if (tmp != null) _blue = tmp.OpComplete;
                return _blue;
            }
        }

        [DisplayName("Green")]
        public bool Green
        {
            get
            {
                var _green = false;
                var tmp = this.LoadListJobStatus.FirstOrDefault(x => x.OpCode.ToUpper() == "GREEN");
                if (tmp != null) _green = tmp.OpComplete;
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
                if (tmp != null) _lship = tmp.OpComplete;
                return _lship;
            }
        }

        [DisplayName("  Box  ")]
        public bool Box
        {
            get
            {
                var _box = false;
                var tmp = this.LoadListJobStatus.FirstOrDefault(x => x.OpCode.ToUpper() == "BOX");
                if (tmp != null) _box = tmp.OpComplete;
                return _box;
            }
        }

        [DisplayName("Ship")]
        public bool Ship
        {
            get
            {
                var _ship = false;
                var tmp = this.LoadListJobStatus.FirstOrDefault(x => x.OpCode.ToUpper() == "SHIP");
                if (tmp != null) _ship = tmp.OpComplete;
                return _ship;
            }
        }

        [DisplayName("OR")]
        public bool OutRigger
        {
            get
            {
                var _outrigger = false;
                var tmp = this.LoadListJobStatus.FirstOrDefault(x => x.OpCode.ToUpper() == "OR");
                if (tmp != null) _outrigger = tmp.OpComplete;
                if (tmp != null && tmp.IgnoreFlag == true) OutRigger_Ignored = true; else OutRigger_Ignored = false;
                return _outrigger;
            }
        }

        [DisplayName("Ped")]
        public bool Ped
        {
            get
            {
                var _ped = false;
                var tmp = this.LoadListJobStatus.FirstOrDefault(x => x.OpCode.ToUpper() == "PED");
                if (tmp != null) _ped = tmp.OpComplete;
                if (tmp != null && tmp.IgnoreFlag == true) Ped_Ignored = true; else Ped_Ignored = false;
                return _ped;
            }
        }

        [DisplayName("Bucket")]
        public bool Bucket
        {
            get
            {
                var _bucket = false;
                var tmp = this.LoadListJobStatus.FirstOrDefault(x => x.OpCode.ToUpper() == "BUCKET");
                if (tmp != null) _bucket = tmp.OpComplete;
                if (tmp != null && tmp.IgnoreFlag == true) Bucket_Ignored = true; else Bucket_Ignored = false;
                return _bucket;
            }
        }

        public bool Bucket_Ignored { get; set; }
        public bool OutRigger_Ignored { get; set; }
        public bool Ped_Ignored { get; set; }
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