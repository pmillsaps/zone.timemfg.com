using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Time.Data.EntityModels.TimeMFG
{
    [MetadataType(typeof(LoadListEmailMetadata))]
    public partial class LoadListEmail
    {
    }

    public class LoadListEmailMetadata
    {
        public string Name { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}