using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Time.Data.EntityModels.ITInventory
{
    [MetadataType(typeof(MaintenanceDataDetailMetadata))]
    public partial class MaintenanceDataDetail
    {
    }

    public class MaintenanceDataDetailMetadata
    {
        [Display(Name = "Maintenance Data")]
        public int MaintenanceDataId { get; set; }

        [Required(ErrorMessage = "Purchase Date is required.")]
        [Display(Name = "Purchase Date")]
        public System.DateTime PurchaseDate { get; set; }

        [Required(ErrorMessage = "Expiration Date is required.")]
        [Display(Name = "Expiration Date")]
        public System.DateTime ExpirationDate { get; set; }

        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Display(Name = "Alternate Info")]
        [DataType(DataType.MultilineText)]
        public string AlternateInfo { get; set; }
    }
}