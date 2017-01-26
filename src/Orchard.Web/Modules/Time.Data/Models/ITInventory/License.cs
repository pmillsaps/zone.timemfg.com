using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.ITInventory
{
    [MetadataType(typeof(LicenseMetadata))]
    public partial class License
    {
        public string FullName
        {
            // This is assuming you've also fixed the property names to be conventional
            // I'd also suggest changing "Name" to "GivenName" or "FirstName".
            get { return string.Format("{0} | {1}", Name, LicenseKey); }
        }
    }

    public class LicenseMetadata
    {
        [DisplayName("License Key")]
        public string LicenseKey { get; set; }

        [DisplayName("Quantity Assigned")]
        public int QuantityAssigned { get; set; }

        [DisplayName("Computer")]
        public Nullable<int> ComputerId { get; set; }

        [DisplayName("License Type")]
        public Nullable<int> LicenseTypeId { get; set; }

        [DisplayName("Purchase Date")]
        public Nullable<System.DateTime> PurchaseDate { get; set; }

        [DataType(DataType.MultilineText)]
        public string Note { get; set; }
    }
}