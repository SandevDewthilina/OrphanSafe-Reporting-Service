using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Schedule;

namespace HRMS_WEB.DbContext
{
    public class HRMSDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public HRMSDbContext(DbContextOptions<HRMSDbContext> options) : base(options)
        {
        }
    }
}