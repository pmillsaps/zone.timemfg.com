using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Time.Data.EntityModels.TimeMFG
{
    [MetadataType(typeof(LoadListJobStatusMetadata))]
    public partial class LoadListJobStatu
    {
    }

    public class LoadListJobStatusMetadata
    {
        [UIHint("ByteCheckBox")]
        public byte OpComplete { get; set; }
    }
}
