using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using PriceSignageSystem.Helper;
using PriceSignageSystem.Models.Constants;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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

        public List<Models.STRPRC> GetData(decimal O3SKU)
        {
            var data = _sTRPRCRepository.GetData(O3SKU);
            return data;
        }

        public ActionResult DisplayReport(STRPRCDto model)
        {
            var reportPath = "";
            if(model.SelectedCategoryId == ReportConstants.Category.Appliance)
            {
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
            }

            else if (model.SelectedCategoryId == ReportConstants.Category.NonAppliance)
            {
                if (model.SelectedSizeId == ReportConstants.Size.Whole)
                {
                    reportPath = Server.MapPath(ReportConstants.NonApplianceReportPath);
                }
                else if (model.SelectedSizeId == ReportConstants.Size.Half)
                {
                    reportPath = Server.MapPath(ReportConstants.NonApplianceReportPath_Half);
                }
                else if (model.SelectedSizeId == ReportConstants.Size.Jewelry)
                {
                    reportPath = Server.MapPath(ReportConstants.NonApplianceReportPath_Jewelry);
                }
                else if (model.SelectedSizeId == ReportConstants.Size.Skinny)
                {
                    reportPath = Server.MapPath(ReportConstants.NonApplianceReportPath_Skinny);
                }
            }

            var localReport = new LocalReport();
            localReport.ReportPath = reportPath;

            List<ReportParameter> parameters = new List<ReportParameter>();
            parameters.Add(new ReportParameter("TypeId", model.SelectedTypeId.ToString()));
            localReport.SetParameters(parameters);

            var reportData = GetData(model.O3SKU);


            var dataSource = new ReportDataSource("STRPRCDS", reportData);
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

            return File(renderedBytes, mimeType);
        }

        public ActionResult DisplayReportFromAdvancePrinting(string response)
        {
            var dto  = JsonConvert.DeserializeObject<STRPRCDto>(response);

            var reportPath = "";
            if (dto.SelectedCategoryId == ReportConstants.Category.Appliance)
            {
                if (dto.SelectedSizeId == ReportConstants.Size.Whole)
                {
                    reportPath = Server.MapPath(ReportConstants.ApplianceReportPath);
                }

                else if (dto.SelectedSizeId == ReportConstants.Size.Half)
                {
                    reportPath = Server.MapPath(ReportConstants.ApplianceReportPath_Half);
                }
            }

            else if (dto.SelectedCategoryId == ReportConstants.Category.NonAppliance)
            {
                if (dto.SelectedSizeId == ReportConstants.Size.Whole)
                {
                    reportPath = Server.MapPath(ReportConstants.NonApplianceReportPath);
                }

                else if (dto.SelectedSizeId == ReportConstants.Size.Half)
                {
                    reportPath = Server.MapPath(ReportConstants.NonApplianceReportPath_Half);
                }
            }

            var localReport = new LocalReport();
            localReport.ReportPath = reportPath;

            List<ReportParameter> parameters = new List<ReportParameter>();
            parameters.Add(new ReportParameter("TypeId", dto.SelectedTypeId.ToString()));
            localReport.SetParameters(parameters);

            DataTable dataTable = ConversionHelper.ConvertObjectToDataTable(dto);

            var dataSource = new ReportDataSource("STRPRCDS", dataTable);
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

            return File(renderedBytes, mimeType);
        }
    }
}