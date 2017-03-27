using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.ITInventory
{
    [MetadataType(typeof(MaintDataCompanyMetadata))]
    public partial class MaintDataCompany
    {
    }

    public class MaintDataCompanyMetadata
    {
        [Required]
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }
    }
}