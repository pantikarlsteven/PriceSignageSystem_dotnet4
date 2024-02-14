using CrystalDecisions.CrystalReports.Engine;
using PdfiumViewer;
using PriceSignageSystem.Code;
using PriceSignageSystem.Code.CustomValidations;
using PriceSignageSystem.Helper;
using PriceSignageSystem.Models;
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
    public class QueueController : Controller
    {
        private readonly ISTRPRCRepository _sTRPRCRepository;
        private readonly IQueueRepository _queueRepository;
        private readonly string _printerName;
        private readonly string defaultPDFViewerLocation;
        public QueueController(IQueueRepository queueRepository, ISTRPRCRepository sTRPRCRepository)
        {
            _sTRPRCRepository = sTRPRCRepository;
            _queueRepository = queueRepository;
            _printerName = ConfigurationManager.AppSettings["ReportPrinter"];
            defaultPDFViewerLocation = ConfigurationManager.AppSettings["DefaultPDFViewerLocation"];
        }

        // GET: Queue
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult QueueItem(STRPRCDto dto)
        {
            _queueRepository.AddItemQueue(dto);
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult QueueSelectedItems(decimal[] selectedRows)
        {
            _queueRepository.QueueMultipleItems(selectedRows);
            return Json(new { success = true });
        }

        public ActionResult PrintQueueList(int sizeId)
        {
            var username = User.Identity.Name;
            var data = _queueRepository.GetQueueListPerUser(username).Where(a => a.SizeId == sizeId && a.Status == ReportConstants.Status.InQueue);
            if (!data.Any())
            {
                //Return an error message
                return Json(new { success = false });
            }
            ReportDocument report = new ReportDocument();
            var reportPath = string.Empty;

            switch (sizeId)
            {
                case ReportConstants.Size.Whole:
                    reportPath = Server.MapPath(ReportConstants.Dynamic_WholeReportPath);
                    break;
                case ReportConstants.Size.OneEight:
                    reportPath = Server.MapPath(ReportConstants.Dynamic_OneEightReportPath);
                    break;
                case ReportConstants.Size.Jewelry:
                    reportPath = Server.MapPath(ReportConstants.Dynamic_JewelryReportPath);
                    break;
            }
            report.Load(reportPath);
            report.SetDataSource(ConversionHelper.ConvertListToDataTable(data));

            report.PrintOptions.PrinterName = ConfigurationManager.AppSettings["ReportPrinter"];
            report.PrintToPrinter(1, true, 0, 0);

            //Update Item Queue status to Printed
            _queueRepository.UpdateStatus(data);

            report.Close();
            report.Dispose();

            return Json(new { success = true });
        }

        [HttpGet]
        public ActionResult PrintPreviewQueueReport(int sizeId)
        {
            //var isSuccess = true;
            try
            {
                var username = User.Identity.Name;
                var data = _queueRepository.GetQueueListPerUser(username).Where(a =>/* a.SizeId == sizeId && */a.Status == ReportConstants.Status.InQueue);
                foreach (var item in data)
                {
                    
                    item.UserName = User.Identity.Name;
                    var textToImage = new TextToImage();
                    textToImage.GetImageWidth(item.O3FNAM, item.O3IDSC, sizeId);
                    item.IsSLBrand = textToImage.IsSLBrand;
                    item.IsSLDescription = textToImage.IsSLDescription;
                    item.IsBiggerFont = textToImage.IsBiggerFont;
                    item.O3SDSC = _sTRPRCRepository.GetSubClassDescription(item.O3SKU);
                    item.O3REGU = item.qRegularPrice != 0 ? item.qRegularPrice : item.O3REGU;
                    item.O3POS = item.qCurrentPrice != 0 ? item.qCurrentPrice : item.O3POS;
                    item.O3IDSC = item.qItemDesc != null ? item.qItemDesc : item.O3IDSC;
                    item.O3FNAM = item.qBrand != null ? item.qBrand : item.O3FNAM;
                    item.O3MODL = item.qModel != null ? item.qModel : item.O3MODL;
                    item.O3DIV = item.qDivisor != null ? item.qDivisor : item.O3DIV;
                    item.TypeId = item.qTypeId != 0 ? item.qTypeId : item.TypeId;
                    item.O3TUOM = !string.IsNullOrEmpty(item.qTuom) ? item.qTuom : item.O3TUOM;
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

                //report.Close();
                //report.Dispose();
                //_queueRepository.UpdateStatus(data);
                #endregion

                Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                var pdfBytes = new byte[stream.Length];
                stream.Read(pdfBytes, 0, pdfBytes.Length);

                _queueRepository.UpdateStatus(data);
                Response.AppendHeader("Content-Disposition", "inline; filename=QueueReport.pdf");
                report.Close();
                report.Dispose();
                return File(pdfBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                // Handle the case when no row IDs are selected
                return Content("<h2>Error:" + ex.Message + "</h2>", "text/html");
                //isSuccess = false;
            }
            //return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CheckItemQueuesPerUser(int sizeId)
        {
            var username = User.Identity.Name;
            var data = _queueRepository.GetQueueListPerUser(username).Where(a => /*a.SizeId == sizeId && */a.Status == ReportConstants.Status.InQueue);
            if (!data.Any())
            {
                //Return an error message
                return Json(new { success = false });
            }
            return Json(new { success = true });

        }

        [HttpPost]
        public ActionResult RequeueItem(int id)
        {
            var username = User.Identity.Name;

            var count = _queueRepository.RequeueItem(id, username);

            if(count > 0)
            {
                return Json(new { success = true});
            }

            return Json(new { success = false });
        }
    }
}