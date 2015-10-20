using System;
using System.Collections.Generic;
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
        //public string Name { get; set; }
        //public string LicenseKey { get; set; }
    }
}