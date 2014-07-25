﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.OrderLog.EntityModels
{
    public class OrderLog
    {
        [MetadataType(typeof(OrderMetadata))]
        public partial class Order
        {
        }

        public class OrderMetadata
        {
            [System.ComponentModel.DisplayName("PO#")]
            public string PO { get; set; }

            [System.ComponentModel.DisplayName("Dealer")]
            public string DealerName { get; set; }
        }
    }
}