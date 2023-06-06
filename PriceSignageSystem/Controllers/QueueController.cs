using CrystalDecisions.CrystalReports.Engine;
using Microsoft.Reporting.WebForms;
using PriceSignageSystem.Helper;
using PriceSignageSystem.Models.Constants;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using System;
using System.Configuration;
using System.Data;
using System.Linq;
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

        public ActionResult PrintQueueList(int sizeId)
        {
            var username = (string)Session["Username"];
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
                case ReportConstants.Size.Half:
                    reportPath = Server.MapPath(ReportConstants.Dynamic_HalfReportPath);
                    break;
                case ReportConstants.Size.Jewelry:
                    reportPath = Server.MapPath(ReportConstants.Dynamic_JewelryReportPath);
                    break;
                case ReportConstants.Size.Skinny:
                    reportPath = Server.MapPath(ReportConstants.Dynamic_SkinnyReportPath);
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
    }
}