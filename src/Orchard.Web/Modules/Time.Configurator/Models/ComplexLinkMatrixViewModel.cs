using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.Configurator;

namespace Time.Configurator.Models
{
    public class ComplexLinkMatrixViewModel
    {
        public ComplexStructure complexStructure { get; set; }
        public IEnumerable<Lookup> Lookups { get; set; }
        public IEnumerable<IEnumerable<ComplexLink>> ComplexLinks { get; set; }
    }
}