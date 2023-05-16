using Microsoft.Reporting.WebForms;
using PriceSignageSystem.Models.Constants;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        private DataTable ConvertListToDataTable<T>(IEnumerable<T> list)
        {
            var dataTable = new DataTable();

            // Get the properties of the type
            var properties = typeof(T).GetProperties();

            // Create columns in the DataTable based on the property names
            foreach (var prop in properties)
            {
                dataTable.Columns.Add(prop.Name, prop.PropertyType);
            }

            // Add rows to the DataTable with property values from the list
            foreach (var item in list)
            {
                var values = properties.Select(prop => prop.GetValue(item)).ToArray();
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
        public List<Models.STRPRC> GetData(decimal O3SKU)
        {
            var data = _sTRPRCRepository.GetData(O3SKU);
            return data;
        }

        public ActionResult DisplayReport(STRPRCDto model)
        {
            //model.O3SKU = 10166; // 5068; // for testing PH
            //model.SelectedCategoryId = 2;

            var reportPath = "";
            if(model.SelectedCategoryId == CategoryConstants.Appliance)
            {
                 reportPath = Server.MapPath(ReportConstants.ApplianceReportPath);
            }

            if (model.SelectedCategoryId == CategoryConstants.NonAppliance)
            {
                 reportPath = Server.MapPath(ReportConstants.NonApplianceReportPath);
            }

            var localReport = new LocalReport();
            localReport.ReportPath = reportPath;

            List<ReportParameter> parameters = new List<ReportParameter>();
            parameters.Add(new ReportParameter("TypeId", model.SelectedTypeId.ToString()));
            localReport.SetParameters(parameters);

            var reportData = GetData(model.O3SKU);
            //DataTable dataTable = ConvertListToDataTable(data);


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
    }
}