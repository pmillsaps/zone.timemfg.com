using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.Configurator;

namespace Time.Configurator.Models
{
    public class ConfigOptionsViewModel
    {
        public List<ConfigOption> ConfigOptions { get; set; }
        public List<StructureSeq> KeyDescriptions { get; set; }
    }
}