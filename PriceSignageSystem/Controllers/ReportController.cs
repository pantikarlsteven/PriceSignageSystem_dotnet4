using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using PriceSignageSystem.Helper;
using PriceSignageSystem.Models;
using PriceSignageSystem.Models.Constants;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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

        public List<STRPRCDto> GetData(decimal O3SKU)
        {
            var data = _sTRPRCRepository.GetData(O3SKU);
            return data;
        }

        public List<CountryDto> GetCountryImg(string country)
        {
            var data = _sTRPRCRepository.GetCountryImg(country);
            return data;
        }

        public ActionResult DisplayReport(STRPRCDto model)
        {
            var reportData = GetData(model.O3SKU);
            var country = "";
            var countryImgData = new List<CountryDto>();

            foreach (var item in reportData)
            {
                if (!String.IsNullOrEmpty(item.O3TRB3))
                {
                    country = reportData.FirstOrDefault().O3TRB3;
                    countryImgData = GetCountryImg(country);
                }

                var convertedImage = ConvertBinaryToJpeg(countryImgData.FirstOrDefault().country_img);
            }


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

            List<ReportParameter> parameters = new List<ReportParameter>();
            parameters.Add(new ReportParameter("TypeId", model.SelectedTypeId.ToString()));
            parameters.Add(new ReportParameter("CategoryId", model.SelectedCategoryId.ToString()));
            localReport.SetParameters(parameters);

            var dataSource = new ReportDataSource("STRPRCDS", reportData);
            localReport.DataSources.Add(dataSource);

            foreach (var item2 in reportData)
            {
                if (!String.IsNullOrEmpty(item2.O3TRB3))
                {
                    var dataSource2 = new ReportDataSource("CountryImageDS", countryImgData);
                    localReport.DataSources.Add(dataSource2);
                }
                else
                {
                    var dataSource2 = new ReportDataSource("CountryImageDS", new List<CountryDto>());
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

            List<ReportParameter> parameters = new List<ReportParameter>();
            parameters.Add(new ReportParameter("TypeId", dto.SelectedTypeId.ToString()));
            parameters.Add(new ReportParameter("CategoryId", dto.SelectedCategoryId.ToString()));

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
        public ActionResult ConvertBinaryToJpeg(byte[] binaryData)
        {
            // Create a MemoryStream from the binary data
            using (MemoryStream memoryStream = new MemoryStream(binaryData))
            {
                // Create an Image object from the MemoryStream
                Image image = Image.FromStream(memoryStream);

                // Create a new MemoryStream to store the JPEG image
                using (MemoryStream jpegStream = new MemoryStream())
                {
                    // Save the Image as a JPEG to the MemoryStream
                    image.Save(jpegStream, ImageFormat.Jpeg);

                    // Set the content type and return the JPEG image
                    return File(jpegStream.ToArray(), "image/jpeg");
                }
            }
        }
    }
}