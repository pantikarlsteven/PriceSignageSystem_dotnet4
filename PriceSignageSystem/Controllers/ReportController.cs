using Microsoft.Reporting.WebForms;
using System;
using System.Web.Mvc;

namespace PriceSignageSystem.Controllers
{
    public class ReportController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DisplayReport()
        {
            try
            {
                var localReport = new LocalReport();
                localReport.ReportPath = Server.MapPath("~/Reports/NonApplianceReport.rdlc"); // Replace with your report path

                // Add report parameters if required
                //localReport.SetParameters(new ReportParameter("ParameterName", "ParameterValue"));

                // Add report data sources if required
                //localReport.DataSources.Add(new ReportDataSource("DataSourceName", GetReportData()));

                string reportType = "PDF"; // Change the output format if needed (PDF, Excel, Word, etc.)
                byte[] renderedBytes;

                // Render the report and get the report bytes
                renderedBytes = localReport.Render(reportType);

                // Return the report as a file download
                return File(renderedBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}