using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.Install.Models
{
    public class AddVSWOptionManually
    {
        public string AddOptionManually { get; set; }
        public int AddQuantityManually { get; set; }
        public decimal AddPriceManually { get; set; }
        public decimal AddLaborHoursManually { get; set; }
        public bool AddPaintFlagManually { get; set; }
    }
}