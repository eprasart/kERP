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

namespace kERP.SYS
{
    public partial class frmMain : Form
    {
        private string Module = "Main Form";

        public frmMain()
        {
            InitializeComponent();
        }

        private void LoadImages()
        {
            btnHome.Image = ImageFacade.FromFile("Home24");
            btnIC.Image = ImageFacade.FromFile("IC24");
            btnAR.Image = ImageFacade.FromFile("AR24");
            btnAP.Image = ImageFacade.FromFile("AP24");
            btnPO.Image = ImageFacade.FromFile("PO24");
            btnSO.Image = ImageFacade.FromFile("SO24");
            btnGL.Image = ImageFacade.FromFile("GL24");
            btnReport.Image = ImageFacade.FromFile("Report24");
            btnSM.Image = ImageFacade.FromFile("SM24");
            btnSYS.Image = ImageFacade.FromFile("SYS24");
        }

        private void LoadLastNav()
        {
            var uc = new HomeUI();
            uc.Dock = DockStyle.Fill;
            splitContainer1.Panel2.Controls.Add(uc);

            var sNav = ConfigFacade.GetByUser("last_nav", "H");
            switch (sNav)
            {
                case "H":
                    btnHome.Checked = true;
                    break;
                case "IC":
                    btnIC.Checked = true;
                    break;

                case "AR":
                    break;
                case "AP":
                    btnAP.Checked = true;
                    break;
                case "PO":
                    btnAP.Checked = true;
                    break;
                case "SO":

                    break;
                case "GL":

                    break;
                case "R":

                    break;
                case "SM":
                    btnSM.Checked = true;
                    break;
                case "SYS":

                    break;
                default:
                    btnHome.Checked = true;
                    break;
            }           
        }

        private void SaveLastNav()
        {
            var sNav="";
            if (btnHome.Checked)
                sNav = "H";
            else if (btnIC.Checked)
                sNav = "IC";
            else if (btnAR.Checked)
                sNav = "AR";
            else if (btnAP.Checked)
                sNav = "AP";
            else if (btnPO.Checked)
                sNav = "PO";
            else if (btnSO.Checked)
                sNav = "SO";
            else if (btnGL.Checked)
                sNav = "GL";
            else if (btnReport.Checked)
                sNav = "R";
            else if (btnSM.Checked)
                sNav = "SM";
            else if (btnSYS.Checked)
                sNav = "SYS";
            ConfigFacade.Set("last_nav", sNav);
        }

        private void AddToPanelR(UserControl ui)
        {
            ui.Dock = DockStyle.Fill;
            splitContainer1.Panel2.Controls.Add(ui);
            splitContainer1.Panel2.Controls.RemoveAt(1);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            LoadImages();
            splitContainer1.SplitterDistance = ConfigFacade.GetSplitterDistance(Name);
            FormFacade.SetFormState(this);
            
            LoadLastNav();
            
            App.fSplash.ShowMsg("");
            App.fSplash.StartTimer();
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            ConfigFacade.Set(Name + Constant.Splitter_Distance, splitContainer1.SplitterDistance);
            FormFacade.SaveFormSate(this);
            SaveLastNav();
            App.Close();
        }

        private void btnBranch_Click(object sender, EventArgs e)
        {
            var f = App.fBranch;
            if (!Privilege.CanAccess("IC_BRN", "V"))
            {
                MessageFacade.Show(MessageFacade.Get("sys_no_access"), LabelFacade.SYS_Branch);
                return;
            }
            Cursor = Cursors.WaitCursor;
            if (f == null || f.IsDisposed == true)
            {
                f = new frmBranch();
                f.Show();
            }
            if (f.WindowState == FormWindowState.Minimized)
                f.WindowState = FormWindowState.Normal;
            f.Focus();
            SessionLogFacade.Log(Constant.Priority_Information, Module, Constant.Log_Open, f.Text + " opened.");
            Cursor = Cursors.Default;
        }

        private void btnIC_CheckedChanged(object sender, EventArgs e)
        {
            if (!btnIC.Checked) return;
            AddToPanelR(new ICUI());
        }

        private void btnHome_CheckedChanged(object sender, EventArgs e)
        {
            if (!btnHome.Checked) return;
            AddToPanelR(new HomeUI());
        }

        private void btnSM_CheckedChanged(object sender, EventArgs e)
        {
            if (!btnSM.Checked) return;
            AddToPanelR(new SMUI());
        }

        private void btnAR_CheckedChanged(object sender, EventArgs e)
        {
            if (!btnAR.Checked) return;
        }

        private void btnAP_CheckedChanged(object sender, EventArgs e)
        {
            if (!btnAP.Checked) return;
            AddToPanelR(new APUI());
        }

        private void btnPO_CheckedChanged(object sender, EventArgs e)
        {
            if (!btnPO.Checked) return;
            AddToPanelR(new POUI());
        }

        private void btnSO_CheckedChanged(object sender, EventArgs e)
        {
            if (!btnSO.Checked) return;
        }

        private void btnGL_CheckedChanged(object sender, EventArgs e)
        {
            if (!btnGL.Checked) return;
        }

        private void btnReport_CheckedChanged(object sender, EventArgs e)
        {
            if (!btnReport.Checked) return;
        }

        private void btnSYS_CheckedChanged(object sender, EventArgs e)
        {
            if (!btnSYS.Checked) return;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel1_Click(object sender, EventArgs e)
        {

        }
    }
}
