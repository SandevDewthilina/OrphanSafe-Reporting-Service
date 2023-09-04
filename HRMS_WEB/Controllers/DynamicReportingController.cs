using HRMS_WEB.DbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Controllers
{
    public class DynamicReportingController : Controller
    {
        private readonly HRMSDbContext context;
        private readonly IConfiguration configuration;

        public DynamicReportingController(HRMSDbContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        public IActionResult LiveQuery()
        {
            return View();
        }

        public IActionResult LiveQueryPreview(String sql)
        {

            using (var conn = new MySqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    // open the database connection
                    conn.Open();

                    // mysql command
                    var command = new MySqlCommand(sql, conn);

                    // database reader
                    var reader = command.ExecuteReader();

                    // load data and store
                    var dataTable = new DataTable();
                    dataTable.Load(reader);

                    var resultobjects = GetRows(dataTable);

                    ViewBag.Rows = JsonConvert.SerializeObject(resultobjects);
                    ViewBag.rowcount = dataTable.Rows.Count;
                }
                catch (Exception ex)
                {
                    return NotFound(ex.StackTrace);
                }
            }

            return View();
        }

        private object GetRows(DataTable dt)
        {
            var rows = new List<object>();

            var headerRow = new List<object>();
            var columns = dt.Columns.Cast<DataColumn>().ToList();
            for (int colIndex = 0; colIndex < columns.Count; colIndex++)
            {
                var col = columns[colIndex];
                headerRow.Add(new { value = col.ColumnName });
            }
            rows.Add(new { cells = headerRow });

            foreach (DataRow row in dt.Rows)
            {
                var dataRow = new List<object>();
                for (int colIndex = 0; colIndex < columns.Count; colIndex++)
                {
                    var col = columns[colIndex];

                    if (row[col] == null || row[col] == DBNull.Value)
                        dataRow.Add(new { value = "" });
                    else if (col.DataType == typeof(decimal))
                    {
                        dataRow.Add(new { value = (decimal)row[col] });
                    }
                    else if (col.DataType == typeof(int))
                        dataRow.Add(new { value = (int)row[col] });
                    else
                        dataRow.Add(new { value = row[col].ToString() });
                }

                rows.Add(new { cells = dataRow });
            }

            return rows;
        }

    }
}
