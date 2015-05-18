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
    public partial class SMUI : UserControl
    {
        private string Module = "Main Form";

        public SMUI()
        {
            InitializeComponent();
        }

        private void LoadImages()
        {
            lblUser.Image = ImageFacade.FromFile("User");
            lblRole.Image = ImageFacade.FromFile("Role");
            lblFunction.Image = ImageFacade.FromFile("Function");
            lblUserRole.Image = ImageFacade.FromFile("UserRole");
            lblRoleFunction.Image = ImageFacade.FromFile("RoleFunction");
            lblUserFunction.Image = ImageFacade.FromFile("UserFunction");
            lblAuditLog.Image = ImageFacade.FromFile("AuditLog");
        }

        private void ICUI_Load(object sender, EventArgs e)
        {
            LoadImages();
        }

        private void lblUser_Click(object sender, EventArgs e)
        {
            var f = App.fUser;
            if (!Privilege.CanAccess("SM_USR", "V"))
            {
                MessageFacade.Show(MessageFacade.Get("sys_no_access"), LabelFacade.SM_User);
                return;
            }
            Cursor = Cursors.WaitCursor;
            if (f == null || f.IsDisposed == true)
            {
                f = new frmUser();
                f.Show();
            }
            if (f.WindowState == FormWindowState.Minimized)
                f.WindowState = FormWindowState.Normal;
            f.Focus();
            SessionLogFacade.Log(Constant.Priority_Information, Module, Constant.Log_Open, f.Text + " opened.");
            Cursor = Cursors.Default;
        }

        private void lblAuditLog_Click(object sender, EventArgs e)
        {
            var f = App.fAuditLog;
            if (!Privilege.CanAccess("SM_ALG", "V"))
            {
                MessageFacade.Show(MessageFacade.Get("sys_no_access"), LabelFacade.SM_AuditLog);
                return;
            }
            Cursor = Cursors.WaitCursor;
            if (f == null || f.IsDisposed == true)
            {
                f = new frmAuditLog();
                f.Show();
            }
            if (f.WindowState == FormWindowState.Minimized)
                f.WindowState = FormWindowState.Normal;
            f.Focus();
            SessionLogFacade.Log(Constant.Priority_Information, Module, Constant.Log_Open, f.Text + " opened.");
            Cursor = Cursors.Default;
        }

        private void lblRole_Click(object sender, EventArgs e)
        {
            var f = App.fRole;
            if (!Privilege.CanAccess("SM_ROL", "V"))
            {
                MessageFacade.Show(MessageFacade.Get("sys_no_access"), LabelFacade.SM_Role);
                return;
            }
            Cursor = Cursors.WaitCursor;
            if (f == null || f.IsDisposed == true)
            {
                f = new frmRole();
                f.Show();
            }
            if (f.WindowState == FormWindowState.Minimized)
                f.WindowState = FormWindowState.Normal;
            f.Focus();
            SessionLogFacade.Log(Constant.Priority_Information, Module, Constant.Log_Open, f.Text + " opened.");
            Cursor = Cursors.Default;
        }

        private void lblUserRole_Click(object sender, EventArgs e)
        {
            var f = App.fUserRole;
            if (!Privilege.CanAccess("SM_UR", "V"))
            {
                MessageFacade.Show(MessageFacade.Get("sys_no_access"), LabelFacade.SM_Role);
                return;
            }
            Cursor = Cursors.WaitCursor;
            if (f == null || f.IsDisposed == true)
            {
                f = new frmUserRole();
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
