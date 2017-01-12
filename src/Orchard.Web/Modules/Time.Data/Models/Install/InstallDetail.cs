using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.Install
{
    [MetadataType(typeof(InstallDetailMetadata))]
    public partial class InstallDetail
    {
    }

    public class InstallDetailMetadata
    {
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Price { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public Nullable<decimal> ExtendedPrice { get; set; }
    }
}