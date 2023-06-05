using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using PriceSignageSystem.Helper;
using PriceSignageSystem.Models.Constants;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace PriceSignageSystem.Controllers
{
    public class ReportController : Controller
    {
        private readonly ISTRPRCRepository _sTRPRCRepository;

        public ReportController(ISTRPRCRepository sTRPRCRepository)
        {
            _sTRPRCRepository = sTRPRCRepository;
        }

        public List<STRPRCDto> GetData(decimal O3SKU)
        {
            var data = _sTRPRCRepository.GetData(O3SKU);
            return data;
        }

        public CountryDto GetCountryImg(string country)
        {
            var data = _sTRPRCRepository.GetCountryImg(country);
            return data;
        }

        public ActionResult PreviewCrystalReport(string id)
        {
            // Create a new instance of the report document
            ReportDocument reportDoc = new ReportDocument();

            // Load the report file (.rpt)
            reportDoc.Load(Server.MapPath("~/Reports/CrystalReports/WholeReport/WholeReport_SLBrandAndSLDesc.rpt"));

            // Set the database login information if required
            // reportDoc.SetDatabaseLogon("username", "password");

            reportDoc.SetDatabaseLogon("sa", "@dm1n@8800");
            reportDoc.SetParameterValue("sku", id);
            reportDoc.SetParameterValue("user", Session["Username"].ToString());

            // Export the report to PDF
            Stream stream = reportDoc.ExportToStream(ExportFormatType.PortableDocFormat);

            // Convert the PDF stream to a byte array
            byte[] pdfBytes = new byte[stream.Length];
            stream.Read(pdfBytes, 0, pdfBytes.Length);

            // Set the Content-Disposition header to inline, which displays the PDF in the browser
            Response.AppendHeader("Content-Disposition", "inline; filename=Report.pdf");

            // Return the PDF byte array to the view
            return File(pdfBytes, "application/pdf");
        }
        [HttpPost]
        public void AutoPrintSingleReport(string response)
        {
            var model = JsonConvert.DeserializeObject<STRPRCDto>(response);

            ReportDocument report = new ReportDocument();
            report.Load(Server.MapPath(ReportConstants.WholeReport_SLBrandAndSLDescPath));

            report.SetDatabaseLogon("sa", "@dm1n@8800");
            report.SetParameterValue("sku", model.O3SKU.ToString());
            report.SetParameterValue("user", Session["Username"].ToString());

            string printerName = ConfigurationManager.AppSettings["ReportPrinter"];

            PrinterSettings printerSettings = new PrinterSettings();
            printerSettings.PrinterName = printerName; 

            // Print the report directly to the printer without showing the printer settings or preview
            report.PrintOptions.PrinterName = printerSettings.PrinterName;
            report.PrintToPrinter(printerSettings, new PageSettings(), false);

            // Dispose the report object after printing
            report.Close();
            report.Dispose();

        }

        #region FOR RDLC REPORT
        public ActionResult AutoPrintReport(STRPRCDto model)
        {
            var reportData = GetData(model.O3SKU);
            var country = "";
            var countryImgData = new CountryDto();

            var reportPath = "";

            if (model.SelectedSizeId == ReportConstants.Size.Whole)
            {
                reportPath = Server.MapPath(ReportConstants.ApplianceReportPath);
            }
            else if (model.SelectedSizeId == ReportConstants.Size.Half)
            {
                reportPath = Server.MapPath(ReportConstants.ApplianceReportPath_Half);
            }
            else if (model.SelectedSizeId == ReportConstants.Size.Jewelry)
            {
                reportPath = Server.MapPath(ReportConstants.ApplianceReportPath_Jewelry);
            }
            else if (model.SelectedSizeId == ReportConstants.Size.Skinny)
            {
                reportPath = Server.MapPath(ReportConstants.ApplianceReportPath_Skinny);
            }

            var localReport = new LocalReport();
            localReport.ReportPath = reportPath;
            localReport.EnableExternalImages = true;

            List<ReportParameter> parameters = new List<ReportParameter>();
            parameters.Add(new ReportParameter("TypeId", model.SelectedTypeId.ToString()));
            parameters.Add(new ReportParameter("CategoryId", model.SelectedCategoryId.ToString()));
            localReport.SetParameters(parameters);

            var dataSource = new ReportDataSource("STRPRCDS", reportData);
            localReport.DataSources.Add(dataSource);

            // IF REPORT HAS NO FLAG
            foreach (var item2 in reportData)
            {
                if (!String.IsNullOrEmpty(item2.O3TRB3))
                {
                    country = reportData.FirstOrDefault().O3TRB3;
                    countryImgData = GetCountryImg(country);

                    DataTable countryDt = ConversionHelper.ConvertObjectToDataTable(countryImgData);

                    var dataSource2 = new ReportDataSource("CountryImageDS", countryDt);
                    localReport.DataSources.Add(dataSource2);
                }
                else
                {
                    var dataSource2 = new ReportDataSource("CountryImageDS", new DataTable());
                    localReport.DataSources.Add(dataSource2);
                }
            }

            var reportType = "PDF";
            var mimeType = "";
            var encoding = "";
            var fileNameExtension = "";

            var deviceInfo =
                $"<DeviceInfo><OutputFormat>{reportType}</OutputFormat><EmbedFonts>None</EmbedFonts></DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            ViewBag.ReportData = Convert.ToBase64String(renderedBytes); // Pass rendered report bytes to the view
            return View();
        }

        public ActionResult AutoPrintReportFromPCA(string response)
        {
            var model = JsonConvert.DeserializeObject<STRPRCDto>(response);

            var reportData = GetData(model.O3SKU);
            var country = "";
            var countryImgData = new CountryDto();

            var reportPath = "";

            if (model.SelectedSizeId == ReportConstants.Size.Whole)
            {
                reportPath = Server.MapPath(ReportConstants.ApplianceReportPath);
            }
            else if (model.SelectedSizeId == ReportConstants.Size.Half)
            {
                reportPath = Server.MapPath(ReportConstants.ApplianceReportPath_Half);
            }
            else if (model.SelectedSizeId == ReportConstants.Size.Jewelry)
            {
                reportPath = Server.MapPath(ReportConstants.ApplianceReportPath_Jewelry);
            }
            else if (model.SelectedSizeId == ReportConstants.Size.Skinny)
            {
                reportPath = Server.MapPath(ReportConstants.ApplianceReportPath_Skinny);
            }

            var localReport = new LocalReport();
            localReport.ReportPath = reportPath;
            localReport.EnableExternalImages = true;

            List<ReportParameter> parameters = new List<ReportParameter>();
            parameters.Add(new ReportParameter("TypeId", model.SelectedTypeId.ToString()));
            parameters.Add(new ReportParameter("CategoryId", model.SelectedCategoryId.ToString()));
            localReport.SetParameters(parameters);

            var dataSource = new ReportDataSource("STRPRCDS", reportData);
            localReport.DataSources.Add(dataSource);

            // IF REPORT HAS NO FLAG
            foreach (var item2 in reportData)
            {
                if (!String.IsNullOrEmpty(item2.O3TRB3))
                {
                    country = reportData.FirstOrDefault().O3TRB3;
                    countryImgData = GetCountryImg(country);

                    DataTable countryDt = ConversionHelper.ConvertObjectToDataTable(countryImgData);

                    var dataSource2 = new ReportDataSource("CountryImageDS", countryDt);
                    localReport.DataSources.Add(dataSource2);
                }
                else
                {
                    var dataSource2 = new ReportDataSource("CountryImageDS", new DataTable());
                    localReport.DataSources.Add(dataSource2);
                }
            }

            var reportType = "PDF";
            var mimeType = "";
            var encoding = "";
            var fileNameExtension = "";

            var deviceInfo =
                $"<DeviceInfo><OutputFormat>{reportType}</OutputFormat><EmbedFonts>None</EmbedFonts></DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            ViewBag.ReportData = Convert.ToBase64String(renderedBytes); // Pass rendered report bytes to the view
            return View();
        }

        public ActionResult PreviewRDLCReport()
        {
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/Report_Whole123.rdlc");

            var reportType = "PDF";
            var mimeType = "";
            var encoding = "";
            var fileNameExtension = "";

            var deviceInfo = $@"
                <DeviceInfo>
                    <OutputFormat>{reportType}</OutputFormat>
                    <EmbedFonts>None</EmbedFonts>
                </DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            return File(renderedBytes, "application/pdf");
        }

        public ActionResult DisplayReport(STRPRCDto model)
        {
            var reportData = GetData(model.O3SKU);
            var country = "";
            var countryImgData = new CountryDto();

            var reportPath = "";

            if (model.SelectedSizeId == ReportConstants.Size.Whole)
            {
                reportPath = Server.MapPath(ReportConstants.ApplianceReportPath);
            }
            else if (model.SelectedSizeId == ReportConstants.Size.Half)
            {
                reportPath = Server.MapPath(ReportConstants.ApplianceReportPath_Half);
            }
            else if (model.SelectedSizeId == ReportConstants.Size.Jewelry)
            {
                reportPath = Server.MapPath(ReportConstants.ApplianceReportPath_Jewelry);
            }
            else if (model.SelectedSizeId == ReportConstants.Size.Skinny)
            {
                reportPath = Server.MapPath(ReportConstants.ApplianceReportPath_Skinny);
            }

            var localReport = new LocalReport();
            localReport.ReportPath = reportPath;
            localReport.EnableExternalImages = true;

            List<ReportParameter> parameters = new List<ReportParameter>();
            parameters.Add(new ReportParameter("TypeId", model.SelectedTypeId.ToString()));
            parameters.Add(new ReportParameter("CategoryId", model.SelectedCategoryId.ToString()));
            localReport.SetParameters(parameters);

            var dataSource = new ReportDataSource("STRPRCDS", reportData);
            localReport.DataSources.Add(dataSource);


            // IF REPORT HAS NO FLAG
            foreach (var item2 in reportData)
            {
                if (!String.IsNullOrEmpty(item2.O3TRB3))
                {
                    country = reportData.FirstOrDefault().O3TRB3;
                    countryImgData = GetCountryImg(country);

                    DataTable countryDt = ConversionHelper.ConvertObjectToDataTable(countryImgData);

                    var dataSource2 = new ReportDataSource("CountryImageDS", countryDt);
                    localReport.DataSources.Add(dataSource2);
                }
                else
                {
                    var dataSource2 = new ReportDataSource("CountryImageDS", new DataTable());
                    localReport.DataSources.Add(dataSource2);
                }
            }

            var reportType = "PDF";
            var mimeType = "";
            var encoding = "";
            var fileNameExtension = "";

            var deviceInfo =
                $"<DeviceInfo><OutputFormat>{reportType}</OutputFormat><EmbedFonts>None</EmbedFonts></DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            return File(renderedBytes, mimeType);
        }

        public ActionResult DisplayReportFromAdvancePrinting(string response)
        {
            var dto = JsonConvert.DeserializeObject<STRPRCDto>(response);
            var countryImgData = new CountryDto();
            var reportPath = "";

            if (dto.SelectedSizeId == ReportConstants.Size.Whole)
            {
                reportPath = Server.MapPath(ReportConstants.ApplianceReportPath);
            }
            else if (dto.SelectedSizeId == ReportConstants.Size.Half)
            {
                reportPath = Server.MapPath(ReportConstants.ApplianceReportPath_Half);
            }
            else if (dto.SelectedSizeId == ReportConstants.Size.Jewelry)
            {
                reportPath = Server.MapPath(ReportConstants.ApplianceReportPath_Jewelry);
            }
            else if (dto.SelectedSizeId == ReportConstants.Size.Skinny)
            {
                reportPath = Server.MapPath(ReportConstants.ApplianceReportPath_Skinny);
            }

            var localReport = new LocalReport();
            localReport.ReportPath = reportPath;
            localReport.EnableExternalImages = true;


            List<ReportParameter> parameters = new List<ReportParameter>();
            parameters.Add(new ReportParameter("TypeId", dto.SelectedTypeId.ToString()));
            parameters.Add(new ReportParameter("CategoryId", dto.SelectedCategoryId.ToString()));
            localReport.SetParameters(parameters);

            DataTable dataTable = ConversionHelper.ConvertObjectToDataTable(dto);

            var dataSource = new ReportDataSource("STRPRCDS", dataTable);
            localReport.DataSources.Add(dataSource);

            // IF REPORT HAS NO FLAG
            if (!String.IsNullOrEmpty(dto.O3TRB3))
            {
                countryImgData = GetCountryImg(dto.O3TRB3);

                var dataSource2 = new ReportDataSource("CountryImageDS", countryImgData);
                localReport.DataSources.Add(dataSource2);
            }
            else
            {
                var dataSource2 = new ReportDataSource("CountryImageDS", new DataTable());
                localReport.DataSources.Add(dataSource2);
            }

            var reportType = "PDF";
            var mimeType = "";
            var encoding = "";
            var fileNameExtension = "";

            var deviceInfo =
                $"<DeviceInfo><OutputFormat>{reportType}</OutputFormat><EmbedFonts>None</EmbedFonts></DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            return File(renderedBytes, mimeType);
        }
        #endregion
    }
}