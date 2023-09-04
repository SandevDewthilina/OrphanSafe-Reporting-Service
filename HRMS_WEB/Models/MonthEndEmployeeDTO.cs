using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Models
{
    public class MonthEndEmployeeDTO
    {
        public string Month { get; set; }

        public List<MonthEndEmployeeProfile> monthEndEmployeeDTOs { get; set; }
    }

    public class MonthEndEmployeeProfile
    {
        public int Index { get; set; }
        public String Name { get; set; }
        public String WorkedHours { get; set; }
        public int WorkedDays { get; set; }
        public String IdleHours { get; set; }
        public String OtHours { get; set; }
        public String HolidayOTHours { get; set; }
        public String WeekdayOTHours { get; set; }
        public String PerformaceIndex { get; set; }
    }
}
