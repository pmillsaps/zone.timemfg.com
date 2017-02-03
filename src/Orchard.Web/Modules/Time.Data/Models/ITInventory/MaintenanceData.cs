using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.ITInventory
{
    [MetadataType(typeof(MaintenanceDataMetadata))]
    public partial class MaintenanceData
    {
    }

    public class MaintenanceDataMetadata
    {
        [Required(ErrorMessage = "Company Name is required.")]
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Budget Item is required.")]
        [DisplayName("Budget Item")]
        public string BudgetItem { get; set; }

        [DisplayName("Account Number")]
        public string AccountNumber { get; set; }

        [DisplayName("Original Purch Date")]
        public Nullable<System.DateTime> OriginalPurchDate { get; set; }

        [DisplayName("Computer")]
        public Nullable<int> ComputerId { get; set; }

        [DisplayName("License")]
        public Nullable<int> LicenseId { get; set; }
    }
}