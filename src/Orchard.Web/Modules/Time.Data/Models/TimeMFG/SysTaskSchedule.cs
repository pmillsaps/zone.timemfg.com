using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Time.Data.EntityModels.TimeMFG
{
    [MetadataType(typeof(SysTaskScheduleMetadata))]
    public partial class SysTaskSchedule
    {
    }

    public class SysTaskScheduleMetadata
    {
        [DisplayName("Type")]
        public string SchedType { get; set; }
        [DisplayName("Task Desc")]
        public string Description { get; set; }
        [DisplayName("Next Run")]
        public DateTime NextRunOn { get; set; }
        [DisplayName("Effective")]
        [DataType(DataType.Date)]
        public DateTime EffectiveDate { get; set; }
        [DisplayName("Mon Opt")]
        public string MonthlyOption { get; set; }
        [DisplayName("DOM")]
        public string DayOfMonth { get; set; }
        [DisplayName("Week")]
        public string WeekOfMonth { get; set; }
        [DisplayName("Day")]
        public string DayOfWeek { get; set; }
        [DisplayName("Rpt Weeks")]
        public string EveryNWeeks { get; set; }
        [DisplayName("Last Run")]
        public DateTime LastRunOn { get; set; }
        [DisplayName("Mon")]
        public bool Mondays { get; set; }
        [DisplayName("Tue")]
        public bool Tuesdays { get; set; }
        [DisplayName("Wed")]
        public bool Wednesdays { get; set; }
        [DisplayName("Thu")]
        public bool Thursdays { get; set; }
        [DisplayName("Fri")]
        public bool Fridays { get; set; }
        [DisplayName("Sat")]
        public bool Saturdays { get; set; }
        [DisplayName("Sun")]
        public bool Sundays { get; set; }
        [DisplayName("HR")]
        public int StartHour { get; set; }
        [DisplayName("Min")]
        public int StartMinute { get; set; }
        [DisplayName("Win Start")]
        public int WindowStart { get; set; }
        [DisplayName("Win End")]
        public int WindowEnd { get; set; }
        [DisplayName("Win Mins")]
        public string WindowMinutes { get; set; }
    }
}