using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.IT.Models
{
    public class ComputersViewModel
    {
        public int Id { get; set; }
        public string CmpName { get; set; }
        public string Status { get; set; }
        public string Model { get; set; }
        public string WindowsKey { get; set; }
        public string Memory { get; set; }
        public string Processor { get; set; }
        public string DeviceType { get; set; }
        public string OS { get; set; }
        public string VideoCard { get; set; }
        public string Sound { get; set; }
        public string LastEditedBy { get; set; }
        public string LastDateEdited { get; set; }
        public string Note { get; set; }
        public string PO { get; set; }
        public string PurchaseDate { get; set; }
        public string AssetTag { get; set; }
        public string BIOS_Version { get; set; }
        public string WarrantyExpirationDate { get; set; }
        public string Notes { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
        public string LastBuildDate { get; set; }
        public string PhoneNumber { get; set; }
        public string SerialNumber { get; set; }
        public string AdditionalHW { get; set; }
    }
}
