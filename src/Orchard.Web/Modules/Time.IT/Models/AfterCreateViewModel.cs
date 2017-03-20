using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.IT.Models
{
    public class AfterCreateViewModel
    {
        public int EmpID { get; set; }
        public int PropID { get; set; }
        public bool CellPhone { get; set; }
        public bool FindMyPhoneOff { get; set; }
        public bool CellReceived { get; set; }
        public bool Cables { get; set; }
        public bool CablesReceived { get; set; }
        public bool OfficeKey { get; set; }
        public bool OKeyReceived { get; set; }
        public bool BuildingKey { get; set; }
        public bool BKeyReceived { get; set; }
        public int ITID { get; set; }
        public string WindowsAccAccess { get; set; }
        public bool ArchiveEmail { get; set; }
        public string EmailAtW { get; set; }
        public bool FowardEmails { get; set; }
        public string ForwardAtW { get; set; }
        public bool DeleteUDrive { get; set; }
        public string UDriveAtW { get; set; }
        public bool DeskPhone { get; set; }
        public bool DeskPhoneFW { get; set; }
        public string DeskPhoneFWtW { get; set; }
        public bool DeskPhoneRename { get; set; }
        public string PhoneRenametW { get; set; }
        public bool EpicorAcc { get; set; }
        public string EpicorUserID { get; set; }
    }
}