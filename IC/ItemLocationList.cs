using System;
using System.Windows.Forms;
using kERP.SM;
using kERP.SYS;
using System.Text;
using System.Drawing;

namespace kERP
{
    public partial class frmItemLocation : Form
    {
        long Id = 0;
        int RowIndex = 0;   // Current gird selected row
        bool IsExpand = false;
        bool IsDirty = false;
        bool IsIgnore = true;

        string ItemCode = "";
        string LocationCode = "";
        string SupplierCode = "";

        string ModuleName = "IC Item Location";
        string TitleLabel = ItemLocationFacade.TitleLabel;

        public frmItemLocation()
        {
            InitializeComponent();
        }

        private void LoadImages()
        {
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

            btnItem.Image = btnFind.Image;
            btnLocation.Image = btnFind.Image;
            btnSupplier.Image = btnFind.Image;
        }

        private string GetStatus()
        {
            var status = "";
            if (mnuActive.Checked && !mnuInactive.Checked)
                status = Constant.RecordStatus_Active;
            else if (mnuInactive.Checked && !mnuActive.Checked)
                status = Constant.RecordStatus_InActive;
            return status;
        }

        private string GetSearchCols()
        {
            var cols = "";
            if (mnuItem.Checked)
                cols = "i.code, i.description, i.description2, i.barcode, i.upc_code";
            if (mnuLocation.Checked)
            {
                if (cols.Length > 0) cols += ", ";
                cols += "l.code, l.description";
            }
            return cols;
        }

        private void RefreshGrid(long seq = 0)
        {
            Cursor = Cursors.WaitCursor;
            //IsIgnore = true;
            if (dgvList.SelectedRows.Count > 0) RowIndex = dgvList.SelectedRows[0].Index;
            try
            {
                dgvList.DataSource = ItemLocationFacade.GetDataTable(txtFind.Text, GetStatus(), GetSearchCols());
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
            txtItem.ReadOnly = Id != 0;
            btnItem.Enabled = Id == 0;
            txtLocation.ReadOnly = Id != 0;
            btnLocation.Enabled = Id == 0;
            txtSupplier.ReadOnly = l;
            btnSupplier.Enabled = !l;
            txtLeadTime.ReadOnly = l;
            txtMinQty.ReadOnly = l;
            txtMaxQty.ReadOnly = l;
            txtStdCost.ReadOnly = l;
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
            Validator.Close(this);
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
            var valid = new Validator(this, "ic_item_location");
            if (ItemCode.Length == 0) valid.Add(txtItem, "item_unspecified");
            if (LocationCode.Length == 0) valid.Add(txtLocation, "location_unspecified");
            if (ItemLocationFacade.Exists(ItemCode, LocationCode, Id)) valid.Add(txtItem, "item_location_exists");
            if (!Util.IsDouble(txtLeadTime.Text)) valid.Add(txtLeadTime, "deliver_lead_time_invalid");
            if (!Util.IsDouble(txtMinQty.Text)) valid.Add(txtMinQty, "min_qty_invalid");
            if (!Util.IsDouble(txtMaxQty.Text)) valid.Add(txtMaxQty, "max_qty_invalid");
            if (!Util.IsDouble(txtStdCost.Text)) valid.Add(txtStdCost, "standard_cost_invalid");
            return valid.Show();
        }

        private void ClearAllBoxes()
        {
            txtItem.Text = "";
            txtItem.Focus();
            txtBarcode.Text = "";
            txtLocation.Text = "";
            txtSupplier.Text = "";
            txtLeadTime.Text = "0";
            txtMinQty.Text = "0";
            txtMaxQty.Text = "0";
            txtStdCost.Text = "0";
            txtNote.Text = "";
            IsDirty = false;
        }

        private void LoadData()
        {
            Id = dgvList.Id;
            if (Id != 0)
                try
                {
                    var m = ItemLocationFacade.Select(Id);
                    ItemCode = m.item_code;
                    var i = ItemFacade.SelectLessCols(ItemCode);
                    txtItem.Text = Util.ConcatCodeDescription(m.item_code, i.Description);
                    txtBarcode.Text = i.Barcode;
                    txtLocation.Text = Util.ConcatCodeDescription(m.location_code, LocationFacade.GetDescription(m.location_code));
                    LocationCode = m.location_code;
                    txtSupplier.Text = m.default_supplier_code.Length > 0 ? Util.ConcatCodeDescription(m.default_supplier_code, SupplierFacade.GetDescription(m.default_supplier_code)) : "";
                    SupplierCode = m.default_supplier_code;
                    txtLeadTime.Text = m.lead_time.ToString();
                    txtMinQty.Text = m.min_qty.ToString();
                    txtMaxQty.Text = m.max_qty.ToString();
                    txtStdCost.Text = m.std_cost.ToString();
                    txtOnhand.Text = m.onhand.ToString();
                    txtAvgCost.Text = m.avg_cost.ToString();
                    txtLastCost.Text = m.last_cost.ToString();
                    //txtStockUoM.Text=
                    txtLastSupplier.Text = Util.ConcatCodeDescription(m.last_supplier_code, SupplierFacade.GetDescription(m.last_supplier_code));
                    txtNote.Text = m.Note;
                    SetStatus(m.Status);
                    LockControls();
                    IsDirty = false;
                    SessionLogFacade.Log(Constant.Priority_Information, ModuleName, Constant.Log_View, "View. Id=" + m.Id + ", Item Code=" + m.item_code + ", Location Code=" + m.location_code);
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
                FormFacade.SetFormState(this);
            }
            catch (Exception ex)
            {
                ErrorLogFacade.Log(ex, "Set settings");
            }
        }

        private void SetLabels()
        {
            var prefix = "ic_item_location_";
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


        private void CheckExists()
        {        // Check if entered code already exists
            if (txtItem.ReadOnly) return;
            if (ItemCode.Length > 0 && LocationCode.Length > 0 && ItemLocationFacade.Exists(ItemCode, LocationCode, Id))
            {
                var valid = new Validator(this, "item_location");
                valid.Add(txtItem, "item_invalid");
            }
            else
                Validator.Close(this);
        }

        private void ShowItem(string search = "")
        {
            var f = new frmItem();
            f.IsDlg = true;
            f.SearchText = search;
            if (f.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            ItemCode = f.Code;
            var m = ItemFacade.SelectLessCols(f.Id);
            txtItem.Text = Util.ConcatCodeDescription(m.Code, m.Description);
            txtBarcode.Text = m.Barcode;
            txtLocation.Focus();
        }

        private void ShowLocation(string search = "")
        {
            var f = new frmLocation();
            f.IsDlg = true;
            f.SearchText = search;
            if (f.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            LocationCode = f.Code;
            var m = LocationFacade.SelectLessCols(f.Id);
            txtLocation.Text = Util.ConcatCodeDescription(m.Code, m.Description);
            txtLocation.Focus();
        }

        private void ShowSupplier(string search = "")
        {
            var f = new frmItem();
            f.IsDlg = true;
            f.SearchText = search;
            if (f.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            ItemCode = f.Code;
            var m = ItemFacade.SelectLessCols(f.Id);
            txtItem.Text = Util.ConcatCodeDescription(m.Code, m.Description);
            txtBarcode.Text = m.Barcode;
            txtLocation.Focus();
        }

        private bool Save()
        {
            if (!IsValidated()) return false;
            Cursor = Cursors.WaitCursor;
            var m = new ItemLocation();
            var log = new SessionLog { Module = ModuleName };
            m.Id = Id;
            m.item_code = ItemCode;
            m.location_code = LocationCode;
            m.default_supplier_code = SupplierCode;
            m.lead_time = double.Parse(txtLeadTime.Text);
            m.min_qty = double.Parse(txtMinQty.Text);
            m.max_qty = double.Parse(txtMaxQty.Text);
            m.std_cost = double.Parse(txtStdCost.Text);
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
                m.Id = ItemLocationFacade.Save(m);
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
            log.Message = "Saved. Id=" + m.Id + ", Code=" + txtItem.Text;
            SessionLogFacade.Log(log);
            IsDirty = false;
            return true;
        }

        private void frmLocationList_Load(object sender, EventArgs e)
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
            SessionLogFacade.Log(Constant.Priority_Information, ModuleName, Constant.Log_SaveAndNew, "Saved and new. Id=" + dgvList.Id + ", Code=" + txtItem.Text);
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
                var lInfo = ItemLocationFacade.GetLock(Id);
                string msg = "";
                if (lInfo.Locked)
                {
                    msg = string.Format(MessageFacade.delete_locked, lInfo.Lock_By, lInfo.Lock_At);
                    if (!Privilege.CanAccess(Constant.Function_IC_Unit_Measure, "O"))
                    {
                        MessageFacade.Show(msg, LabelFacade.sys_delete, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        SessionLogFacade.Log(Constant.Priority_Caution, ModuleName, Constant.Log_Delete, "Cannot delete. Currently locked by '" + lInfo.Lock_By + "' since '" + lInfo.Lock_At + "' . Id=" + dgvList.Id + ", Code=" + txtItem.Text);
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
                    ItemLocationFacade.SetStatus(Id, Constant.RecordStatus_Deleted);
                }
                catch (Exception ex)
                {
                    MessageFacade.Show(MessageFacade.error_delete + "\r\n" + ex.Message, LabelFacade.sys_delete, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ErrorLogFacade.Log(ex);
                }
                RefreshGrid();
                // log
                SessionLogFacade.Log(Constant.Priority_Warning, ModuleName, Constant.Log_Delete, "Deleted. Id=" + dgvList.Id + ", Code=" + txtItem.Text);
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
            txtItem.Focus();
            LockControls(false);
            SessionLogFacade.Log(Constant.Priority_Information, ModuleName, Constant.Log_Copy, "Copy from Id=" + dgvList.Id + "Code=" + txtItem.Text);
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
            if (IsExpand) picExpand_Click(sender, e);
            dgvList_SelectionChanged(sender, e);    // reload data since SelectionChanged will not occured on current row            
        }

        private void btnActive_Click(object sender, EventArgs e)
        {            
            if (Id == 0) return;
            string status = btnActive.Text == LabelFacade.sys_button_inactive ? Constant.RecordStatus_InActive : Constant.RecordStatus_Active;
            // If referenced
            //todo: check if already used in ic_item

            //If locked
            var lInfo = ItemLocationFacade.GetLock(Id);
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
                ItemLocationFacade.SetStatus(Id, status);
            }
            catch (Exception ex)
            {
                MessageFacade.Show(MessageFacade.error_active_inactive + ex.Message, MessageFacade.active_inactive, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrorLogFacade.Log(ex);
            }
            RefreshGrid();
            SessionLogFacade.Log(Constant.Priority_Caution, ModuleName, status == Constant.RecordStatus_InActive ? Constant.Log_Inactive : Constant.Log_Active, "Id=" + dgvList.Id + ", Code=" + txtItem.Text);
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
                    ItemLocationFacade.ReleaseLock(dgvList.Id);
                }
                catch (Exception ex)
                {
                    MessageFacade.Show(MessageFacade.error_unlock + "\r\n" + ex.Message, LabelFacade.sys_unlock, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ErrorLogFacade.Log(ex);
                    return;
                }
                if (dgvList.CurrentRow != null && !dgvList.CurrentRow.Selected)
                    dgvList.CurrentRow.Selected = true;
                SessionLogFacade.Log(Constant.Priority_Information, ModuleName, Constant.Log_Unlock, "Unlock cancel. Id=" + dgvList.Id + ", Code=" + txtItem.Text);
                btnUnlock.ToolTipText = "Unlock (Ctrl+L)";
                IsDirty = false;
                return;
            }
            // Unlock
            if (Id == 0) return;
            try
            {
                var lInfo = ItemLocationFacade.GetLock(Id);

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
                            SessionLogFacade.Log(Constant.Priority_Caution, ModuleName, Constant.Log_Lock, "Override lock. Id=" + dgvList.Id + ", Code=" + txtItem.Text);
                        else
                            return;
                }
                txtSupplier.Focus2();
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
                ItemLocationFacade.Lock(dgvList.Id, txtItem.Text);
            }
            catch (Exception ex)
            {
                MessageFacade.Show(MessageFacade.error_lock + "\r\n" + ex.Message, LabelFacade.sys_lock, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrorLogFacade.Log(ex);
                return;
            }
            SessionLogFacade.Log(Constant.Priority_Information, ModuleName, Constant.Log_Lock, "Locked. Id=" + dgvList.Id + ", Code=" + txtItem.Text);
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
            if (!mnuActive.Checked && !mnuInactive.Checked)
                mnuActive.Checked = true;
            RefreshGrid();
            Cursor = Cursors.Default;
        }

        private void mnuSearch_CheckedChanged(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            if (!mnuItem.Checked && !mnuLocation.Checked)
            {
                mnuItem.Checked = true;
                mnuLocation.Checked = true;
            }
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
            if (e.KeyCode != Keys.Delete) return;
            if (btnDelete.Enabled) btnDelete_Click(null, null);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            Application.DoEvents();
            ItemLocationFacade.Export();
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

        private void btnItem_Click(object sender, EventArgs e)
        {
            ShowItem();
        }

        private void btnLocation_Click(object sender, EventArgs e)
        {
            var f = new frmLocation();
            f.IsDlg = true;
            if (f.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            LocationCode = f.Code;
            txtLocation.Text = Util.ConcatCodeDescription(f.Code, f.Description);
            txtSupplier.Focus();
        }

        private void btnSupplier_Click(object sender, EventArgs e)
        {
            var f = new frmSupplier();
            f.IsDlg = true;
            if (f.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            SupplierCode = f.Code;
            txtSupplier.Text = Util.ConcatCodeDescription(f.Code, f.Description);
            txtLeadTime.Focus();
        }

        private void txtItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (!btnItem.Enabled) return;
            switch (e.KeyCode)
            {
                case Keys.F2:
                    btnItem_Click(null, null);
                    break;
                case Keys.Delete:
                    txtItem.Text = "";
                    ItemCode = "";
                    break;
            }
        }

        private void txtLocation_KeyDown(object sender, KeyEventArgs e)
        {
            if (!btnLocation.Enabled) return;
            switch (e.KeyCode)
            {
                case Keys.F2:
                    btnLocation_Click(null, null);
                    break;
                case Keys.Delete:
                    txtLocation.Text = "";
                    LocationCode = "";
                    break;
            }
        }

        private void txtSupplier_KeyDown(object sender, KeyEventArgs e)
        {
            if (!btnSupplier.Enabled) return;
            switch (e.KeyCode)
            {
                case Keys.F2:
                    btnSupplier_Click(null, null);
                    break;
                case Keys.Delete:
                    txtSupplier.Text = "";
                    SupplierCode = "";
                    break;
            }
        }

        private void txtItem_Leave(object sender, EventArgs e)
        {
            Validator.Close(this);
            string sItem = txtItem.Text;
            if (txtItem.ReadOnly || txtItem.Text.Length == 0 || sItem.Contains(ConfigFacade.Code_Description_Separator)) return;
            int count = ItemFacade.GetCount(sItem);
            if (count == 1) // match 1
            {
                var m = ItemFacade.SelectLessCols(sItem);
                ItemCode = m.Code;
                txtItem.Text = Util.ConcatCodeDescription(m.Code, m.Description);
                txtBarcode.Text = m.Barcode;
            }
            else if (count > 1) // match multiple
                ShowItem(sItem);
            else    // < 0; not match
            {
                ItemCode = "";
                var valid = new Validator(this, "ic_item_location");
                valid.Add(txtItem, "item_invalid");
                valid.Show();
            }
        }

        private void txtLocation_Leave(object sender, EventArgs e)
        {
            Validator.Close(this);
            string sLocation = txtLocation.Text;
            if (txtLocation.ReadOnly || txtLocation.Text.Length == 0 || sLocation.Contains(ConfigFacade.Code_Description_Separator)) return;
            int count = LocationFacade.GetCount(sLocation);
            if (count == 1) // match 1
            {
                var m = LocationFacade.SelectLessCols(sLocation);
                LocationCode = m.Code;
                txtLocation.Text = Util.ConcatCodeDescription(m.Code, m.Description);
            }
            else if (count > 1) // match multiple
                ShowLocation(sLocation);
            else    // < 0; not match
            {
                LocationCode = "";
                var valid = new Validator(this, "ic_item_location");
                valid.Add(txtLocation, "location_invalid");
                valid.Show();
            }
        }

        private void txtSupplier_Leave(object sender, EventArgs e)
        {
            Validator.Close(this);
            string sSupplier = txtSupplier.Text;
            if (txtSupplier.ReadOnly || txtSupplier.Text.Length == 0 || sSupplier.Contains(ConfigFacade.Code_Description_Separator)) return;
            int count = SupplierFacade.GetCount(sSupplier);
            if (count == 1) // match 1
            {
                var m = SupplierFacade.SelectLessCols(sSupplier);
                SupplierCode = m.Code;
                txtSupplier.Text = Util.ConcatCodeDescription(m.Code, m.Description);
            }
            else if (count > 1) // match multiple
                ShowSupplier(sSupplier);
            else    // < 0; not match
            {
                SupplierCode = "";
                var valid = new Validator(this, "ic_item_location");
                valid.Add(txtSupplier, "location_invalid");
                valid.Show();
            }
        }

        private void SwitchToEN_Enter(object sender, EventArgs e)
        {
            Language.SwitchToEN();
        }
    }
}