using Microsoft.Reporting.WebForms;
using PriceSignageSystem.Helper;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PriceSignageSystem.Controllers
{
    public class QueueController : Controller
    {
        private readonly IQueueRepository _queueRepository;
        private readonly ISTRPRCRepository _sTRPRCRepository;
        public QueueController(IQueueRepository queueRepository, ISTRPRCRepository sTRPRCRepository)
        {
            _queueRepository = queueRepository;
            _sTRPRCRepository = sTRPRCRepository;
        }

        // GET: Queue
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult QueueItem(STRPRCDto strprcDto, int sizeId, int typeId, int categoryId)
        {
            strprcDto.SelectedCategoryId = categoryId;
            strprcDto.SelectedSizeId = sizeId;
            strprcDto.SelectedTypeId = typeId;
            var data = _queueRepository.AddItemQueue(strprcDto);
            return Json(new { success = true });
        }

    
        public ActionResult PrintQueueList()
        {
            var reportData = _sTRPRCRepository.GetDataBySKU(1002);
            DataTable dataTable = ConversionHelper.ConvertObjectToDataTable(reportData);

            var username = (string)System.Web.HttpContext.Current.Session["Username"];

            var queueList = _queueRepository.GetQueueListPerUser(username);
            var queueListWhole = queueList.Where(a => a.SizeId == 1);
            var queueListHalf = queueList.Where(a => a.SizeId == 2);
            var queueListJewelry = queueList.Where(a => a.SizeId == 3);
            var queueListSkinny = queueList.Where(a => a.SizeId == 4);

            var localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/DynamicQueueReport.rdlc");

            var dataSource = new ReportDataSource("STRPRCDS_Whole", queueListWhole);
            localReport.DataSources.Add(dataSource);
            var dataSource2 = new ReportDataSource("STRPRCDS_Half", queueListHalf);
            localReport.DataSources.Add(dataSource2);
            var dataSource3 = new ReportDataSource("STRPRCDS_Jewelry", queueListJewelry);
            localReport.DataSources.Add(dataSource3);
            var dataSource4 = new ReportDataSource("STRPRCDS_Skinny", queueListSkinny);
            localReport.DataSources.Add(dataSource4);

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

            ViewBag.ReportQueueData = Convert.ToBase64String(renderedBytes); // Pass rendered report bytes to the view
            return View();
        }
    }
}