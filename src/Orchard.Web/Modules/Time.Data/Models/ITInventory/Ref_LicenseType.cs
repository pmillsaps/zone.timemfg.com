using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.ITInventory
{
    [MetadataType(typeof(Ref_LicenseTypeMetadata))]
    public partial class Ref_LicenseType
    {
    }

    public class Ref_LicenseTypeMetadata
    {
        [DisplayName("License Type")]
        public string LicenseType { get; set; }
    }
}