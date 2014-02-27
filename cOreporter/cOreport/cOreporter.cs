using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace cOreporter
{
    public class cOreporter
    {
        private bool reportExists(string rdlcFile)
        {
            if (!System.IO.File.Exists(rdlcFile))
            {
                MessageBox.Show(String.Format("Report template {0} not found!!!",rdlcFile));
                return false;
            }
            return true;
        }

        private XmlDocument getXMLfromRDL(string rdlcFile)
        {
            if (!reportExists(rdlcFile)) return null;
            XmlDocument xdoc = new XmlDocument();
            try
            {
                xdoc.Load(rdlcFile);
            }
            catch (Exception ex)
            {

                MessageBox.Show(String.Format("Error parsing report tample {0}!!!\n{1}", rdlcFile, ex.Message));
                return null;
            }
            return xdoc;
        }

        public cOreporter(string rdlcFile, DataTable table, System.Collections.Hashtable parameters )
        {
            List<DataTable> tables = new List<DataTable>();
            tables.Add(table);
            init(rdlcFile, tables.ToArray(), parameters);
        }
 
        public cOreporter(string rdlcFile,DataTable[] tables, System.Collections.Hashtable parameters)
        {
            init(rdlcFile, tables, parameters);
        }

        private void init(string rdlcFile, DataTable[] table, System.Collections.Hashtable parameters)
        {
            XmlDocument xdoc = getXMLfromRDL(rdlcFile);
            if (xdoc == null) return;
            ReportPreview report = new ReportPreview();
            report.dataTables.AddRange(table);
            foreach (DictionaryEntry param in parameters) report.reportParameters.Add(param.Key.ToString(), param.Value == null ? string.Empty : param.Value.ToString());
            report.reportFile = xdoc;
            report.ShowDialog();
        }

    }
}
