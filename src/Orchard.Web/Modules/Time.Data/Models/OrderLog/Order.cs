using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.OrderLog
{
    [MetadataType(typeof(OrderMetadata))]
    public partial class Order
    {
        public string CityStateZip { get {return String.Format("{0} {1} {2}", City, State, Zip);} }
    }

    public class OrderMetadata
    {
        [DisplayName("PO#")]
        public string PO { get; set; }
    }
}