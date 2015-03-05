using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.Collections.Generic;

namespace kERP
{
    public partial class frmReport : Form
    {
        //Fields
        private string _FileName;
        private List<ReportParameter> Parameters = new List<ReportParameter>();

        //Property
        public DataTable ReportSource { get; set; }

        public string FileName
        {
            get
            {
                return System.IO.Path.Combine(App.StartupPath, "Report", _FileName);
            }
            set
            {
                _FileName = value;
            }
        }

        public void AddParameter(ReportParameter p)
        {
            Parameters.Add(p);
        }

        public void AddParameter(string name, string value)
        {

            Parameters.Add(new ReportParameter(name, value));
        }

        public void PreviewReport()
        {
            Cursor = Cursors.WaitCursor;
            rptViewer.LocalReport.EnableExternalImages = true;
            rptViewer.LocalReport.ReportPath = FileName;
            ReportDataSource rds = new ReportDataSource("DataSet1", ReportSource);
            rptViewer.LocalReport.DataSources.Add(rds);
            if (Parameters.Count > 0)
                rptViewer.LocalReport.SetParameters(Parameters);
            rptViewer.RefreshReport();
            Show();
            Cursor = Cursors.Default;
        }

        public frmReport()
        {
            InitializeComponent();
        }

        public frmReport(string Title)
            : this()
        {
            Text = Title;
        }

        private void frmReport_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl+P => Print
            if (e.Control && e.KeyCode == Keys.P)
            {
                this.rptViewer.PrintDialog();
            }
        }
    }
}
