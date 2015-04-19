using System;
using System.Windows.Forms;
using kERP.SM;
using kERP.SYS;
using System.Text;
using System.Drawing;

namespace kERP
{
    public partial class frmCategory : Form
    {
        public long Id = 0;
        int RowIndex = 0;   // Current gird selected row
        bool IsExpand = false;
        bool IsDirty = false;
        bool IsIgnore = true;

        public bool IsDlg = false; // Show dialog box for selecting one 
        public string SearchText = "";
        public string Code;
        public string Description;

        frmMsg fMsg = null;
        string ModuleName = "IC Category";
        string TitleLabel = CategoryFacade.TitleLabel;

        public frmCategory()
        {
            InitializeComponent();
        }
        private void LoadImages()
        {
            btnSelect.Image = ImageFacade.FromFile("Select");
            btnNew.Image = ImageFacade.FromFile("New");
            btnCopy.Image = ImageFacade.FromFile("Copy");
            btnUnlock.Image = ImageFacade.FromFile("Unlock");
            btnSave.Image = ImageFacade.FromFile("Save");
            btnSaveNew.Image = ImageFacade.FromFile("SaveNew");
            btnActive.Image = ImageFacade.FromFile("Inactive");
            btnDelete.Image = ImageFacade.FromFile("Delete");
            btnMode.Image = ImageFacade.FromFile("Mode");
            btnExport.Image = ImageFacade.FromFile("Export");

            btnFind.Image = ImageFacade.FromFile("Find");
            btnClear.Image = ImageFacade.FromFile("Clear");
            btnFilter.Image = ImageFacade.FromFile("Filter");
        }

        private string GetStatus()
        {
            var status = "";
            if (mnuShowA.Checked && !mnuShowI.Checked)
                status = Constant.RecordStatus_Active;
            else if (mnuShowI.Checked && !mnuShowA.Checked)
                status = Constant.RecordStatus_InActive;
            return status;
        }

        private void RefreshGrid(long seq = 0)
        {
            Cursor = Cursors.WaitCursor;
            //IsIgnore = true;
            if (dgvList.SelectedRows.Count > 0) RowIndex = dgvList.SelectedRows[0].Index;
            try
            {
                dgvList.DataSource = CategoryFacade.GetDataTable(txtFind.Text, GetStatus());
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageFacade.Show(MessageFacade.error_retrieve_data + "\r\n" + ex.Message, TitleLabel, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrorLogFacade.Log(ex);
                return;
            }
            if (dgvList.RowCount > 0)
            {
                if (seq == 0)
                {
                    if (RowIndex >= dgvList.RowCount) RowIndex = dgvList.RowCount - 1;
                    dgvList.CurrentCell = dgvList[1, RowIndex];
                }
                else
                    foreach (DataGridViewRow row in dgvList.Rows)
                        if ((long)row.Cells[0].Value == seq)
                        {
                            Id = (int)seq;
                            dgvList.CurrentCell = dgvList[1, row.Index];
                            break;
                        }
            }
            else
            {
                btnCopy.Enabled = false;
                btnUnlock.Enabled = false;
                btnActive.Enabled = false;
                btnDelete.Enabled = false;
                ClearAllBoxes();
            }
            IsIgnore = false;
            Cursor = Cursors.Default;
        }

        private void LockControls(bool l = true)
        {
            if (Id != 0 && l == false)
                txtCode.ReadOnly = true;
            else
                txtCode.ReadOnly = l;
            txtDescription.ReadOnly = l;
            txtNote.ReadOnly = l;
            btnNew.Enabled = l;
            btnCopy.Enabled = dgvList.Id != 0 && l;
            btnSave.Enabled = !l;
            btnSaveNew.Enabled = !l;
            btnActive.Enabled = dgvList.Id != 0 && l;
            btnDelete.Enabled = dgvList.Id != 0 && l;
            splitContainer1.Panel1.Enabled = l;
            btnUnlock.Enabled = !l || dgvList.RowCount > 0;
            btnUnlock.Text = l ? LabelFacade.sys_button_unlock : LabelFacade.sys_button_cancel;
            txtFind.ReadOnly = !l;
            btnFind.Enabled = l;
            btnClear.Enabled = l;
            btnFilter.Enabled = l;
            if (fMsg != null && !fMsg.IsDisposed) fMsg.Close();
        }

        private void SetStatus(string stat)
        {
            if (stat == Constant.RecordStatus_Active)
            {
                if (btnActive.Text == LabelFacade.sys_button_inactive) return;
                btnActive.Text = LabelFacade.sys_button_inactive;
                if (btnActive.Text.Equals(LabelFacade.sys_button_inactive))
                    btnActive.Image = ImageFacade.FromFile("Inactive");
            }
            else
            {
                if (btnActive.Text == LabelFacade.sys_button_active) return;
                btnActive.Text = LabelFacade.sys_button_active;
                if (btnActive.Text.Equals(LabelFacade.sys_button_active))
                    btnActive.Image = ImageFacade.FromFile("Active");
            }
        }

        private bool IsValidated()
        {
            var valid = new Validator(this, "ic_category");
            string sCode = txtCode.Text.Trim();
            if (sCode.Length == 0)
                valid.Add(txtCode, "code_blank");
            else if (CategoryFacade.Exists(sCode, Id))
                valid.Add(txtCode, "code_exists");
            if (txtDescription.IsEmptyTrim) valid.Add(txtDescription, "description_blank");
            return valid.Show();
        }

        private void ClearAllBoxes()
        {
            txtCode.Text = "";
            txtCode.Focus();
            txtDescription.Text = "";
            txtNote.Text = "";
            IsDirty = false;
        }

        private void LoadData()
        {
            Id = dgvList.Id;
            if (Id != 0)
                try
                {
                    var m = CategoryFacade.Select(Id);
                    txtCode.Text = m.Code;
                    txtDescription.Text = m.Description;
                    txtNote.Text = m.Note;
                    SetStatus(m.Status);
                    LockControls();
                    IsDirty = false;
                    SessionLogFacade.Log(Constant.Priority_Information, ModuleName, Constant.Log_View, "View. Id=" + m.Id + ", Code=" + m.Code);
                }
                catch (Exception ex)
                {
                    MessageFacade.Show(MessageFacade.error_load_record + "\r\n" + ex.Message, TitleLabel, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ErrorLogFacade.Log(ex);
                }
            else    // when grid is empty => disable buttons and clear all controls
            {
                if (dgvList.RowCount == 0)
                {
                    btnUnlock.Enabled = false;
                    ClearAllBoxes();
                }
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
                splitContainer1.SplitterDistance = ConfigFacade.GetSplitterDistance(Name);

                txtCode.CharacterCasing = ConfigFacade.Character_Casing;
                txtCode.MaxLength = ConfigFacade.Code_Max_Length;
                FormFacade.SetFormState(this);
            }
            catch (Exception ex)
            {
                ErrorLogFacade.Log(ex, "Set settings");
            }
        }

        private void SetLabels()
        {
            var prefix = "ic_category_";
            btnNew.Text = LabelFacade.sys_button_new ?? btnNew.Text;
            btnCopy.Text = LabelFacade.sys_button_copy ?? btnCopy.Text;
            btnUnlock.Text = LabelFacade.sys_button_unlock ?? btnUnlock.Text;
            btnSave.Text = LabelFacade.sys_button_save ?? btnSave.Text;
            btnSaveNew.Text = LabelFacade.sys_button_save_new ?? btnSaveNew.Text;
            btnActive.Text = LabelFacade.sys_button_inactive ?? btnActive.Text;
            btnDelete.Text = LabelFacade.sys_button_delete ?? btnDelete.Text;
            btnMode.Text = LabelFacade.sys_button_mode ?? btnMode.Text;
            btnExport.Text = LabelFacade.sys_export ?? btnExport.Text;
            lblSearch.Text = LabelFacade.sys_search_place_holder ?? lblSearch.Text;
            btnFind.Text = "     " + (LabelFacade.sys_button_find ?? btnFind.Text.Replace(" ", ""));
            btnClear.Text = "     " + (LabelFacade.sys_button_clear ?? btnClear.Text.Replace(" ", ""));
            btnFilter.Text = "     " + (LabelFacade.sys_button_filter ?? btnFilter.Text.Replace(" ", ""));

            colCode.HeaderText = LabelFacade.Get(prefix + "code") ?? colCode.HeaderText;
            lblCode.Text = colCode.HeaderText;
            glbGeneral.Caption = LabelFacade.Get(prefix + "general") ?? glbGeneral.Caption;
            glbNote.Caption = LabelFacade.Get(prefix + "note") ?? glbNote.Caption;
            //todo: load the rest
        }

        private bool Save()
        {
            if (!IsValidated()) return false;
            Cursor = Cursors.WaitCursor;
            var m = new Category();
            var log = new SessionLog { Module = ModuleName };
            m.Id = Id;
            m.Code = txtCode.Text.Trim();
            m.Description = txtDescription.Text;
            m.Note = txtNote.Text;
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
            try
            {
                m.Id = CategoryFacade.Save(m);
            }
            catch (Exception ex)
            {
                MessageFacade.Show(MessageFacade.error_save + "\r\n" + ex.Message, LabelFacade.sys_save, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrorLogFacade.Log(ex);
            }
            if (dgvList.CurrentRow != null) RowIndex = dgvList.CurrentRow.Index;
            RefreshGrid(m.Id);
            LockControls();
            Cursor = Cursors.Default;
            log.Message = "Saved. Id=" + m.Id + ", Code=" + txtCode.Text;
            SessionLogFacade.Log(log);
            IsDirty = false;
            return true;
        }

        private void frmCategory_Load(object sender, EventArgs e)
        {
            try
            {
                LoadImages();
                dgvList.ShowLessColumns(true);
                SetSettings();
                SetLabels();
                SessionLogFacade.Log(Constant.Priority_Information, ModuleName, Constant.Log_Open, "Opened");
                RefreshGrid();

                LoadData();
            }
            catch (Exception ex)
            {
                ErrorLogFacade.Log(ex, "Form_Load");
                MessageFacade.Show(MessageFacade.error_load_form + "\r\n" + ex.Message, TitleLabel, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            btnSelect.Visible = IsDlg;
            if (IsDlg)
            {
                btnMode_Click(null, null);
                toolStrip1.Refresh();
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (!Privilege.CanAccess(Constant.Function_IC_Unit_Measure, Constant.Privilege_New))
            {
                MessageFacade.Show(MessageFacade.privilege_no_access, LabelFacade.sys_new, MessageBoxButtons.OK, MessageBoxIcon.Information);
                SessionLogFacade.Log(Constant.Priority_Caution, ModuleName, Constant.Log_NoAccess, "New: No access");
                return;
            }
            if (IsExpand) picExpand_Click(sender, e);
            ClearAllBoxes();
            if (dgvList.CurrentRow != null)
                dgvList.CurrentRow.Selected = false;
            Id = 0;
            LockControls(false);
            if (dgvList.CurrentRow != null) RowIndex = dgvList.CurrentRow.Index;
            SessionLogFacade.Log(Constant.Priority_Information, ModuleName, Constant.Log_New, "New clicked");
            IsDirty = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnFind_Click(null, null);
            }
        }

        private void dgvList_SelectionChanged(object sender, EventArgs e)
        {
            if (IsIgnore) return;
            LoadData();
        }

        private void btnSaveNew_Click(object sender, EventArgs e)
        {
            SessionLogFacade.Log(Constant.Priority_Information, ModuleName, Constant.Log_SaveAndNew, "Saved and new. Id=" + dgvList.Id + ", Code=" + txtCode.Text);
            btnSave_Click(sender, e);
            if (btnSaveNew.Enabled) return;
            btnNew_Click(sender, e);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (Id == 0) return;
                // If referenced
                //todo: check if exist in ic_item
                // If locked
                var lInfo = CategoryFacade.GetLock(Id);
                string msg = "";
                if (lInfo.Locked)
                {
                    msg = string.Format(MessageFacade.delete_locked, lInfo.Lock_By, lInfo.Lock_At);
                    if (!Privilege.CanAccess(Constant.Function_IC_Unit_Measure, "O"))
                    {
                        MessageFacade.Show(msg, LabelFacade.sys_delete, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        SessionLogFacade.Log(Constant.Priority_Caution, ModuleName, Constant.Log_Delete, "Cannot delete. Currently locked by '" + lInfo.Lock_By + "' since '" + lInfo.Lock_At + "' . Id=" + dgvList.Id + ", Code=" + txtCode.Text);
                        return;
                    }
                }
                // Delete
                msg = MessageFacade.delete_confirmation;
                if (lInfo.Locked) msg = string.Format(MessageFacade.lock_currently, lInfo.Lock_By, lInfo.Lock_At) + "'\n" + msg;
                if (MessageFacade.Show(msg, LabelFacade.sys_delete, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No)
                    return;
                try
                {
                    CategoryFacade.SetStatus(Id, Constant.RecordStatus_Deleted);
                }
                catch (Exception ex)
                {
                    MessageFacade.Show(MessageFacade.error_delete + "\r\n" + ex.Message, LabelFacade.sys_delete, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ErrorLogFacade.Log(ex);
                }
                RefreshGrid();
                // log
                SessionLogFacade.Log(Constant.Priority_Warning, ModuleName, Constant.Log_Delete, "Deleted. Id=" + dgvList.Id + ", Code=" + txtCode.Text);
            }
            catch (Exception ex)
            {
                MessageFacade.Show(MessageFacade.error_delete + "\r\n" + ex.Message, LabelFacade.sys_delete, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrorLogFacade.Log(ex);
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (!Privilege.CanAccess(Constant.Function_IC_Unit_Measure, Constant.Privilege_New))
            {
                MessageFacade.Show(MessageFacade.privilege_no_access, LabelFacade.sys_copy, MessageBoxButtons.OK, MessageBoxIcon.Information);
                SessionLogFacade.Log(Constant.Priority_Information, ModuleName, Constant.Log_NoAccess, "Copy: No access");
                return;
            }
            Id = 0;
            if (IsExpand) picExpand_Click(sender, e);
            txtCode.Focus();
            LockControls(false);
            SessionLogFacade.Log(Constant.Priority_Information, ModuleName, Constant.Log_Copy, "Copy from Id=" + dgvList.Id + "Code=" + txtCode.Text);
            IsDirty = false;
        }

        private void picExpand_Click(object sender, EventArgs e)
        {
            splitContainer1.IsSplitterFixed = !IsExpand;
            if (!IsExpand)
            {
                splitContainer1.SplitterDistance = splitContainer1.Size.Width;
                splitContainer1.FixedPanel = FixedPanel.Panel2;
            }
            else
            {
                splitContainer1.SplitterDistance = ConfigFacade.GetInt(Name + Constant.Splitter_Distance);
                splitContainer1.FixedPanel = FixedPanel.Panel1;
            }
            dgvList.ShowLessColumns(IsExpand);
            IsExpand = !IsExpand;
        }

        private void dgvList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            if (!IsDlg)
            {
                if (IsExpand) picExpand_Click(sender, e);
                dgvList_SelectionChanged(sender, e);    // reload data since SelectionChanged will not occured on current row
            }
            else
                btnSelect_Click(null, null);
        }

        private void btnActive_Click(object sender, EventArgs e)
        {
            if (Id == 0) return;

            string status = btnActive.Text == LabelFacade.sys_button_inactive ? Constant.RecordStatus_InActive : Constant.RecordStatus_Active;
            // If referenced
            //todo: check if already used in ic_item

            //If locked
            var lInfo = CategoryFacade.GetLock(Id);
            if (lInfo.Locked)
            {
                string msg = string.Format(MessageFacade.lock_currently, lInfo.Lock_By, lInfo.Lock_At);
                if (!Privilege.CanAccess(Constant.Function_IC_Unit_Measure, "O"))
                {
                    MessageFacade.Show(msg, MessageFacade.active_inactive, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                    if (MessageFacade.Show(msg + "\r\n" + MessageFacade.proceed_confirmation, MessageFacade.active_inactive, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No)
                        return;
            }
            try
            {
                CategoryFacade.SetStatus(Id, status);
            }
            catch (Exception ex)
            {
                MessageFacade.Show(MessageFacade.error_active_inactive + ex.Message, MessageFacade.active_inactive, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrorLogFacade.Log(ex);
            }
            RefreshGrid();
            SessionLogFacade.Log(Constant.Priority_Caution, ModuleName, status == Constant.RecordStatus_InActive ? Constant.Log_Inactive : Constant.Log_Active, "Id=" + dgvList.Id + ", Code=" + txtCode.Text);
        }

        private void btnUnlock_Click(object sender, EventArgs e)
        {
            if (!Privilege.CanAccess(Constant.Function_IC_Unit_Measure, Constant.Privilege_Update))
            {
                MessageFacade.Show(MessageFacade.privilege_no_access, LabelFacade.sys_button_unlock, MessageBoxButtons.OK, MessageBoxIcon.Information);
                SessionLogFacade.Log(Constant.Priority_Information, ModuleName, Constant.Log_NoAccess, "Copy: No access");
                return;
            }
            if (IsExpand) picExpand_Click(sender, e);
            Id = dgvList.Id;
            // Cancel
            if (btnUnlock.Text == LabelFacade.sys_button_cancel)
            {
                if (IsDirty)
                {
                    var result = MessageFacade.Show(MessageFacade.save_confirmation, LabelFacade.sys_button_cancel, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.Yes) // Save then close
                        btnSave_Click(null, null);
                    else if (result == System.Windows.Forms.DialogResult.No)
                        LoadData(); // Load original back if changes (dirty)
                    else if (result == System.Windows.Forms.DialogResult.Cancel)
                        return;
                }
                LockControls(true);
                dgvList.Focus();
                try
                {
                    CategoryFacade.ReleaseLock(dgvList.Id);
                }
                catch (Exception ex)
                {
                    MessageFacade.Show(MessageFacade.error_unlock + "\r\n" + ex.Message, LabelFacade.sys_unlock, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ErrorLogFacade.Log(ex);
                    return;
                }
                if (dgvList.CurrentRow != null && !dgvList.CurrentRow.Selected)
                    dgvList.CurrentRow.Selected = true;
                SessionLogFacade.Log(Constant.Priority_Information, ModuleName, Constant.Log_Unlock, "Unlock cancel. Id=" + dgvList.Id + ", Code=" + txtCode.Text);
                btnUnlock.ToolTipText = "Unlock (Ctrl+L)";
                IsDirty = false;
                return;
            }
            // Unlock
            if (Id == 0) return;
            try
            {
                var lInfo = CategoryFacade.GetLock(Id);

                if (lInfo.Locked) // Check if record is locked
                {
                    string msg = string.Format(MessageFacade.lock_currently, lInfo.Lock_By, lInfo.Lock_At);
                    if (!Privilege.CanAccess(Constant.Function_IC_Unit_Measure, "O"))
                    {
                        MessageFacade.Show(msg, LabelFacade.sys_unlock, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else
                        if (MessageFacade.Show(msg + "\r\n" + MessageFacade.lock_override, LabelFacade.sys_unlock, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
                            SessionLogFacade.Log(Constant.Priority_Caution, ModuleName, Constant.Log_Lock, "Override lock. Id=" + dgvList.Id + ", Code=" + txtCode.Text);
                        else
                            return;
                }
                txtDescription.Focus2();
                LockControls(false);
            }
            catch (Exception ex)
            {
                MessageFacade.Show(MessageFacade.error_unlock + "\r\n" + ex.Message, LabelFacade.sys_unlock, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrorLogFacade.Log(ex);
                return;
            }
            try
            {
                CategoryFacade.Lock(dgvList.Id, txtCode.Text);
            }
            catch (Exception ex)
            {
                MessageFacade.Show(MessageFacade.error_lock + "\r\n" + ex.Message, LabelFacade.sys_lock, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrorLogFacade.Log(ex);
                return;
            }
            SessionLogFacade.Log(Constant.Priority_Information, ModuleName, Constant.Log_Lock, "Locked. Id=" + dgvList.Id + ", Code=" + txtCode.Text);
            btnUnlock.ToolTipText = "Cancel (Esc or Ctrl+L)";
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
                case Keys.Escape:
                    if (btnUnlock.Text.StartsWith("C")) btnUnlock_Click(null, null);    // Cancel
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
                case Keys.Control | Keys.F:
                    if (!txtFind.ReadOnly) txtFind.Focus();
                    break;
                case Keys.F3:
                case Keys.F5:
                    if (btnFind.Enabled) btnFind_Click(null, null);
                    break;
                case Keys.F4:
                    if (btnClear.Enabled) btnClear_Click(null, null);
                    break;
                case Keys.F8:
                    if (btnFilter.Enabled) btnFilter_Click(null, null);
                    break;
                case Keys.F9:
                    if (btnMode.Enabled) btnMode_Click(null, null);
                    break;
                case Keys.F12:
                    if (btnExport.Enabled) btnExport_Click(null, null);
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtFind.IsEmpty) btnFind_Click(null, null);
        }

        private void mnuShow_CheckedChanged(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            if (!mnuShowA.Checked && !mnuShowI.Checked)
                mnuShowA.Checked = true;
            RefreshGrid();
            Cursor = Cursors.Default;
        }

        private void Dirty_TextChanged(object sender, EventArgs e)
        {
            IsDirty = true;
        }

        private void frmUnitMeasureList_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsDirty)
            {
                switch (MessageFacade.Show(MessageFacade.save_confirmation, LabelFacade.sys_close, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case System.Windows.Forms.DialogResult.Yes: // Save then close
                        if (!Save())
                            e.Cancel = true;
                        break;
                    case System.Windows.Forms.DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
            if (e.Cancel) return;
            IsDirty = false;
            if (btnUnlock.Text == LabelFacade.sys_button_cancel)
                btnUnlock_Click(null, null);
            if (!IsExpand)
                ConfigFacade.Set(Name + Constant.Splitter_Distance, splitContainer1.SplitterDistance);
            FormFacade.SaveFormSate(this);
        }

        private void txtCode_Leave(object sender, EventArgs e)
        {
            // Check if entered code already exists
            if (txtCode.ReadOnly) return;
            if (CategoryFacade.Exists(txtCode.Text.Trim()))
            {
                MessageFacade.Show(this, ref fMsg, LabelFacade.sys_msg_prefix + MessageFacade.code_already_exists, LabelFacade.SYS_Branch);
            }
        }

        private void btnMode_Click(object sender, EventArgs e)
        {
            splitContainer1.IsSplitterFixed = !IsExpand;
            if (!IsExpand)
            {
                ConfigFacade.SetSplitterDistance(Name, splitContainer1.SplitterDistance);
                splitContainer1.SplitterDistance = splitContainer1.Size.Width;
                splitContainer1.FixedPanel = FixedPanel.Panel2;
            }
            else
            {
                splitContainer1.SplitterDistance = ConfigFacade.GetSplitterDistance(Name);
                splitContainer1.FixedPanel = FixedPanel.Panel1;
            }
            dgvList.ShowLessColumns(IsExpand);
            IsExpand = !IsExpand;
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            mnuShow.Show(btnFilter, 0, 27);
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            RefreshGrid();
            txtFind.Focus();
            Cursor = Cursors.Default;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtFind.Clear();
            txtFind.Focus();
        }

        private void dgvList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && btnDelete.Enabled)
                btnDelete_Click(null, null);
            if (IsDlg && e.KeyCode == Keys.Enter)
                btnSelect_Click(null, null);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            Application.DoEvents();
            CategoryFacade.Export();
            Cursor = Cursors.Default;
        }

        private void lblSearch_Click(object sender, EventArgs e)
        {
            txtFind.Focus();
        }

        private void txtFind_Enter(object sender, EventArgs e)
        {
            lblSearch.Visible = false;
        }

        private void txtFind_Leave(object sender, EventArgs e)
        {
            lblSearch.Visible = (txtFind.IsEmpty);
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            Id = dgvList.Id;
            Code = dgvList.CurrentRow.Cells["colCode"].Value.ToString();
            Description = dgvList.CurrentRow.Cells["colDescription"].Value.ToString();
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void txtCode_Enter(object sender, EventArgs e)
        {
            Language.SwitchToEN();
        }
    }
}