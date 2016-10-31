using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.OrderLog
{
    [MetadataType(typeof(OrderLineUnitMetadata))]
    public partial class OrderLineUnit
    {
    }

    public class OrderLineUnitMetadata
    {
        [DisplayName("ATS Date")]
        public string ATSDate { get; set; }

        [DisplayName("Sales Order #")]
        public string SalesOrderNum { get; set; }

        [DisplayName("Invoice Date")]
        public string InvoiceDate { get; set; }

        [DisplayName("Invoice Amount")]
        public string InvoiceAmt { get; set; }
    }
}