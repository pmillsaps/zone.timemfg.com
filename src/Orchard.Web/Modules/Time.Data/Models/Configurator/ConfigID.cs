using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.Configurator
{
    
    [MetadataType(typeof(ConfigIDMetadata))]
    public partial class ConfigID
    {
    }

    public class ConfigIDMetadata
    {
        [DisplayName("Configurator ID")]
        public string ConfigID1 { get; set; }
    }
    

}
