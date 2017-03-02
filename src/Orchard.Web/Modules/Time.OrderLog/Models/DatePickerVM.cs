using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.OrderLog.Models
{
    public class DatePickerVM
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public DatePickerVM()
        {
            StartDate = null;
            EndDate = null;
        }
    }
}