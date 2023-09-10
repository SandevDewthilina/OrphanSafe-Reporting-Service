using DevExpress.XtraReports.UI;
using HRMS_WEB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Npgsql;

namespace HRMS_WEB.Controllers
{
    [AllowAnonymous]
    public class ReporterController : Controller
    {

        private readonly IConfiguration configuration;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ILogger logger;
        private static String query;

        public ReporterController(IConfiguration configuration,
            IHostingEnvironment hostingEnvironment, 
            ILogger<ReporterController> logger)
        {
            this.configuration = configuration;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
        }

        public IActionResult AvailableReports()
        {
            var pathfactor = "\\";
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                pathfactor = "/";
            }


            var folderpath = Path.Combine(hostingEnvironment.ContentRootPath, "Reports");
            var reportnamelist = new List<string>();
            foreach (String file in Directory.EnumerateFiles(folderpath, "*", SearchOption.TopDirectoryOnly))
            {

                var filename = file.Substring(file.LastIndexOf(pathfactor) + 1);

                if (filename.Contains("ecodex"))
                {
                        reportnamelist.Add(filename.Replace(".ecodex", ""));
                    
                }


            }
            ViewBag.reportnamelist = reportnamelist;
            return View();
        }

        public IActionResult Viewer(String sql)
        {
            return View();
        }

        //public ActionResult Export(string format = "pdf")
        //{
        //    format = format.ToLower();
        //    Samplereport report = ViewBag.report;
        //    string contentType = string.Format("application/{0}", format);
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        switch (format)
        //        {
        //            case "pdf":
        //                contentType = "application/pdf";
        //                report.ExportToPdf(ms);
        //                break;
        //            case "docx":
        //                contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        //                report.ExportToDocx(ms);
        //                break;
        //            case "xls":
        //                contentType = "application/vnd.ms-excel";
        //                report.ExportToXls(ms);
        //                break;
        //            case "xlsx":
        //                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //                report.ExportToXlsx(ms);
        //                break;
        //            case "rtf":
        //                report.ExportToRtf(ms);
        //                break;
        //            case "mht":
        //                contentType = "message/rfc822";
        //                report.ExportToMht(ms);
        //                break;
        //            case "html":
        //                contentType = "text/html";
        //                report.ExportToHtml(ms);
        //                break;
        //            case "txt":
        //                contentType = "text/plain";
        //                report.ExportToText(ms);
        //                break;
        //            case "csv":
        //                contentType = "text/plain";
        //                report.ExportToCsv(ms);
        //                break;
        //            case "png":
        //                contentType = "image/png";
        //                report.ExportToImage(ms, new ImageExportOptions() { Format = System.Drawing.Imaging.ImageFormat.Png });
        //                break;
        //        }
        //        return File(ms.ToArray(), contentType);
        //    }
        //}

        public DataSet getDataset(String sql, string sreportname = "Dynamic report")
        {
            using var conn = new NpgsqlConnection(configuration.GetConnectionString("DefaultPostgreSQLConnection"));
            try
            {
                // open the database connection
                conn.Open();

                // mysql command
                var command = new NpgsqlCommand(sql, conn);

                // database reader
                var reader = command.ExecuteReader();

                // load data and store
                var dataTable = new DataTable();
                dataTable.Load(reader);

                DataSet dataSet1 = new DataSet();
                dataSet1.DataSetName = sreportname;

                dataSet1.Tables.Add(dataTable);

                return dataSet1;

            }
            catch (Exception ex)
            {
                return new DataSet();
            }
        }

        public DataTable getDropDownData(String sql)
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

                    return dataTable;

                }
                catch (Exception ex)
                {
                    return new DataTable();
                }
            }
        }

        private object GetRows(DataTable dt)
        {
            var rows = new List<object>();
            var columns = dt.Columns.Cast<DataColumn>().ToList();

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

        public IActionResult GenerateReport(String xmlname)
        {
            var reportpath = Path.Combine(hostingEnvironment.ContentRootPath, "Reports");
            var folderpath = Path.Combine(reportpath, xmlname);
            var dropdownlist = new List<object>();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ReportRoot));
            using (StreamReader stream = new StreamReader(folderpath))
            {
                ReportRoot input = (ReportRoot)xmlSerializer.Deserialize(stream);

                IEnumerable<string> itemlist = input.EParameters.Where(ep => !ep.type.ToLower().Equals("entity")).Select(p => p.bindingName + ":" + p.type + ":" + p.name);

                IEnumerable<EParameter> dropDownParams = input.EParameters.Where(ep => ep.type.ToLower().Equals("entity"));

                foreach (EParameter epara in dropDownParams)
                {
                    if (epara.query != null && !epara.query.Equals(""))
                    {
                        dropdownlist.Add(new { bindingname = epara.bindingName, lablename = epara.name, data = GetRows(getDropDownData(epara.query)) });
                    }
                }


                bool hasparams = bool.Parse(input.Query.hasParams);
                query = input.Query.value;
                String reportname = input.ReportName;
                String filename = input.FileName;
                bool IsDynamic = input.IsDynamicEnable;

                ViewBag.formItems = itemlist;
                ViewBag.hasparams = hasparams;
                ViewBag.reportname = reportname;
                ViewBag.filename = filename;
                ViewBag.IsDynamic = IsDynamic;
                stream.Close();

            }

            ViewBag.dropdownRows = JsonConvert.SerializeObject(dropdownlist);

            return View();

        }

        public async Task<IActionResult> ReturnReport(String args, String sql, bool hasparams, String filename, String reportname, bool IsDynamic)
        {
            sql = query;
            try
            {
                if (hasparams && args != null)
                {
                    List<String> parameters = new List<string>();
                    if (args.Contains(","))
                    {
                        parameters = args.Split(",").ToList();
                    }
                    else
                    {
                        parameters.Add(args);
                    }

                    Dictionary<string, string> paramDic = new Dictionary<string, string>();

                    foreach (string complexpara in parameters)
                    {
                        if (!paramDic.ContainsKey(complexpara.Split("*")[2]))
                        {
                            paramDic.Add(complexpara.Split("*")[2], complexpara.Split("*")[0]);
                        }
                    }

                    if (IsDynamic)
                    {

                        foreach (string key in paramDic.Keys)
                        {
                            sql = sql.Replace("@" + key, paramDic[key]);
                        }
                    }
                    else
                    {

                        switch (reportname)
                        {
                            // case "Month End Draughtmen Report":
                            //     var obj = viewdataRepository.GetUserMonthEndSummary(paramDic.FirstOrDefault().Value, int.Parse(paramDic.ElementAt(1).Value)).Result;
                            //     MonthEndUserReport xreport = new MonthEndUserReport(obj);
                            //     ViewBag.report = xreport;
                            //     return View("/Views/Reporter/Viewer.cshtml");
                            default:
                                return View("/Views/Reporter/Viewer.cshtml");
                        }


                    }

                }
                var reportpath = Path.Combine(hostingEnvironment.ContentRootPath, "Reports");
                XtraReport report = new XtraReport();
                report.LoadLayoutFromXml(Path.Combine(reportpath, filename + ".vsrepx"));
                logger.LogWarning(sql);
                DataSet ds = getDataset(sql, sreportname: reportname);
                report.DataSource = ds;
                if (ds.Tables != null && ds.Tables.Count > 0)
                {
                    report.DataMember = ds.Tables[0].TableName;
                }
                ViewBag.report = report;

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
            return View("/Views/Reporter/Viewer.cshtml");
        }

        public IActionResult ReportDesigner(String xmlname)
        {
            var reportpath = Path.Combine(hostingEnvironment.ContentRootPath, "Reports");
            var folderpath = Path.Combine(reportpath, xmlname);
            ReportRoot input = null;

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ReportRoot));
            using (StreamReader stream = new StreamReader(folderpath))
            {
                input = (ReportRoot)xmlSerializer.Deserialize(stream);
                stream.Close();

            }

            //Type t = Type.GetType($"HRMS_WEB.Reports.{input.FileName}");
            //ConstructorInfo constructorInfo = t.GetConstructors()[0];
            //constructorInfo.Invoke(new object[] { });
            //XtraReport report = (XtraReport)Activator.CreateInstance(t);
            XtraReport report = new XtraReport();
            report.LoadLayoutFromXml(Path.Combine(reportpath, input.FileName + ".vsrepx"));
            ViewBag.report = report;
            return View();
        }
    }
}
