using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kERP.SM
{
    public partial class frmPwdReset : Form
    {
        public long Id { get; set; }    // User Id
        public frmPwdReset()
        {
            InitializeComponent();
        }

        public frmPwdReset(string UserName, string FullName)
            : this()
        {
            txtUsernane.Text = UserName;
            txtFullName.Text = FullName;
        }

        private bool IsValidated()
        {
            if (txtPwd.Text != txtPwdAgain.Text )
            {
                MessageBox.Show("Password does not match.", "Reset Password", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtPwd.Focus();
                txtPwd.SelectAll();
            }
            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!IsValidated()) return;
            var usr = new User()
            {
                Id = Id,
                Pwd = txtPwd.Text
            };
            UserFacade.UpdatePwd(usr);
            DialogResult = System.Windows.Forms.DialogResult.OK;
            SessionLogFacade.Log(Constant.Priority_Caution, "User", Constant.Log_ResetPwd, "Password reset successfully. Username=" + txtUsernane.Text);
        }
    }
}
