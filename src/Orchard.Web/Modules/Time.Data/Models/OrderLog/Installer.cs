using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.OrderLog
{
    [MetadataType(typeof(InstallerMetadata))]
    public partial class Installer
    {
    }

    public class InstallerMetadata
    {
        [DisplayName("Installer Name")]
        public string InstallerName { get; set; }
    }
}