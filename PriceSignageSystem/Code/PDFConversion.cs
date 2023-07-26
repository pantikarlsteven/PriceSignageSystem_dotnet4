using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Code
{
    public class PDFConversion
    {
        public static void ConvertCrystalReportToPDF(string defaultPDFViewerLocation, ReportDocument report, string pdfPath, string pdf)
        {
            //Create Reports folder if not existing
            if (!Directory.Exists(pdfPath))
                Directory.CreateDirectory(pdfPath);


            //Export report to pdf
            ExportOptions CrExportOptions;
            DiskFileDestinationOptions CrDiskFileDestinationOptions = new DiskFileDestinationOptions();
            PdfRtfWordFormatOptions CrFormatTypeOptions = new PdfRtfWordFormatOptions();
            CrDiskFileDestinationOptions.DiskFileName = pdf;
            Logs.WriteToFile(CrDiskFileDestinationOptions.DiskFileName);
            CrExportOptions = report.ExportOptions;
            {
                CrExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                CrExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                CrExportOptions.DestinationOptions = CrDiskFileDestinationOptions;
                CrExportOptions.FormatOptions = CrFormatTypeOptions;
            }
            report.Export();

            const string pdfFileExtension = ".pdf";
            if (!System.IO.File.Exists(defaultPDFViewerLocation))
                Logs.WriteToFile("PDF viewer executable path does not exist.");

            // Set the PDF viewer application path as the default handler for .pdf files
            using (RegistryKey pdfFileKey = Registry.CurrentUser.OpenSubKey("Software\\Classes\\.pdf", true))
            {
                if (pdfFileKey != null)
                {
                    // Create a new subkey for the file type
                    using (RegistryKey fileTypeKey = Registry.CurrentUser.CreateSubKey("Software\\Classes\\" + pdfFileExtension.Substring(1)))
                    {
                        // Set the default value to the PDF viewer application name
                        fileTypeKey.SetValue("", "PDFFile", RegistryValueKind.String);
                    }
                    // Set the default value for the file type key to the PDF viewer executable path
                    pdfFileKey.SetValue("", "PDFFile", RegistryValueKind.String);
                }
            }

            // Set the default command to open PDF files with the specified viewer
            using (RegistryKey openCommandKey = Registry.CurrentUser.OpenSubKey("Software\\Classes\\PDFFile\\shell\\open\\command", true))
            {
                if (openCommandKey != null)
                {
                    openCommandKey.SetValue("", "\"" + defaultPDFViewerLocation + "\" \"%1\"", RegistryValueKind.String);
                }
            }
        }
    }
}