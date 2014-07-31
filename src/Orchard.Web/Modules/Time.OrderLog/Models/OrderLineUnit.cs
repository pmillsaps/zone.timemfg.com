using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.OrderLog.EntityModels
{
    [MetadataType(typeof(OrderLineUnitMetadata))]
    public partial class OrderLineUnit
    {
    }

    public class OrderLineUnitMetadata
    {
        [DisplayName("ATS Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public string ATSDate { get; set; }

        [DisplayName("Sales Order #")]
        public string SalesOrderNum { get; set; }

        [DisplayName("Invoice Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public string InvoiceDate { get; set; }

        [DisplayName("Invoice Amount")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public string InvoiceAmt { get; set; }

        [DisplayName("Comment")]
        public string UnitComment { get; set; }
    }
}