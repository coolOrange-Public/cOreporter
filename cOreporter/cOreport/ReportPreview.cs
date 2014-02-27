using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace cOreporter
{
    public partial class ReportPreview : Form
    {

        public List<DataTable> dataTables { get; set; }
        public XmlDocument reportFile { get; set; }
        public Dictionary<string, string> reportParameters { get; set; }

        public ReportPreview()
        {
            reportParameters = new Dictionary<string, string>();
            dataTables = new List<DataTable>();
            InitializeComponent();
        }

        private void ReportPreview_Load(object sender, EventArgs e)
        {
            if (reportFile == null)
            {
                MessageBox.Show("No report define!!");
                return;
            }
            if (dataTables.Count == 0)
            {
                MessageBox.Show("No data define!!");
                return;
            }
            string fileName = System.IO.Path.GetFileName(reportFile.BaseURI);
            this.Text = String.Format("cOreporter - {0} - {1} records found", fileName, dataTables.First().Rows.Count);

            //this.reportViewer1.RefreshReport();
            this.reportViewer1.Reset();
            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.ReportPath = string.Empty;
            this.reportViewer1.LocalReport.EnableExternalImages = true;
            this.reportViewer1.LocalReport.EnableHyperlinks = true;

            foreach (DataTable dataTable in dataTables)
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource(dataTable.TableName, dataTable));

            using (StringReader reader = new StringReader(reportFile.DocumentElement.OuterXml))
            {
                this.reportViewer1.LocalReport.LoadReportDefinition(reader);
            }

            List<ReportParameter> parameters = new List<ReportParameter>();
            foreach (ReportParameterInfo paramInfo in this.reportViewer1.LocalReport.GetParameters())
            {
                if (reportParameters.ContainsKey(paramInfo.Name))
                {
                    parameters.Add(new ReportParameter(paramInfo.Name, reportParameters[paramInfo.Name]));
                }
            }
            if (parameters.Count > 0)
            {
                try
                {
                    this.reportViewer1.LocalReport.SetParameters(parameters);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Error setting parameters to report. {0}",ex.Message));
                }
            }

            this.reportViewer1.RefreshReport();
            
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.coolorange.com/");
        }
    }
}
