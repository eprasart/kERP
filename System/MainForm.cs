using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            lblVersion.Text = " v " + App.version;

            FormFacade.SetFormState(this);

            App.fSplash.ShowMsg("");
            App.fSplash.StartTimer();
        }

        private void button1_Click(object sender, EventArgs e)
        {

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

        private void btnCategory_Click(object sender, EventArgs e)
        {
            if (!SM.Privilege.CanAccess("ICCAT", "V")) // todo: not hard code
            {
                MessageBox.Show("You don't have the privilege to access this function.");
                return;
            }
            Cursor = Cursors.WaitCursor;
            if (App.fLocation == null || App.fLocation.IsDisposed == true)
            {
                //App.fCategoryList = new IC.frmCategoryList();
                App.fLocation.Show();
            }
            App.fLocation.Focus();
            Cursor = Cursors.Default;
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

        private void btnLoan_Click(object sender, EventArgs e)
        {
            if (!SM.Privilege.CanAccess("LON", "V")) // todo: not hard code
            {
                MessageBox.Show("You don't have the privilege to access this function.");
                return;
            }
            Cursor = Cursors.WaitCursor;
            if (App.fLoan == null || App.fLoan.IsDisposed == true)
            {
                App.fLoan = new frmLoan();
                App.fLoan.Show();
            }
            if (App.fLoan.WindowState == FormWindowState.Minimized) //todo: do this with the rest
                App.fLoan.WindowState = FormWindowState.Normal;
            App.fLoan.Focus();
            Cursor = Cursors.Default;
        }

        private void btnProduct_Click(object sender, EventArgs e)
        {
            var f = App.fProduct;
            if (!SM.Privilege.CanAccess("PRD", "V")) // todo: not hard code
            {
                MessageBox.Show("You don't have the privilege to access this function.");
                return;
            }
            Cursor = Cursors.WaitCursor;
            if (f == null || f.IsDisposed == true)
            {
                f = new frmProduct();
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

       
    }
}
