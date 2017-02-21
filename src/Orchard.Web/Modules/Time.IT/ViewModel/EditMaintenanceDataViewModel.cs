using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.IT.ViewModel
{
    public class EditMaintenanceDataViewModel
    {
        public int Id { get; set; }
        public int MntcDataDtlsId { get; set; }

        [Display(Name = "Company Name")]
        [Required]
        public string CompanyName { get; set; }

        [Display(Name = "Budget Item")]
        [Required]
        public string BudgetItem { get; set; }

        public string Supplier { get; set; }

        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Display(Name = "Original Purch Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> OriginalPurchDate { get; set; }

        [Display(Name = "Expiration Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ExpirationDate { get; set; }

        [Display(Name = "Computer")]
        public Nullable<int> ComputerId { get; set; }

        [Display(Name = "License")]
        public Nullable<int> LicenseId { get; set; }
    }
}