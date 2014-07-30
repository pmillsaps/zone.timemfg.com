﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.OrderLog.EntityModels
{
    [MetadataType(typeof(DealerMetadata))]
    public partial class Dealer
    {
    }

    public class DealerMetadata
    {
        [DisplayName("Dealer")]
        public string DealerName { get; set; }
    }
}