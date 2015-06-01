using System;
using System.Windows.Forms;
using kERP.SM;
using kERP.SYS;
using System.Text;
using System.Drawing;

namespace kERP
{
    public partial class frmPwdReset : Form
    {
        public long Id = 0;

        public bool IsDlg = false; // Show dialog box for selecting one 
        public string SearchText = "";
        public string Code;
        public string Description;

        frmMsg fMsg = null;
        string ModuleName = "SM Reset Password";
        string TitleLabel = "Reset Password";

        public frmPwdReset()
        {
            InitializeComponent();
        }

        public frmPwdReset(long Id)
            : this()
        {
            this.Id = Id;
        }

        private void LoadImages()
        {
            btnResetPwd.Image = ImageFacade.FromFile("ResetPwd");
        }

        private bool IsValidated()
        {
            var valid = new Validator(this, "sm_reset_pwd");
            if (txtPwd.IsEmpty) valid.Add(txtPwd, "pwd_blank");
            if (!txtPwd.Text.Equals(txtPwdAgain.Text)) valid.Add(txtPwd, "pwd_not_match");
            return valid.Show();
        }

        private void LoadData()
        {
            try
            {
                var m = UserFacade.Select(Id);
                txtUsername.Text = m.Username;
                txtFullName.Text = m.Full_Name;
                txtPwd.Text = "";   // make it blank
                txtPwdAgain.Text = "";
                cboUserStatus.Value = m.User_Status;
                chkForcePwdChange.Checked = m.Pwd_Change_Force;
            }
            catch (Exception ex)
            {
                MessageFacade.Show(MessageFacade.error_load_record + "\r\n" + ex.Message, TitleLabel, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrorLogFacade.Log(ex);
            }
        }

        private void SetIconDisplayType()
        {
            if (ConfigFacade.Toolbar_Icon_Display_Style == ToolStripItemDisplayStyle.ImageAndText) return;   // If IT=ImageAndText, then do nothing (the designer already take care this)
            foreach (var c in toolStrip1.Items)
            {
                if (c is ToolStripButton)
                    ((ToolStripButton)c).DisplayStyle = ConfigFacade.Toolbar_Icon_Display_Style;
            }
        }

        private void SetSettings()
        {
            try
            {
                SetIconDisplayType();

                //txtUsername.CharacterCasing = ConfigFacade.Character_Casing;
                txtUsername.MaxLength = ConfigFacade.Code_Max_Length;
                FormFacade.SetFormState(this);
            }
            catch (Exception ex)
            {
                ErrorLogFacade.Log(ex, "Set settings");
            }
        }

        private void SetLabels()
        {
            //var prefix = "sm_user_";
            //btnResetPwd.Text = LabelFacade.Get("sys_button_reset_pwd", null) ?? btnResetPwd.Text;

            //lblCode.Text = colCode.HeaderText;
            //todo: load the rest
        }

        private bool Save()
        {
            if (!IsValidated()) return false;
            Cursor = Cursors.WaitCursor;
            var m = new User();
            var log = new SessionLog { Module = ModuleName };
            m.Id = Id;
            m.Username = txtUsername.Text.Trim();
            m.Full_Name = txtFullName.Text;
            if (!txtPwd.ReadOnly) m.Pwd = txtPwd.Text;
            m.User_Status = cboUserStatus.Value;
            m.Pwd_Change_Force = chkForcePwdChange.Checked;

            log.Priority = Constant.Priority_Caution;
            log.Type = Constant.Log_Update;
            try
            {
                UserFacade.ResetPwd(m);
            }
            catch (Exception ex)
            {
                MessageFacade.Show(MessageFacade.error_save + "\r\n" + ex.Message, LabelFacade.sys_save, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrorLogFacade.Log(ex);
            }
            Cursor = Cursors.Default;
            log.Message = "Pwd Reset. Id=" + m.Id + ", Username=" + txtUsername.Text;
            SessionLogFacade.Log(log);
            return true;
        }

        private void frmPwdReset_Load(object sender, EventArgs e)
        {
            try
            {
                LoadImages();
                SetSettings();
                SetLabels();
                DataFacade.LoadList(cboUserStatus, "sys_user_status");
                SessionLogFacade.Log(Constant.Priority_Information, ModuleName, Constant.Log_Open, "Opened");

                LoadData();
            }
            catch (Exception ex)
            {
                ErrorLogFacade.Log(ex, "Form_Load");
                MessageFacade.Show(MessageFacade.error_load_form + "\r\n" + ex.Message, TitleLabel, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control | Keys.R:
                    if (btnResetPwd.Enabled) btnResetPwd_Click(null, null);
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void frmPwdReset_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormFacade.SaveFormSate(this);
        }

        private void txtUsername_Leave(object sender, EventArgs e)
        {
            // Check if entered code already exists
            if (txtUsername.ReadOnly) return;
            if (UserFacade.Exists(txtUsername.Text.Trim()))
            {
                MessageFacade.Show(this, ref fMsg, LabelFacade.sys_msg_prefix + MessageFacade.code_already_exists, LabelFacade.SYS_Branch);
            }
        }

        private void btnResetPwd_Click(object sender, EventArgs e)
        {
            if (Save())
                DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}