using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.Production;

namespace Time.Epicor.ViewModels
{
    public class DistributorOrderListVM
    {
        public int Distributor { get; set; }
        public IEnumerable<V_DistributorOrderList> Orders { get; set; }
    }
}