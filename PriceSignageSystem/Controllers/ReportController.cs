using Microsoft.Reporting.WebForms;
using PriceSignageSystem.Models.Constants;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using System;
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
        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult DisplayReport(STRPRCDto model)
        //{
        //    model.O3SKU = 5068; // for testing
        //    model.SelectedCategoryId = 2;

        //    try
        //    {
        //        var data = GetData(model.O3SKU);

        //        // Convert list to DataTable
        //        DataTable dataTable = ConvertListToDataTable(data);

        //        var localReport = new LocalReport();

        //        if(model.SelectedCategoryId == CategoryConstants.NonAppliance)
        //            localReport.ReportPath = Server.MapPath(ReportConstants.NonApplianceReportPath);
        //        else localReport.ReportPath = Server.MapPath(ReportConstants.ApplianceReportPath);


        //        // report data source 
        //        var reportDataSource = new ReportDataSource("STRPRCDS", dataTable);

        //        // Set the report data source
        //        localReport.DataSources.Add(reportDataSource);

        //        // Create and set the report parameter
        //        //var parameter = new ReportParameter("MyParameter", "Parameter Value");
        //        //localReport.SetParameters(new ReportParameter[] { parameter });


        //        string reportType = "PDF";

        //        // Render the report and get the report bytes
        //        byte[] renderedBytes = localReport.Render(reportType);

        //        // Return the report as a file download
        //        return File(renderedBytes, "application/pdf", "MyReport.pdf");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(ex.Message);
        //    }
        //}
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
            model.O3SKU = 5068; // for testing
            model.SelectedCategoryId = 2;

            var reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.ReportPath = Server.MapPath(ReportConstants.NonApplianceReportPath);
            
            var data = GetData(model.O3SKU);

            // Convert list to DataTable
            DataTable dataTable = ConvertListToDataTable(data);
            //Set the report data source if needed
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("STRPRCDS", dataTable));

            ViewBag.ReportViewer = reportViewer;

            return View();
        }
    }
}