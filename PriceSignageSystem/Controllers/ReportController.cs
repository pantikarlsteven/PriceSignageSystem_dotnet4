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
        private readonly string _dbUsername;
        private readonly string _dbPassword;
        private readonly string _printerName;
        public ReportController(ISTRPRCRepository sTRPRCRepository)
        {
            _sTRPRCRepository = sTRPRCRepository;
            _dbUsername = ConfigurationManager.AppSettings["DbUserName"];
            _dbPassword = ConfigurationManager.AppSettings["DbPassword"];
            _printerName = ConfigurationManager.AppSettings["ReportPrinter"];
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
            var o3sku = decimal.Parse(id);
            var data = _sTRPRCRepository.GetReportData(o3sku);
            data.UserName = Session["Username"].ToString();
            var dataTable = ConversionHelper.ConvertObjectToDataTable(data);

            ReportDocument report = new ReportDocument();
            report.Load(Server.MapPath(ReportConstants.WholeReport_SLBrandAndSLDescPath));
            report.SetDatabaseLogon(_dbUsername, _dbPassword);
            report.SetDataSource(dataTable);

            Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
            byte[] pdfBytes = new byte[stream.Length];
            stream.Read(pdfBytes, 0, pdfBytes.Length);

            Response.AppendHeader("Content-Disposition", "inline; filename=Report.pdf");

            return File(pdfBytes, "application/pdf");
        }
        [HttpPost]
        public void AutoPrintSingleReport(string response)
        {
            var model = JsonConvert.DeserializeObject<STRPRCDto>(response);
            var data = _sTRPRCRepository.GetReportData(model.O3SKU);
            data.UserName = Session["Username"].ToString();
            var dataTable = ConversionHelper.ConvertObjectToDataTable(data);

            ReportDocument report = new ReportDocument();
            report.Load(Server.MapPath(ReportConstants.WholeReport_SLBrandAndSLDescPath));

            report.SetDatabaseLogon(_dbUsername, _dbPassword);
            report.SetDataSource(dataTable);

            PrinterSettings printerSettings = new PrinterSettings();
            printerSettings.PrinterName = _printerName; 
            report.PrintOptions.PrinterName = printerSettings.PrinterName;
            report.PrintToPrinter(printerSettings, new PageSettings(), false);

            report.Close();
            report.Dispose();
        }

        [HttpPost]
        public void AutoPrintMultipleReport(string[] selectedIds)
        {
            if (selectedIds != null && selectedIds.Length > 0)
            {
                foreach (var rowId in selectedIds)
                {
                    var o3sku = decimal.Parse(rowId);
                    var data = _sTRPRCRepository.GetReportData(o3sku);
                    data.UserName = Session["Username"].ToString();
                    var dataTable = ConversionHelper.ConvertObjectToDataTable(data);

                    ReportDocument report = new ReportDocument();
                    report.Load(Server.MapPath(ReportConstants.WholeReport_SLBrandAndSLDescPath));
                
                    report.SetDatabaseLogon(_dbUsername, _dbPassword);
                    report.SetDataSource(dataTable);

                    PrinterSettings printerSettings = new PrinterSettings();
                    printerSettings.PrinterName = _printerName;
                    report.PrintOptions.PrinterName = printerSettings.PrinterName;
                    report.PrintToPrinter(printerSettings, new PageSettings(), false);
                }
            }
            else
            {
                // Handle the case when no row IDs are selected
            }
        }

        [HttpPost]
        public ActionResult AutoPrintOnDemandReport(STRPRCDto model)
        {
            var data = _sTRPRCRepository.GetReportData(model.O3SKU);
            data.UserName = Session["Username"].ToString();
            var dataTable = ConversionHelper.ConvertObjectToDataTable(data);
            var reportPath = "";

            if (model.SelectedSizeId == ReportConstants.Size.Whole)
            {
                if (data.O3FNAM.Length <= 12 && data.O3IDSC.Length <= 44)
                {
                    reportPath = Server.MapPath(ReportConstants.WholeReport_SLBrandAndSLDescPath);
                }
                else if (data.O3FNAM.Length > 12 && data.O3IDSC.Length > 44)
                {
                    reportPath = Server.MapPath(ReportConstants.WholeReport_DLBrandAndDLDescPath);

                }
                else if (data.O3FNAM.Length <= 12 && data.O3IDSC.Length > 44)
                {
                    reportPath = Server.MapPath(ReportConstants.WholeReport_SLBrandAndDLDescPath);

                }
                else if (data.O3FNAM.Length > 12 && data.O3IDSC.Length <= 44)
                {
                    reportPath = Server.MapPath(ReportConstants.WholeReport_DLBrandAndSLDescPath);

                }
            }

            ReportDocument report = new ReportDocument();
            report.Load(reportPath);
            report.SetDatabaseLogon(_dbUsername, _dbPassword);
            report.SetDataSource(dataTable);

            PrinterSettings printerSettings = new PrinterSettings();
            printerSettings.PrinterName = _printerName;
            report.PrintOptions.PrinterName = printerSettings.PrinterName;
            report.PrintToPrinter(printerSettings, new PageSettings(), false);

            report.Close();
            report.Dispose();

            return RedirectToAction("Index", "STRPRC");
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