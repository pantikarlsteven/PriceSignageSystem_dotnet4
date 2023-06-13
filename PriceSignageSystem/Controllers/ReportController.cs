using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using PriceSignageSystem.Code;
using PriceSignageSystem.Helper;
using PriceSignageSystem.Models.Constants;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
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
            report.Load(Server.MapPath(ReportConstants.Dynamic_WholeReportPath));
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
                data.TypeId = model.SelectedTypeId;
                data.SizeId = model.SelectedSizeId;
                data.CategoryId = model.SelectedCategoryId;
            var dataTable = ConversionHelper.ConvertObjectToDataTable(data);
            var reportPath = "";

            if (model.SelectedSizeId == ReportConstants.Size.Whole)
            {
                reportPath = Server.MapPath(ReportConstants.Dynamic_WholeReportPath);
            }
            else if (model.SelectedSizeId == ReportConstants.Size.OneEight)
            {
                reportPath = Server.MapPath(ReportConstants.Dynamic_OneEightReportPath);
            }
            else if (model.SelectedSizeId == ReportConstants.Size.Skinny)
            {
                reportPath = Server.MapPath(ReportConstants.Dynamic_SkinnyReportPath);
            }
            else if (model.SelectedSizeId == ReportConstants.Size.Jewelry)
            {
                reportPath = Server.MapPath(ReportConstants.Dynamic_JewelryReportPath);
            }

            ReportDocument report = new ReportDocument();
            report.Load(reportPath);

            report.SetDataSource(dataTable);

            PrinterSettings printerSettings = new PrinterSettings();
            printerSettings.PrinterName = _printerName; 
            report.PrintOptions.PrinterName = _printerName;
            report.PrintToPrinter(1, true, 0, 0);

            report.Close();
            report.Dispose();
        }

        [HttpGet]
        public ActionResult PrintPreviewSingleReport(string response)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<ReportDto>(response);
                ReportDocument report = new ReportDocument();
                var path = string.Empty;

                switch (model.SizeId)
                {
                    case ReportConstants.Size.Whole:
                        path = Server.MapPath(ReportConstants.Dynamic_WholeReportPath);
                        break;
                    case ReportConstants.Size.OneEight:
                        path = Server.MapPath(ReportConstants.Dynamic_OneEightReportPath);
                        break;
                    case ReportConstants.Size.Jewelry:
                        path = Server.MapPath(ReportConstants.Dynamic_JewelryReportPath);
                        break;
                    case ReportConstants.Size.Skinny:
                        path = Server.MapPath(ReportConstants.Dynamic_SkinnyReportPath);
                        break;
                }

                report.Load(path);
                var skuModel = _sTRPRCRepository.GetReportData(model.O3SKU);
                skuModel.TypeId = model.TypeId;
                skuModel.CategoryId = model.CategoryId;
                skuModel.UserName = Session["Username"].ToString();

                report.SetDataSource(ConversionHelper.ConvertObjectToDataTable(skuModel));

                Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                var pdfBytes = new byte[stream.Length];
                stream.Read(pdfBytes, 0, pdfBytes.Length);

                Response.AppendHeader("Content-Disposition", "inline; filename=" + model.O3SKU.ToString() + ".pdf");
                return File(pdfBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                Logs.WriteToFile(ex.Message);
                return Content("<h2>Error: " + ex.Message + "</h2>", "text/html");
            }
        }

        [HttpGet]
        public ActionResult PrintPreviewMultipleReport(string[] selectedIds, int sizeId)
        {
            if (selectedIds != null && selectedIds.Length > 0)
            {
                List<decimal> o3skus = selectedIds[0].Split(',').Select(decimal.Parse).ToList();
                var data = _sTRPRCRepository.GetReportDataList(o3skus);
                foreach (var item in data)
                {
                    item.UserName = Session["Username"].ToString();
                }
                var dataTable = ConversionHelper.ConvertListToDataTable(data);
                var reportPath = string.Empty;

                if (sizeId == ReportConstants.Size.Whole)
                {
                    reportPath = Server.MapPath(ReportConstants.Dynamic_WholeReportPath);
                }
                else if (sizeId == ReportConstants.Size.OneEight)
                {
                    reportPath = Server.MapPath(ReportConstants.Dynamic_OneEightReportPath);
                }
                else if (sizeId == ReportConstants.Size.Skinny)
                {
                    reportPath = Server.MapPath(ReportConstants.Dynamic_SkinnyReportPath);
                }
                else if (sizeId == ReportConstants.Size.Jewelry)
                {
                    reportPath = Server.MapPath(ReportConstants.Dynamic_JewelryReportPath);
                }

                ReportDocument report = new ReportDocument();
                report.Load(reportPath);
                report.SetDataSource(dataTable);

                Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                var pdfBytes = new byte[stream.Length];
                stream.Read(pdfBytes, 0, pdfBytes.Length);

                Response.AppendHeader("Content-Disposition", "inline; filename=MultipleSKUs.pdf");
                return File(pdfBytes, "application/pdf");
            }
            else
            {
                // Handle the case when no row IDs are selected
                return Content("<h2>Error: No rows have found.</h2>", "text/html");
            }
        }

        [HttpPost]
        public void AutoPrintMultipleReport(string[] selectedIds, int sizeId, int typeId, int categoryId)
        {
            if (selectedIds != null && selectedIds.Length > 0)
            {
                var reportDto = new ReportDto();
                var printList = new List<ReportDto>();

                foreach (var rowId in selectedIds)
                {
                    var o3sku = decimal.Parse(rowId);
                    reportDto = _sTRPRCRepository.GetReportData(o3sku);
                    reportDto.UserName = Session["Username"].ToString();

                    printList.Add(reportDto);
                }

                var dataTable = ConversionHelper.ConvertListToDataTable(printList);
                var reportPath = string.Empty;

                if (sizeId == ReportConstants.Size.Whole)
                {
                    reportPath = Server.MapPath(ReportConstants.Dynamic_WholeReportPath);
                }
                else if (sizeId == ReportConstants.Size.OneEight)
                {
                    reportPath = Server.MapPath(ReportConstants.Dynamic_OneEightReportPath);
                }
                else if (sizeId == ReportConstants.Size.Skinny)
                {
                    reportPath = Server.MapPath(ReportConstants.Dynamic_SkinnyReportPath);
                }
                else if (sizeId == ReportConstants.Size.Jewelry)
                {
                    reportPath = Server.MapPath(ReportConstants.Dynamic_JewelryReportPath);
                }

                ReportDocument report = new ReportDocument();
                report.Load(reportPath);
                report.SetDataSource(dataTable);

                PrinterSettings printerSettings = new PrinterSettings();
                printerSettings.PrinterName = _printerName;
                report.PrintOptions.PrinterName = printerSettings.PrinterName;
                report.PrintToPrinter(1, true, 0, 0);
            }
            else
            {
                // Handle the case when no row IDs are selected
            }
        }

        [HttpPost]
        public ActionResult AutoPrintOnDemandReport(STRPRCDto model)
        {
            try
            {
                var data = _sTRPRCRepository.GetReportData(model.O3SKU);
                data.UserName = Session["Username"].ToString();
                var dataTable = ConversionHelper.ConvertObjectToDataTable(data);
                var reportPath = "";

                if (model.SelectedSizeId == ReportConstants.Size.Whole)
                {
                    reportPath = Server.MapPath(ReportConstants.Dynamic_WholeReportPath);
                }
                else if (model.SelectedSizeId == ReportConstants.Size.OneEight)
                {
                    reportPath = Server.MapPath(ReportConstants.Dynamic_OneEightReportPath);
                }
                else if (model.SelectedSizeId == ReportConstants.Size.Skinny)
                {
                    reportPath = Server.MapPath(ReportConstants.Dynamic_SkinnyReportPath);
                }
                else if (model.SelectedSizeId == ReportConstants.Size.Jewelry)
                {
                    reportPath = Server.MapPath(ReportConstants.Dynamic_JewelryReportPath);
                }

                ReportDocument report = new ReportDocument();
                report.Load(reportPath);
                report.SetDataSource(dataTable);

                PrinterSettings printerSettings = new PrinterSettings();
                //printerSettings.PrinterName = _printerName;
                report.PrintOptions.PrinterName = _printerName;
                report.PrintToPrinter(1, true, 0, 0);

                report.Close();
                report.Dispose();

            }
            catch (Exception ex)
            {
                Logs.WriteToFile(ex.InnerException.Message);
                Logs.WriteToFile(ex.Message);
            }
            return RedirectToAction("Index", "STRPRC");
        }

        #region FOR RDLC REPORT
        public ActionResult AutoPrintReport(STRPRCDto model)
        {
            var reportData = GetData(model.O3SKU);
            var country = "";
            var countryImgData = new CountryDto();

            var reportPath = "";

            //if (model.SelectedSizeId == ReportConstants.Size.Whole)
            //{
            //    reportPath = Server.MapPath(ReportConstants.ApplianceReportPath);
            //}
            //else if (model.SelectedSizeId == ReportConstants.Size.Half)
            //{
            //    reportPath = Server.MapPath(ReportConstants.ApplianceReportPath_Half);
            //}
            //else if (model.SelectedSizeId == ReportConstants.Size.Jewelry)
            //{
            //    reportPath = Server.MapPath(ReportConstants.ApplianceReportPath_Jewelry);
            //}
            //else if (model.SelectedSizeId == ReportConstants.Size.Skinny)
            //{
            //    reportPath = Server.MapPath(ReportConstants.ApplianceReportPath_Skinny);
            //}

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

            //if (model.SelectedSizeId == ReportConstants.Size.Whole)
            //{
            //    reportPath = Server.MapPath(ReportConstants.ApplianceReportPath);
            //}
            //else if (model.SelectedSizeId == ReportConstants.Size.Half)
            //{
            //    reportPath = Server.MapPath(ReportConstants.ApplianceReportPath_Half);
            //}
            //else if (model.SelectedSizeId == ReportConstants.Size.Jewelry)
            //{
            //    reportPath = Server.MapPath(ReportConstants.ApplianceReportPath_Jewelry);
            //}
            //else if (model.SelectedSizeId == ReportConstants.Size.Skinny)
            //{
            //    reportPath = Server.MapPath(ReportConstants.ApplianceReportPath_Skinny);
            //}

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

            //if (model.SelectedSizeId == ReportConstants.Size.Whole)
            //{
            //    reportPath = Server.MapPath(ReportConstants.ApplianceReportPath);
            //}
            //else if (model.SelectedSizeId == ReportConstants.Size.Half)
            //{
            //    reportPath = Server.MapPath(ReportConstants.ApplianceReportPath_Half);
            //}
            //else if (model.SelectedSizeId == ReportConstants.Size.Jewelry)
            //{
            //    reportPath = Server.MapPath(ReportConstants.ApplianceReportPath_Jewelry);
            //}
            //else if (model.SelectedSizeId == ReportConstants.Size.Skinny)
            //{
            //    reportPath = Server.MapPath(ReportConstants.ApplianceReportPath_Skinny);
            //}

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

            //if (dto.SelectedSizeId == ReportConstants.Size.Whole)
            //{
            //    reportPath = Server.MapPath(ReportConstants.ApplianceReportPath);
            //}
            //else if (dto.SelectedSizeId == ReportConstants.Size.Half)
            //{
            //    reportPath = Server.MapPath(ReportConstants.ApplianceReportPath_Half);
            //}
            //else if (dto.SelectedSizeId == ReportConstants.Size.Jewelry)
            //{
            //    reportPath = Server.MapPath(ReportConstants.ApplianceReportPath_Jewelry);
            //}
            //else if (dto.SelectedSizeId == ReportConstants.Size.Skinny)
            //{
            //    reportPath = Server.MapPath(ReportConstants.ApplianceReportPath_Skinny);
            //}

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