namespace kERP
{
    partial class frmPwdReset
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnResetPwd = new System.Windows.Forms.ToolStripButton();
            this.cboUserStatus = new kUI.ComboBox(this.components);
            this.chkForcePwdChange = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblCode = new System.Windows.Forms.Label();
            this.txtPwdAgain = new kUI.TextBox(this.components);
            this.txtPwd = new kUI.TextBox(this.components);
            this.txtFullName = new kUI.TextBox(this.components);
            this.txtUsername = new kUI.TextBox(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.White;
            this.toolStrip1.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnResetPwd});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(383, 25);
            this.toolStrip1.TabIndex = 7;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnResetPwd
            // 
            this.btnResetPwd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnResetPwd.Name = "btnResetPwd";
            this.btnResetPwd.Size = new System.Drawing.Size(48, 22);
            this.btnResetPwd.Text = "Reset";
            this.btnResetPwd.Click += new System.EventHandler(this.btnResetPwd_Click);
            // 
            // cboUserStatus
            // 
            this.cboUserStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboUserStatus.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboUserStatus.FormattingEnabled = true;
            this.cboUserStatus.Location = new System.Drawing.Point(166, 172);
            this.cboUserStatus.Name = "cboUserStatus";
            this.cboUserStatus.Size = new System.Drawing.Size(200, 25);
            this.cboUserStatus.TabIndex = 5;
            this.cboUserStatus.TabOnEnter = true;
            this.cboUserStatus.Value = "";
            // 
            // chkForcePwdChange
            // 
            this.chkForcePwdChange.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkForcePwdChange.Location = new System.Drawing.Point(166, 203);
            this.chkForcePwdChange.Name = "chkForcePwdChange";
            this.chkForcePwdChange.Size = new System.Drawing.Size(199, 39);
            this.chkForcePwdChange.TabIndex = 6;
            this.chkForcePwdChange.Text = "Force password change on next login";
            this.chkForcePwdChange.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(27, 175);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(131, 17);
            this.label11.TabIndex = 4;
            this.label11.Text = "User status";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(13, 143);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(145, 17);
            this.label4.TabIndex = 2;
            this.label4.Text = "New password again";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(27, 110);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "New password";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(27, 77);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(131, 17);
            this.label10.TabIndex = 10;
            this.label10.Text = "Full name";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblCode
            // 
            this.lblCode.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCode.Location = new System.Drawing.Point(27, 44);
            this.lblCode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(131, 17);
            this.lblCode.TabIndex = 8;
            this.lblCode.Text = "Username";
            this.lblCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPwdAgain
            // 
            this.txtPwdAgain.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPwdAgain.Format = null;
            this.txtPwdAgain.Location = new System.Drawing.Point(166, 140);
            this.txtPwdAgain.Margin = new System.Windows.Forms.Padding(4);
            this.txtPwdAgain.Name = "txtPwdAgain";
            this.txtPwdAgain.Numeric = false;
            this.txtPwdAgain.Size = new System.Drawing.Size(200, 25);
            this.txtPwdAgain.TabIndex = 3;
            this.txtPwdAgain.TabOnEnter = true;
            this.txtPwdAgain.UseSystemPasswordChar = true;
            this.txtPwdAgain.Leave += new System.EventHandler(this.txtUsername_Leave);
            // 
            // txtPwd
            // 
            this.txtPwd.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPwd.Format = null;
            this.txtPwd.Location = new System.Drawing.Point(166, 107);
            this.txtPwd.Margin = new System.Windows.Forms.Padding(4);
            this.txtPwd.Name = "txtPwd";
            this.txtPwd.Numeric = false;
            this.txtPwd.Size = new System.Drawing.Size(200, 25);
            this.txtPwd.TabIndex = 1;
            this.txtPwd.TabOnEnter = true;
            this.txtPwd.UseSystemPasswordChar = true;
            this.txtPwd.Leave += new System.EventHandler(this.txtUsername_Leave);
            // 
            // txtFullName
            // 
            this.txtFullName.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFullName.Format = null;
            this.txtFullName.Location = new System.Drawing.Point(166, 74);
            this.txtFullName.Margin = new System.Windows.Forms.Padding(4);
            this.txtFullName.Name = "txtFullName";
            this.txtFullName.Numeric = false;
            this.txtFullName.ReadOnly = true;
            this.txtFullName.Size = new System.Drawing.Size(200, 25);
            this.txtFullName.TabIndex = 11;
            this.txtFullName.TabOnEnter = true;
            this.txtFullName.Leave += new System.EventHandler(this.txtUsername_Leave);
            // 
            // txtUsername
            // 
            this.txtUsername.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUsername.Format = null;
            this.txtUsername.Location = new System.Drawing.Point(166, 41);
            this.txtUsername.Margin = new System.Windows.Forms.Padding(4);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Numeric = false;
            this.txtUsername.ReadOnly = true;
            this.txtUsername.Size = new System.Drawing.Size(200, 25);
            this.txtUsername.TabIndex = 9;
            this.txtUsername.TabOnEnter = true;
            this.txtUsername.Leave += new System.EventHandler(this.txtUsername_Leave);
            // 
            // frmPwdReset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(383, 253);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.cboUserStatus);
            this.Controls.Add(this.chkForcePwdChange);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.txtFullName);
            this.Controls.Add(this.txtPwd);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtPwdAgain);
            this.Controls.Add(this.lblCode);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "frmPwdReset";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reset Password";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPwdReset_FormClosing);
            this.Load += new System.EventHandler(this.frmPwdReset_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Label lblCode;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label10;
        private kUI.TextBox txtFullName;
        private kUI.TextBox txtUsername;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private kUI.TextBox txtPwdAgain;
        private kUI.TextBox txtPwd;
        private System.Windows.Forms.CheckBox chkForcePwdChange;
        private System.Windows.Forms.Label label11;
        private kUI.ComboBox cboUserStatus;
        private System.Windows.Forms.ToolStripButton btnResetPwd;        
    }
}

