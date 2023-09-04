using DevExpress.DataAccess.ObjectBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Models
{
    public class UserMonthEndSummaryDTO
    {
        public String Name { get; set; }
        public String Month { get; set; }
        public String Designation { get; set; }
        public String Year { get; set; }
        public String OTWeekdaySum { get; set; }
        public String OTWeekdayEarning { get; set; }
        public String OTWeekendSum { get; set; }
        public String OTWeekendEarning { get; set; }
        public String OTAllEarning { get; set; }
        public List<DaySummaryDTO> DaySummaries { get; set; }

    }

    public class DaySummaryDTO
    {
        public int DateNo { get; set; }
        public String DayName { get; set; }
        public DateTime FirstIn { get; set; }
        public DateTime LastOut { get; set; }
        public String IdleHours { get; set; }
        public String WorkedHours { get; set; }
        public String OTTimeweekday { get; set; }
        public String OTTimeweekend { get; set; }
        public bool IsHoliday { get; set; }
    }
}
