using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.OrderLog
{
    [MetadataType(typeof(InstallMetadata))]
    public partial class Install
    {
    }

    public class InstallMetadata
    {
        [DisplayName("Install")]
        public string InstallName { get; set; }
    }
}