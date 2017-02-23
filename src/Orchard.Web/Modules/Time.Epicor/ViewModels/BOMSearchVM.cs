using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Time.Epicor.Models;

namespace Time.Epicor.ViewModels
{
    public class BOMSearchVM
    {
        public List<BOMInfo> bomInfo { get; set; }

        [DisplayName("Filter Raw Material ?")]
        public bool FilterRawMaterial { get; set; }

        [DisplayName("Drill Into Purchase Items ?")]
        public bool DrillIntoPurchaseItems { get; set; }

        [DisplayName("Purchase Items Only ?")]
        public bool PurchaseItemsOnly { get; set; }

        [DisplayName("View Alt Methods ?")]
        public bool ViewAltMethods { get; set; }

        [DisplayName("Export to Excel ?")]
        public bool ExportToExcel { get; set; }

        [DisplayName("Use B52 Data ?")]
        public bool UseTestData { get; set; }

        [DataType(DataType.MultilineText)]
        public string SearchText { get; set; }

        [DisplayName("Search by Job")]
        public bool SearchByJob { get; set; }

        [DisplayName("Drill Into Job")]
        public bool DrillJob { get; set; }
    }
}