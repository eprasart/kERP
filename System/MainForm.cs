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

        private void frmMain_Load(object sender, EventArgs e)
        {
            lblVersion.Text = " v " + App.version + File.GetLastWriteTime(Application.ExecutablePath).ToString(" (yyyy-MM-dd HH:mm)");

            FormFacade.SetFormState(this);

            App.fSplash.ShowMsg("");
            App.fSplash.StartTimer();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            new SM.frmUserList().Show();
            Cursor = Cursors.Default;
        }

        private void btnUser_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            if (App.fUserList == null || App.fUserList.IsDisposed == true)
            {
                App.fUserList = new SM.frmUserList();
                App.fUserList.Show();
            }
            App.fUserList.Focus();
            Cursor = Cursors.Default;
            SM.SessionLogFacade.Log(Constant.Priority_Information, Module, Constant.Log_Open, "User List opened.");
        }

        private void btnAuditLog_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            SM.SessionLogFacade.Log(Constant.Priority_Information, Module, Constant.Log_Open, "Audit Log opened.");
            if (App.fAuditLog == null || App.fAuditLog.IsDisposed == true)
            {
                App.fAuditLog = new SM.frmAuditLog();
                App.fAuditLog.Show();
            }
            App.fAuditLog.Focus();
            Cursor = Cursors.Default;
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormFacade.SaveFormSate(this);

            App.Close();
        }

        private void btnBranch_Click(object sender, EventArgs e)
        {
            if (!SM.Privilege.CanAccess("BRN", "V")) // todo: not hard code
            {
                MessageBox.Show("You don't have the privilege to access this function.");
                return;
            }
            Cursor = Cursors.WaitCursor;
            if (App.fBranch == null || App.fBranch.IsDisposed == true)
            {
                App.fBranch = new frmBranch();
                App.fBranch.Show();
            }
            App.fBranch.Focus();
            Cursor = Cursors.Default;
        }

        private void btnLocation_Click(object sender, EventArgs e)
        {
            if (!SM.Privilege.CanAccess("CUS", "V")) // todo: not hard code
            {
                MessageBox.Show("You don't have the privilege to access this function.");
                return;
            }
            Cursor = Cursors.WaitCursor;
            if (App.fLocation == null || App.fLocation.IsDisposed == true)
            {
                App.fLocation = new frmLocation();
                App.fLocation.Show();
            }
            App.fLocation.Focus();
            Cursor = Cursors.Default;
        }

        private void btnUnitMeasure_Click(object sender, EventArgs e)
        {
            if (!SM.Privilege.CanAccess("LON", "V")) // todo: not hard code
            {
                MessageBox.Show("You don't have the privilege to access this function.");
                return;
            }
            Cursor = Cursors.WaitCursor;
            if (App.fUnitMeasure == null || App.fUnitMeasure.IsDisposed == true)
            {
                App.fUnitMeasure = new frmUnitMeasure();
                App.fUnitMeasure.Show();
            }
            if (App.fUnitMeasure.WindowState == FormWindowState.Minimized) //todo: do this with the rest
                App.fUnitMeasure.WindowState = FormWindowState.Normal;
            App.fUnitMeasure.Focus();
            Cursor = Cursors.Default;
        }

        private void btnCategory_Click(object sender, EventArgs e)
        {
            var f = App.fCategory;
            if (!SM.Privilege.CanAccess("PRD", "V")) // todo: not hard code
            {
                MessageBox.Show("You don't have the privilege to access this function.");
                return;
            }
            Cursor = Cursors.WaitCursor;
            if (f == null || f.IsDisposed == true)
            {
                f = new frmCategory();
                f.Show();
            }
            if (f.WindowState == FormWindowState.Minimized) //todo: do this with the rest
                f.WindowState = FormWindowState.Normal;
            f.Focus();
            Cursor = Cursors.Default;
        }

        private void btnHoliday_Click(object sender, EventArgs e)
        {
            var f = App.fHoliday;
            if (!SM.Privilege.CanAccess("PRD", "V")) // todo: not hard code
            {
                MessageBox.Show("You don't have the privilege to access this function.");
                return;
            }
            Cursor = Cursors.WaitCursor;
            if (f == null || f.IsDisposed == true)
            {
                f = new frmHoliday();
                f.Show();
            }
            if (f.WindowState == FormWindowState.Minimized) //todo: do this with the rest
                f.WindowState = FormWindowState.Normal;
            f.Focus();
            Cursor = Cursors.Default;
        }

        private void btnVendor_Click(object sender, EventArgs e)
        {
            var f = App.fVendor;
            if (!SM.Privilege.CanAccess("PRD", "V")) // todo: not hard code
            {
                MessageBox.Show("You don't have the privilege to access this function.");
                return;
            }
            Cursor = Cursors.WaitCursor;
            if (f == null || f.IsDisposed == true)
            {
                f = new frmVendor();
                f.Show();
            }
            if (f.WindowState == FormWindowState.Minimized) //todo: do this with the rest
                f.WindowState = FormWindowState.Normal;
            f.Focus();
            Cursor = Cursors.Default;
        }

        private void btnItem_Click(object sender, EventArgs e)
        {
            var f = App.fItem;
            if (!SM.Privilege.CanAccess("PRD", "V")) // todo: not hard code
            {
                MessageBox.Show("You don't have the privilege to access this function.");
                return;
            }
            Cursor = Cursors.WaitCursor;
            if (f == null || f.IsDisposed == true)
            {
                f = new frmItem();
                f.Show();
            }
            if (f.WindowState == FormWindowState.Minimized) //todo: do this with the rest
                f.WindowState = FormWindowState.Normal;
            f.Focus();
            Cursor = Cursors.Default;
        }

        private void btnClassification_Click(object sender, EventArgs e)
        {
            var f = App.fClassification;
            if (!SM.Privilege.CanAccess("PRD", "V")) // todo: not hard code
            {
                MessageBox.Show("You don't have the privilege to access this function.");
                return;
            }
            Cursor = Cursors.WaitCursor;
            if (f == null || f.IsDisposed == true)
            {
                f = new frmClassification();
                f.Show();
            }
            if (f.WindowState == FormWindowState.Minimized) //todo: do this with the rest
                f.WindowState = FormWindowState.Normal;
            f.Focus();
            Cursor = Cursors.Default;
        }
    }
}
