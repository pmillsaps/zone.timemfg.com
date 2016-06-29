using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.ITInventory
{
    [MetadataType(typeof(MonitorMetadata))]
    public partial class Monitor
    {
    }

    public class MonitorMetadata
    {
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
    }
}