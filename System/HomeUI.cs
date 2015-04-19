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
    public partial class HomeUI : UserControl
    {
        private string Module = "Main Form";

        public HomeUI()
        {
            InitializeComponent();
        }

        private void LoadImages()
        {            
            //btnLabelMessage.Image = ImageFacade.FromFile(""); 
        }

        private void UI_Load(object sender, EventArgs e)
        {
            LoadImages();

            lblVersion.Text = " v " + App.version + System.IO.File.GetLastWriteTime(Application.ExecutablePath).ToString(" (yyyy-MM-dd HH:mm)");
        }

        private void btnLocation_Click(object sender, EventArgs e)
        {
            var f = App.fLocation;
            if (!Privilege.CanAccess("IC_LOC", "V"))
            {
                MessageFacade.Show(MessageFacade.Get("sys_no_access"), LabelFacade.IC_Location);
                return;
            }
            Cursor = Cursors.WaitCursor;
            if (f == null || f.IsDisposed == true)
            {
                f = new frmLocation();
                f.Show();
            }
            if (f.WindowState == FormWindowState.Minimized)
                f.WindowState = FormWindowState.Normal;
            f.Focus();
            SessionLogFacade.Log(Constant.Priority_Information, Module, Constant.Log_Open, f.Text + " opened.");
            Cursor = Cursors.Default;
        }

        private void btnItemSupplier_Click(object sender, EventArgs e)
        {

        }

        private void buttonFlat7_Click(object sender, EventArgs e)
        {

        }
    }
}
