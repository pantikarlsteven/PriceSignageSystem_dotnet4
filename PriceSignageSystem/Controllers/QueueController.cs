using Microsoft.Reporting.WebForms;
using PriceSignageSystem.Helper;
using PriceSignageSystem.Models.Constants;
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

    
        public ActionResult PrintQueueList(string size)
        {
            
            var username = (string)System.Web.HttpContext.Current.Session["Username"];
            var queueList = _queueRepository.GetQueueListPerUser(username).Where(a => a.SizeId.ToString() == size);
            var reportPath = "";

            if (Convert.ToInt32(size) == ReportConstants.Size.Whole)
            {
                reportPath = Server.MapPath(ReportConstants.DynamicQueueReportPath);
            }
            else if (Convert.ToInt32(size) == ReportConstants.Size.Half)
            {
                reportPath = Server.MapPath(ReportConstants.DynamicQueueReportPath_Half);
            }
            else if (Convert.ToInt32(size) == ReportConstants.Size.Jewelry)
            {
                reportPath = Server.MapPath(ReportConstants.DynamicQueueReportPath_Jewelry);
            }
            //else if (Convert.ToInt32(size) == ReportConstants.Size.Skinny)
            //{
            //    reportPath = Server.MapPath(ReportConstants.ApplianceReportPath_Skinny);
            //}

            var localReport = new LocalReport();
            localReport.ReportPath = reportPath;

            var dataSource = new ReportDataSource("STRPRCDS_Queue", queueList);
            localReport.DataSources.Add(dataSource);
     
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