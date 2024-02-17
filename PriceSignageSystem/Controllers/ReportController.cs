using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Microsoft.Reporting.WebForms;
using Microsoft.Win32;
using Newtonsoft.Json;
using PdfiumViewer;
using PriceSignageSystem.Code;
using PriceSignageSystem.Code.CustomValidations;
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
    [CustomAuthorize]
    public class ReportController : Controller
    {
        private readonly ISTRPRCRepository _sTRPRCRepository;
        private readonly string _dbUsername;
        private readonly string _dbPassword;
        private readonly string _printerName;
        private readonly string defaultPDFViewerLocation;
        private readonly int storeId;
        private readonly string crystalReportPath;

        public ReportController(ISTRPRCRepository sTRPRCRepository)
        {
            _sTRPRCRepository = sTRPRCRepository;
            _dbUsername = ConfigurationManager.AppSettings["DbUserName"];
            _dbPassword = ConfigurationManager.AppSettings["DbPassword"];
            _printerName = ConfigurationManager.AppSettings["ReportPrinter"];
            defaultPDFViewerLocation = ConfigurationManager.AppSettings["DefaultPDFViewerLocation"];
            crystalReportPath = ConfigurationManager.AppSettings["CrystalReportPath"];
            storeId = int.Parse(ConfigurationManager.AppSettings["StoreID"]);
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

        [AllowAnonymous]
        public ActionResult PreviewCrystalReport(string id)
        {
            var o3sku = decimal.Parse(id);
            var data = _sTRPRCRepository.GetReportData(o3sku);
            data.UserName = User.Identity.Name;

            var textToImage = new TextToImage();
            textToImage.GetImageWidth(data.O3FNAM, data.O3IDSC, ReportConstants.Size.Whole);
            data.IsSLBrand = textToImage.IsSLBrand;
            data.IsSLDescription = textToImage.IsSLDescription;
            data.IsBiggerFont = textToImage.IsBiggerFont;

            var test = _sTRPRCRepository.GetReportData(11656);
            data.country_img = test.country_img;
            data.O3SDSC = _sTRPRCRepository.GetSubClassDescription(o3sku);
            var dataTable = ConversionHelper.ConvertObjectToDataTable(data);

            ReportDocument report = new ReportDocument();
            report.Load(Server.MapPath("~/Reports/CrystalReports/Dynamic_WholeReport.rpt"));
            report.SetDataSource(dataTable);

            string pdfPath = Server.MapPath("~/Reports/PDFs");
            Guid guid = Guid.NewGuid();
            var pdf = pdfPath + "\\" + guid + ".pdf";
            PDFConversion.ConvertCrystalReportToPDF(defaultPDFViewerLocation, report, pdfPath, pdf);

            Logs.WriteToFile("Installed Printers:");
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                Logs.WriteToFile(printer);
            }

            PrinterSettings printerSettings = new PrinterSettings()
            {
                PrinterName = _printerName,
                Copies = 1
            };

            PageSettings pageSettings = new PageSettings(printerSettings)
            {
                Margins = new Margins(0, 0, 0, 0)
            };

            foreach (System.Drawing.Printing.PaperSize paperSize in printerSettings.PaperSizes)
            {
                if (paperSize.PaperName == "Letter")
                {
                    pageSettings.PaperSize = paperSize;
                    break;
                }
            }

            using (PdfDocument pdfDocument = PdfDocument.Load(pdf))
            {
                using (PrintDocument printDocument = pdfDocument.CreatePrintDocument())
                {
                    printDocument.PrinterSettings = printerSettings;
                    printDocument.DefaultPageSettings = pageSettings;
                    printDocument.PrintController = (PrintController)new StandardPrintController();
                    //printDocument.Print();
                    Logs.WriteToFile("Start printing");
                }
            }

            Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
            byte[] pdfBytes = new byte[stream.Length];
            stream.Read(pdfBytes, 0, pdfBytes.Length);

            Response.AppendHeader("Content-Disposition", "inline; filename=Report.pdf");
            report.Close();
            report.Dispose();
            return File(pdfBytes, "application/pdf");
        }
        [HttpPost]
        public void AutoPrintSingleReport(string response)
        {
            var model = JsonConvert.DeserializeObject<STRPRCDto>(response);
            var data = _sTRPRCRepository.GetReportData(model.O3SKU);
                data.UserName = User.Identity.Name;
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
            //var isSuccess = true;
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
                }

                report.Load(path);
                var skuModel = _sTRPRCRepository.GetReportData(model.O3SKU);
                skuModel.TypeId = model.TypeId;
                skuModel.CategoryId = model.CategoryId;
                skuModel.UserName = User.Identity.Name;
                skuModel.O3SDSC = _sTRPRCRepository.GetSubClassDescription(model.O3SKU);
                skuModel.O3REGU = model.O3REGU != 0 ? model.O3REGU: skuModel.O3REGU;
                skuModel.O3POS = model.O3POS != 0 ? model.O3POS : skuModel.O3POS;
                skuModel.O3IDSC = !string.IsNullOrEmpty(model.O3IDSC) ? model.O3IDSC : skuModel.O3IDSC;
                skuModel.O3FNAM = !string.IsNullOrEmpty(model.O3FNAM) ? model.O3FNAM : skuModel.O3FNAM;
                skuModel.O3MODL = !string.IsNullOrEmpty(model.O3MODL) ? model.O3MODL : skuModel.O3MODL;
                skuModel.O3DIV = !string.IsNullOrEmpty(model.O3DIV) ? model.O3DIV : skuModel.O3DIV;
                skuModel.O3TUOM = !string.IsNullOrEmpty(model.O3TUOM) ? model.O3TUOM : skuModel.O3TUOM;

                var textToImage = new TextToImage();
                textToImage.GetImageWidth(skuModel.O3FNAM, skuModel.O3IDSC, model.SizeId);
                skuModel.IsSLBrand = textToImage.IsSLBrand;
                skuModel.IsSLDescription = textToImage.IsSLDescription;
                skuModel.IsBiggerFont = textToImage.IsBiggerFont;

                report.SetDataSource(ConversionHelper.ConvertObjectToDataTable(skuModel));
                Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                var pdfBytes = new byte[stream.Length];
                stream.Read(pdfBytes, 0, pdfBytes.Length);
                Response.AppendHeader("Content-Disposition", "inline; filename=" + model.O3SKU.ToString() + ".pdf");
                report.Close();
                report.Dispose();

                #region For Auto Printing
                //string pdfPath = Server.MapPath("~/Reports/PDFs");
                //Guid guid = Guid.NewGuid();
                //var pdf = pdfPath + "\\" + guid + ".pdf";
                //PDFConversion.ConvertCrystalReportToPDF(defaultPDFViewerLocation, report, pdfPath, pdf);

                //Logs.WriteToFile("Installed Printers:");
                //foreach (string printer in PrinterSettings.InstalledPrinters)
                //{
                //    Logs.WriteToFile(printer);
                //}

                //PrinterSettings printerSettings = new PrinterSettings()
                //{
                //    PrinterName = _printerName,
                //    Copies = 1
                //};

                //PageSettings pageSettings = new PageSettings(printerSettings)
                //{
                //    Margins = new Margins(0, 0, 0, 0)
                //};

                //foreach (System.Drawing.Printing.PaperSize paperSize in printerSettings.PaperSizes)
                //{
                //    if (paperSize.PaperName == "Letter")
                //    {
                //        pageSettings.PaperSize = paperSize;
                //        break;
                //    }
                //}

                //using (PdfDocument pdfDocument = PdfDocument.Load(pdf))
                //{
                //    using (PrintDocument printDocument = pdfDocument.CreatePrintDocument())
                //    {
                //        printDocument.PrinterSettings = printerSettings;
                //        printDocument.DefaultPageSettings = pageSettings;
                //        printDocument.PrintController = (PrintController)new StandardPrintController();
                //        printDocument.Print();
                //        Logs.WriteToFile("Start printing");
                //    }
                //}
                #endregion

                _sTRPRCRepository.UpdateSingleStatus(model.O3SKU);
                _sTRPRCRepository.AddInventoryPrintingLog(model, User.Identity.Name);

                return File(pdfBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                Logs.WriteToFile(ex.Message);
                return Content("<h2>Error: " + ex.Message + "</h2>", "text/html");
                //isSuccess = false;
            }
            //return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult PrintPreviewMultipleReport(string[] selectedIds, int sizeId)
        {
            //var isSuccess = true;
            try
            {
                if (selectedIds != null && selectedIds.Length > 0)
                {
                    List<decimal> o3skus = selectedIds[0].Split(',').Select(decimal.Parse).ToList();
                    var data = _sTRPRCRepository.GetReportDataList(o3skus);
                    foreach (var item in data)
                    {
                        item.UserName = User.Identity.Name;
                        var textToImage = new TextToImage();
                        textToImage.GetImageWidth(item.O3FNAM, item.O3IDSC, sizeId);
                        item.IsSLBrand = textToImage.IsSLBrand;
                        item.IsSLDescription = textToImage.IsSLDescription;
                        item.IsBiggerFont = textToImage.IsBiggerFont;
                        item.O3SDSC = _sTRPRCRepository.GetSubClassDescription(item.O3SKU);
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
                    report.Close();
                    report.Dispose();

                    #region For Auto Printing
                    //string pdfPath = Server.MapPath("~/Reports/PDFs");
                    //Guid guid = Guid.NewGuid();
                    //var pdf = pdfPath + "\\" + guid + ".pdf";
                    //PDFConversion.ConvertCrystalReportToPDF(defaultPDFViewerLocation, report, pdfPath, pdf);

                    //PrinterSettings printerSettings = new PrinterSettings()
                    //{
                    //    PrinterName = _printerName,
                    //    Copies = 1
                    //};

                    //PageSettings pageSettings = new PageSettings(printerSettings)
                    //{
                    //    Margins = new Margins(0, 0, 0, 0)
                    //};

                    //foreach (System.Drawing.Printing.PaperSize paperSize in printerSettings.PaperSizes)
                    //{
                    //    if (paperSize.PaperName == "Letter")
                    //    {
                    //        pageSettings.PaperSize = paperSize;
                    //        break;
                    //    }
                    //}

                    //using (PdfDocument pdfDocument = PdfDocument.Load(pdf))
                    //{
                    //    using (PrintDocument printDocument = pdfDocument.CreatePrintDocument())
                    //    {
                    //        printDocument.PrinterSettings = printerSettings;
                    //        printDocument.DefaultPageSettings = pageSettings;
                    //        printDocument.PrintController = (PrintController)new StandardPrintController();
                    //        printDocument.Print();
                    //        Logs.WriteToFile("Start printing");
                    //    }
                    //}
                    #endregion

                    _sTRPRCRepository.UpdateMultipleStatus(o3skus);
                    _sTRPRCRepository.AddMultipleInventoryPrintingLog(o3skus, User.Identity.Name);

                    return File(pdfBytes, "application/pdf");
                }
                else
                    throw new Exception("No Selected Id");
            }
            catch (Exception ex)
            {
                //isSuccess = false;
                return Content("<h2>Error: " + ex.Message + "</h2>", "text/html");
            }
            //return Json(isSuccess, JsonRequestBehavior.AllowGet);
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
                    reportDto.UserName = User.Identity.Name;

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

                report.Close();
                report.Dispose();
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
                data.UserName = User.Identity.Name;
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

                _sTRPRCRepository.UpdateSingleStatus(model.O3SKU);
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