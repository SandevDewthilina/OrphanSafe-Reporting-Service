using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HRMS_WEB.Services
{
    public class ReportStorageWebExtension1 : DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension
    {
        readonly string ReportDirectory;
        const string FileExtension = ".vsrepx";
        public ReportStorageWebExtension1(IWebHostEnvironment env)
        {
            ReportDirectory = Path.Combine(env.ContentRootPath, "Reports");
            if (!Directory.Exists(ReportDirectory))
            {
                Directory.CreateDirectory(ReportDirectory);
            }
        }

        public override bool CanSetData(string url)
        {
            // Determines whether or not it is possible to store a report by a given URL. 
            // For instance, make the CanSetData method return false for reports that should be read-only in your storage. 
            // This method is called only for valid URLs (i.e., if the IsValidUrl method returned true) before the SetData method is called.

            return true;
        }

        public override bool IsValidUrl(string url)
        {
            // Determines whether or not the URL passed to the current Report Storage is valid. 
            // For instance, implement your own logic to prohibit URLs that contain white spaces or some other special characters. 
            // This method is called before the CanSetData and GetData methods.

            return true;
        }


        public override void SetData(XtraReport report, string url)
        {
            // Stores the specified report to a Report Storage using the specified URL. 
            // This method is called only after the IsValidUrl and CanSetData methods are called.
            
            report.SaveLayoutToXml(Path.Combine(ReportDirectory, url + FileExtension));
        }

        public override string SetNewData(XtraReport report, string defaultUrl)
        {
            // Stores the specified report using a new URL. 
            // The IsValidUrl and CanSetData methods are never called before this method. 
            // You can validate and correct the specified URL directly in the SetNewData method implementation 
            // and return the resulting URL used to save a report in your storage.
            string destinationDirectory = ReportDirectory;
            string newFileName = defaultUrl + ".ecodex"; // Replace with your desired new file name
            string destinationFilePath = Path.Combine(destinationDirectory, newFileName);
            string sourceFilePath = Path.Combine(destinationDirectory, "DefaultTemplate.ecodex");
            try
            {
                if (!File.Exists((destinationFilePath)))
                {
                    string fileContent = File.ReadAllText(sourceFilePath);
                    fileContent = fileContent.Replace("#report-name", defaultUrl);
                    fileContent = fileContent.Replace("DefaultTemplate", defaultUrl);
                    File.WriteAllText(destinationFilePath, fileContent);
                }
                report.SaveLayoutToXml(Path.Combine(ReportDirectory, defaultUrl + FileExtension));
            }
            catch (IOException e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
            }
            return defaultUrl;
        }
    }
}