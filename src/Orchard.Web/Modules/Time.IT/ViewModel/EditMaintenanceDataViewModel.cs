﻿using System;
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
        [Required(ErrorMessage = "Company Name is required.")]
        public int CompanyId { get; set; }

        [Display(Name = "Budget Item")]
        [Required(ErrorMessage = "Budget Item is required.")]
        public string BudgetItem { get; set; }

        public string Supplier { get; set; }

        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Display(Name = "Original Purch Date")]
        public Nullable<System.DateTime> OriginalPurchDate { get; set; }

        [Display(Name = "Expiration Date")]
        public Nullable<System.DateTime> ExpirationDate { get; set; }

        [Display(Name = "Computer")]
        public Nullable<int> ComputerId { get; set; }

        [Display(Name = "License")]
        public Nullable<int> LicenseId { get; set; }
    }
}