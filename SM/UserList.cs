using System;
using System.Windows.Forms;

namespace kERP.SM
{
    public partial class frmUserList : Form
    {
        private long Id = 0;
        private int rowIndex = 0;
        private bool IsExpand = false;
                private string Module = "User";   // Log module

        public frmUserList()
        {
            InitializeComponent();            
        }

        private string GetStatus()
        {
            var status = "";
            if (mnuShowA.Checked && !mnuShowI.Checked)
                status = "A";
            else if (mnuShowI.Checked && !mnuShowA.Checked)
                status = "I";
            return status;
        }

        private void RefreshGrid(long seq = 0)
        {
           
        }

        private void LockControls(bool l = true)
        {
            foreach (Control c in splitContainer1.Panel2.Controls)
            {
                if (c is TextBox)
                    ((TextBox)c).ReadOnly = l;
                else if (c is ComboBox)
                    ((ComboBox)c).Enabled = !l;
                else if (c is DateTimePicker)
                    ((DateTimePicker)c).Enabled = !l;
            }
            txtPwd.Enabled = false;
            txtPwdAgain.Enabled = false;
            btnNew.Enabled = l;
            btnCopy.Enabled = l;
            //btnUnlock.Enabled = !l;
            btnSave.Enabled = !l;
            btnSaveNew.Enabled = !l;
            btnActive.Enabled = l;
            btnDelete.Enabled = l;
            splitContainer1.Panel1.Enabled = l;
            btnUnlock.Text = l ? "Un&lock" : "Cance&l";
            btnUnlock.ToolTipText = btnUnlock.Text + " (Ctrl+C)";
        }

        private void SetStatus(string stat)
        {
            if (stat == "A")
            {
                btnActive.Text = "Inactiv&e";
                if (btnActive.Image != Properties.Resources.Inactive)
                    btnActive.Image = Properties.Resources.Inactive;
            }
            else
            {
                btnActive.Text = "Activ&e";
                if (btnActive.Image != Properties.Resources.Active)
                    btnActive.Image = Properties.Resources.Active;
            }
        }

        private bool IsValidated()
        {
            return true;     //todo: 
        }

        private void LoadData()
        {
            var Id = dgvList.Id;
            if (Id == 0) return;
            var m = UserFacade.Select(Id);
            txtUsernane.Text = m.Username;
            txtFullName.Text = m.Full_Name;
            txtPhone.Text = m.Phone;
            txtEmail.Text = m.Email;
            //txtPwd.Text = m.Pwd;
            dtpStart.Checked = (m.Start_On != null);
            if (dtpStart.Checked) dtpStart.Value = (DateTime)m.Start_On;
            dtpEnd.Checked = (m.End_On != null);
            if (dtpEnd.Checked) dtpEnd.Value = (DateTime)m.End_On;
            txtNote.Text = m.Note;
            SetStatus(m.Status);
            LockControls();
        }

        private void frmUserList_Load(object sender, EventArgs e)
        {
            Icon = Properties.Resources.Icon;
            dgvList.ShowLessColumns(true);
            RefreshGrid();
            Text += " v. " + App.version;
            LockControls();
            SessionLogFacade.Log(Constant.Priority_Information, Module, Constant.Log_Open, "Form opened");
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (IsExpand) picExpand_Click(sender, e);
            txtUsernane.Text = "";
            txtUsernane.Focus();
            txtFullName.Text = "";
            txtPwd.Text = "";
            txtPhone.Text = "";
            txtEmail.Text = "";
            txtNote.Text = "";
            if (dgvList.RowCount > 0)
                dgvList.CurrentRow.Selected = false;
            Id = 0;
            LockControls(false);
            txtPwd.Enabled = true;
            txtPwdAgain.Enabled = true;
            if (dgvList.RowCount > 0) rowIndex = dgvList.CurrentRow.Index;
            SessionLogFacade.Log(Constant.Priority_Information, Module, Constant.Log_New, "New clicked");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!IsValidated()) return;
            Cursor = Cursors.WaitCursor;
            var m = new User();
            var log = new SessionLog { Module = Module };
            m.Id = Id;
            m.Username = txtUsernane.Text.Trim();
            m.Full_Name = txtFullName.Text;
            m.Pwd = txtPwd.Text;
            m.Phone = txtPhone.Text;
            m.Email = txtEmail.Text;
            m.Note = txtNote.Text;
            if (dtpStart.Checked) m.Start_On = dtpStart.Value;
            if (dtpEnd.Checked) m.End_On = dtpEnd.Value;
            if (m.Id == 0)
            {
                log.Priority = Constant.Priority_Information;
                log.Type = Constant.Log_Insert;
            }
            else
            {
                log.Priority = Constant.Priority_Caution;
                log.Type = Constant.Log_Update;
            }
            ////m.Id = UserFacade.Save(m);
            if (dgvList.RowCount > 0) rowIndex = dgvList.CurrentRow.Index;
            RefreshGrid(m.Id);
            LockControls();
            Cursor = Cursors.Default;
            log.Message = "Save successfull. Id=" + m.Id + " ,Username=" + txtUsernane.Text;
            SessionLogFacade.Log(log);
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                RefreshGrid();
            }
        }

        private void lblClear_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            txtSearch.Text = "";
            RefreshGrid();
        }

        private void lblRefresh_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            RefreshGrid();
            Cursor = Cursors.Default;
        }

        private void dgvList_SelectionChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnSaveNew_Click(object sender, EventArgs e)
        {
            SessionLogFacade.Log(Constant.Priority_Information, Module, Constant.Log_SaveAndNew, "Save and new. Id=" + dgvList.Id + ", Username=" + txtUsernane.Text);
            btnSave_Click(sender, e);
            btnNew_Click(sender, e);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
           
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
          
        }

        private void picExpand_Click(object sender, EventArgs e)
        {
            splitContainer1.IsSplitterFixed = !IsExpand;
            if (!IsExpand)
            {
                splitContainer1.SplitterDistance = splitContainer1.Size.Width;
                splitContainer1.FixedPanel = FixedPanel.Panel2;
                imgExpand.Image = Properties.Resources.Back;
            }
            else
            {
                splitContainer1.SplitterDistance = 300;
                splitContainer1.FixedPanel = FixedPanel.Panel1;
                imgExpand.Image = Properties.Resources.Next;
            }
            dgvList.ShowLessColumns(IsExpand);
            IsExpand = !IsExpand;
        }

        private void dgvList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (IsExpand) picExpand_Click(sender, e);
            dgvList_SelectionChanged(sender, e);    // reload data since SelectionChanged will not occured on current row
        }

        private void frmUserList_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (btnUnlock.Text == "Cance&l")
                btnUnlock_Click(null, null);
        }

        private void btnActive_Click(object sender, EventArgs e)
        {
            
        }

        private void btnUnlock_Click(object sender, EventArgs e)
        {

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control | Keys.N:
                    if (btnNew.Enabled) btnNew_Click(null, null);
                    break;
                case Keys.Control | Keys.Y:
                    if (btnCopy.Enabled) btnCopy_Click(null, null);
                    break;
                case Keys.Control | Keys.L:
                    if (btnUnlock.Enabled) btnUnlock_Click(null, null);
                    break;
                case Keys.Control | Keys.S:
                    if (btnSave.Enabled) btnSave_Click(null, null);
                    break;
                case Keys.Control | Keys.W:
                    if (btnSaveNew.Enabled) btnSaveNew_Click(null, null);
                    break;
                case Keys.Control | Keys.E:
                    if (btnActive.Enabled) btnActive_Click(null, null);
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void dgvList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                e.SuppressKeyPress = true;
                if (btnDelete.Enabled) btnDelete_Click(null, null);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            lblClear.Enabled = !txtSearch.IsEmpty;
        }

        private void mnuShow_CheckedChanged(object sender, EventArgs e)
        {
            if (!mnuShowA.Checked && !mnuShowI.Checked)
                mnuShowA.Checked = true;
            RefreshGrid();
        }

        private void lblFilter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            mnuShow.Show(lblFilter, 0, 15);
        }
        
        private void btnPwdReset_Click(object sender, EventArgs e)
        {
            SessionLogFacade.Log(Constant.Priority_Caution, Module, Constant.Log_ResetPwd, "Reset pwd clicked.");
            if (dgvList.Id == 0) return;
            var fPwdReset = new SM.frmPwdReset(txtUsernane.Text, txtFullName.Text);
            fPwdReset.Id = dgvList.Id;
            fPwdReset.ShowDialog();
        }
    }
}
