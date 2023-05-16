using Microsoft.Reporting.WinForms;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ReportControls
{
    public partial class ReportView : UserControl
    {
        public ReportView()
        {
            InitializeComponent();
        }
        public void LoadReport(string reportPath, IEnumerable<object> dataSource)
        {
            // Set the report path
            reportViewer1.LocalReport.ReportPath = reportPath;

            // Set the data source
            ReportDataSource reportDataSource = new ReportDataSource("DataSetName", dataSource);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(reportDataSource);

            // Refresh the report
            reportViewer1.RefreshReport();
        }
        private void textBox3_TextChanged(object sender, System.EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, System.EventArgs e)
        {

        }
    }
}
