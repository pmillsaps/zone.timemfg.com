using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.TimeMFG
{
    public partial class TicketEmployee
    {
        public string FullName 
        { 
            get{ return string.Format("{0} {1}", FirstName, LastName).Trim(); }
        }
    }

    //public class EmployeeMetadata
    //{
    //    [DisplayName("Dealer")]
    //    public string DealerName { get; set; }
    //}
}