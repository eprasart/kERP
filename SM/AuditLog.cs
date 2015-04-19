using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kERP
{
    public partial class frmAuditLog : Form
    {
        public frmAuditLog()
        {
            InitializeComponent();
        }

        private void RefreshGrid()
        {
            dgvList.DataSource = SessionLogFacade.GetDataTable(txtSearch.Text);
        }

        private void frmAuditLog_Load(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }
    }
}
