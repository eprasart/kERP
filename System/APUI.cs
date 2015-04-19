using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kERP
{
    public partial class APUI : UserControl
    {
        private string Module = "Main Form";

        public APUI()
        {
            InitializeComponent();
        }

        private void LoadImages()
        {
            lblSupplier.Image = ImageFacade.FromFile("Supplier");
        }

        private void APUI_Load(object sender, EventArgs e)
        {
            LoadImages();
        }

        private void lblSupplier_Click(object sender, EventArgs e)
        {
            var f = App.fSupplier;
            if (!Privilege.CanAccess("AP_SUR", "V"))
            {
                MessageFacade.Show(MessageFacade.Get("sys_no_access"), LabelFacade.AP_Supplier);
                return;
            }
            Cursor = Cursors.WaitCursor;
            if (f == null || f.IsDisposed == true)
            {
                f = new frmSupplier();
                f.Show();
            }
            if (f.WindowState == FormWindowState.Minimized)
                f.WindowState = FormWindowState.Normal;
            f.Focus();
            SessionLogFacade.Log(Constant.Priority_Information, Module, Constant.Log_Open, f.Text + " opened.");
            Cursor = Cursors.Default;
        }
    }
}
