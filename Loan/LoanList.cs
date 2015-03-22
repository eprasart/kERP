using System;
using System.Windows.Forms;
using kERP.SM;
using kERP.SYS;
using System.Text;
using System.Drawing;
using Microsoft.Reporting.WinForms;

namespace kERP
{
    public partial class frmLoan : Form
    {
        long Id = 0;
        int RowIndex = 0;   // Current gird selected row
        bool IsExpand = false;
        bool IsDirty = false;
        bool IsIgnore = true;

        string customerNo;

        DateTime dteMaturity;
        string ModuleName = "Loan";
        string TitleLabel = LoanFacade.TitleLabel;

        public frmLoan()
        {
            InitializeComponent();
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

        private DateTime GetNextRepaymentDate(DateTime dteCurrent, int nNo = 1)
        {
            string sUnit = cboFrequencyUnit.Value;
            int nFrequency = int.Parse(txtFrequency.Text);
            DateTime dteRepayment = dteCurrent;
            switch (sUnit)
            {
                case "W":
                    dteRepayment = dteCurrent.AddDays(7 * nNo * nFrequency);
                    break;
                case "M":
                    dteRepayment = dteCurrent.AddMonths(nNo * nFrequency);
                    break;
            }
            return dteRepayment;
        }

        // First installment & Maturity date
        private void CalculateDates()
        {
            if (!Util.IsInteger(txtFrequency.Text) || !Util.IsInteger(txtInstallmentNo.Text)) return;

            int nFrequency = int.Parse(txtFrequency.Text);
            int nInstallmentNo = int.Parse(txtInstallmentNo.Text);

            if (!dtpFirstInstallment.Checked)
            {
                dtpFirstInstallment.Value = GetNextRepaymentDate(dtpDisburse.Value);
                dtpFirstInstallment.Checked = false;
            }
            dteMaturity = GetNextRepaymentDate(dtpFirstInstallment.Value, nInstallmentNo - 1);
            txtMaturity.Text = dteMaturity.ToString("ddd dd-MM-yy");
        }

        private double GetRatePerDay(string sUnit, double Rate)
        {
            double r = 0;
            switch (sUnit)
            {
                case "W":
                    r = Rate / 7;
                    break;
                case "M":
                    r = Rate / 30;
                    break;
            }
            return r;
        }

        private int GetTotalDays(DateTime d1, DateTime d2)
        {
            return (int)(d2 - d1).TotalDays;
        }

        private void AddRow(int no, DateTime repayment, double principal, double interest, double total, double outstanding)
        {
            dgvSchedule.Rows.Add();
            var row = dgvSchedule.Rows[no - 1];
            row.Cells["colNo"].Value = no;
            row.Cells["colDate"].Value = repayment;
            row.Cells["colPrin"].Value = principal;
            row.Cells["colInt"].Value = interest;
            row.Cells["colTotal"].Value = total;
            row.Cells["colOutstanding"].Value = outstanding;
        }

        private void GenerateSchedule()
        {
            var valid = new Validator(this, "loan");
            string No = txtAccountNo.Text.Trim();
            if (cboFrequencyUnit.Unspecified) valid.Add(cboFrequencyUnit, "frequency_unit_unspecified");
            if (!Util.IsInteger(txtFrequency.Text)) valid.Add(txtFrequency, "frequency_invalid");
            if (!Util.IsInteger(txtInstallmentNo.Text)) valid.Add(txtInstallmentNo, "installment_no_invalid");
            if (!Util.IsDouble(txtAmount.Text)) valid.Add(txtAmount, "amount_invalid");
            if (cboCurrency.Unspecified) valid.Add(cboCurrency, "currency_unspecified");
            if (cboProduct.Unspecified) valid.Add(cboProduct, "product_unspecified");
            if (!Util.IsDouble(txtInterestRate.Text)) valid.Add(txtInterestRate, "interest_rate_invalid");
            if (Id == 0 && dtpFirstInstallment.Checked && dtpFirstInstallment.Value <= dtpDisburse.Value) valid.Add(dtpFirstInstallment, "first_installment_date_invalid");
            if (!valid.Show(LabelFacade.Get("loan_generate_schedule", "Generate Schedule")))
                return;
            else
                Validator.Close(this);

            Cursor = Cursors.WaitCursor;
            CalculateDates();
            dgvSchedule.DataSource = null;
            dgvSchedule.Rows.Clear();

            int nInstallmentNo = int.Parse(txtInstallmentNo.Text);
            string sFrequencyUnit = cboFrequencyUnit.Value;
            double dAmount = double.Parse(txtAmount.Text);
            double dInterestRate = double.Parse(txtInterestRate.Text);
            if (dInterestRate >= 1) dInterestRate /= 100;
            double dPrincipalPay = 0;
            double dInterestRatePerDay = GetRatePerDay(sFrequencyUnit, dInterestRate);
            double dPrincipalOut = dAmount;
            DateTime dtePrevious = dtpDisburse.Value;
            DateTime dteFirstInstallment = dtpFirstInstallment.Value;
            DateTime dteRepayment = dteFirstInstallment;

            int nDayNum = GetTotalDays(dtePrevious, dteRepayment);
            double dInterestPay = 0;
            double dTotalPay = 0;
            double dGrandTotalPay = 0;
            double dPMT = 0;   // for EMI
            string sProd = cboProduct.Value;    // Calculation method            
            CurrencyFacade.LoadSetting(cboCurrency.Value);
            var prod = ProductFacade.Select(sProd);


            if (sProd != "FI")
                dInterestPay = CurrencyFacade.Round(dPrincipalOut * nDayNum * dInterestRatePerDay, prod.Interest_Round_Rule);
            else    // == "FI", Flat interest ( * original amount)
                dInterestPay = dAmount * nDayNum * dInterestRatePerDay;
            switch (sProd)
            {
                case "FI":
                case "D":
                    dPrincipalPay = CurrencyFacade.Round(dAmount / nInstallmentNo, prod.Principal_Round_Rule);
                    dTotalPay = dPrincipalPay + dInterestPay;
                    break;
                case "EMI":
                    dPMT = ScheduleFacade.EMI(dAmount, dInterestRate, nInstallmentNo);
                    dTotalPay = CurrencyFacade.Round(dPMT, prod.Total_Round_Rule);
                    dPrincipalPay = dTotalPay - dInterestPay;
                    break;
            }
            //dTotalPay = dTotalPay;
            AddRow(1, dteRepayment, dPrincipalPay, dInterestPay, dTotalPay, dPrincipalOut);

            dGrandTotalPay = dPrincipalPay + dInterestPay;
            dPrincipalOut -= dPrincipalPay;
            dtePrevious = dteRepayment;

            for (int i = 2; i < nInstallmentNo; i++)
            {
                dteRepayment = DateFacade.GetNextWorkingDay(GetNextRepaymentDate(dteFirstInstallment, i - 1), prod.Never_On, prod.Non_Working_Day_Move);
                nDayNum = GetTotalDays(dtePrevious, dteRepayment);
                if (sProd != "FI")
                    dInterestPay = dPrincipalOut * nDayNum * dInterestRatePerDay;
                else  // == "FI", Flat interest ( * original amount)
                    dInterestPay = dAmount * nDayNum * dInterestRatePerDay;
                dInterestPay = CurrencyFacade.Round(dInterestPay, prod.Interest_Round_Rule);
                switch (sProd)
                {
                    case "FI":
                    case "D":
                        dTotalPay = dPrincipalPay + dInterestPay;
                        break;
                    case "EMI":
                        dTotalPay = dPMT;
                        dPrincipalPay = dTotalPay - dInterestPay;
                        break;
                }
                dTotalPay = CurrencyFacade.Round(dTotalPay, prod.Total_Round_Rule);
                AddRow(i, dteRepayment, dPrincipalPay, dInterestPay, dTotalPay, dPrincipalOut);

                dPrincipalOut -= dPrincipalPay;
                dtePrevious = dteRepayment;
            }
            nDayNum = GetTotalDays(dtePrevious, dteMaturity);
            if (sProd != "FI")
                dInterestPay = dPrincipalOut * nDayNum * dInterestRatePerDay;
            else  // == "FI", Flat interest ( * original amount)
                dInterestPay = dAmount * nDayNum * dInterestRatePerDay;
            dInterestPay = CurrencyFacade.Round(dInterestPay, prod.Interest_Round_Rule);
            switch (sProd)
            {
                case "FI":
                case "D":
                    dTotalPay = dPrincipalPay + dInterestPay;
                    break;
                case "EMI":
                    dTotalPay = dPMT;
                    dPrincipalPay = dTotalPay - dInterestPay;
                    break;
            }
            dTotalPay = CurrencyFacade.Round(dTotalPay, prod.Total_Round_Rule);
            AddRow(nInstallmentNo, dteMaturity, dPrincipalPay, dInterestPay, dTotalPay, 0);

            if (ModifierKeys != Keys.Control)   // Switch to Schedule tab
                tabControl1.SelectedIndex = 1;
            IsDirty = true;
            Cursor = Cursors.Default;
        }

        private void RefreshGrid(long seq = 0)
        {
            Cursor = Cursors.WaitCursor;
            //IsIgnore = true;
            if (dgvList.SelectedRows.Count > 0) RowIndex = dgvList.SelectedRows[0].Index;
            try
            {
                dgvList.DataSource = LoanFacade.GetDataTable(txtFind.Text, GetStatus());
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
            txtFrequency.ReadOnly = l;
            cboFrequencyUnit.Enabled = !l;
            cboPurpose.Enabled = !l;
            cboPaymentSite.Enabled = !l;
            cboAgent.Enabled = !l;
            cboCurrency.Enabled = !l;
            cboProduct.Enabled = !l;
            dtpDisburse.Enabled = !l;
            dtpFirstInstallment.Enabled = !l;
            txtInstallmentNo.ReadOnly = l;
            txtAmount.ReadOnly = l;
            txtInterestRate.ReadOnly = l;
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

            btnBrowse.Enabled = !l;
            btnGenerate.Enabled = !l;
            btnPreview.Enabled = l;
            Validator.Close(this);
        }

        private void SetStatus(string stat)
        {
            if (stat == Constant.RecordStatus_Active)
            {
                if (btnActive.Text == LabelFacade.sys_button_inactive) return;
                btnActive.Text = LabelFacade.sys_button_inactive;
                if (btnActive.Image != Properties.Resources.Inactive)
                    btnActive.Image = Properties.Resources.Inactive;
            }
            else
            {
                if (btnActive.Text == LabelFacade.sys_button_active) return;
                btnActive.Text = LabelFacade.sys_button_active;
                if (btnActive.Image != Properties.Resources.Active)
                    btnActive.Image = Properties.Resources.Active;
            }
        }

        private bool IsValidated()
        {
            var valid = new Validator(this, "loan");
            string No = txtAccountNo.Text.Trim();
            if (cboFrequencyUnit.Unspecified) valid.Add(cboFrequencyUnit, "frequency_unit_unspecified");
            if (!Util.IsInteger(txtFrequency.Text)) valid.Add(txtFrequency, "frequency_invalid");
            if (!Util.IsInteger(txtInstallmentNo.Text)) valid.Add(txtInstallmentNo, "installment_no_invalid");
            if (!Util.IsDouble(txtAmount.Text)) valid.Add(txtAmount, "amount_invalid");
            if (cboCurrency.Unspecified) valid.Add(cboCurrency, "currency_unspecified");
            if (cboProduct.Unspecified) valid.Add(cboProduct, "product_unspecified");
            if (!Util.IsDouble(txtInterestRate.Text)) valid.Add(txtInterestRate, "interest_rate_invalid");
            if (Id == 0 && dtpFirstInstallment.Checked && dtpFirstInstallment.Value <= dtpDisburse.Value) valid.Add(dtpFirstInstallment, "first_installment_date_invalid");
            if (cboPaymentSite.Unspecified) valid.Add(cboPaymentSite, "payment_site_unspecified");
            if (cboAgent.Unspecified) valid.Add(cboAgent, "credit_agent_unspecified");
            return valid.Show();
        }

        private void ClearAllBoxes()
        {
            txtAccountNo.Text = "";
            txtAccountNo.Focus();
            txtFrequency.Text = "1";
            txtInstallmentNo.Text = "";
            txtAmount.Text = "";
            txtInterestRate.Text = "";
            dtpDisburse.Value = DateTime.Today;
            dtpFirstInstallment.Checked = false;
            txtMaturity.Text = "";
            txtCustomerName.Text = "";
            cboPaymentSite.SelectedIndex = -1;
            cboAgent.SelectedIndex = -1;
            txtAccountStatus.Text = "";
            txtNote.Text = "";
            dgvSchedule.DataSource = null;
            dgvSchedule.Rows.Clear();
            IsDirty = false;
        }

        private void LoadData()
        {
            var Id = dgvList.Id;
            if (Id != 0)
                try
                {
                    var m = LoanFacade.Select(Id);
                    txtAccountNo.Text = m.Account_No;
                    customerNo = m.Customer_No;
                    txtCustomerName.Text = dgvList.CurrentRow.Cells["colName"].Value.ToString();
                    cboFrequencyUnit.Value = m.Frequency_Unit;
                    txtFrequency.Text = m.Frequency.ToString(txtFrequency.Format);
                    txtInstallmentNo.Text = m.Installment_No.ToString(txtInstallmentNo.Format);
                    txtAmount.Text = m.Amount.ToString(txtAmount.Format);
                    cboCurrency.Value = m.Currency;
                    txtInterestRate.Text = m.Interest_Rate.ToString(txtInterestRate.Format);
                    cboProduct.Value = m.Product;
                    dtpDisburse.Value = m.Disburse_Date;
                    dtpFirstInstallment.Value = m.First_Installment_Date;
                    dtpFirstInstallment.Checked = (dtpFirstInstallment.Value != GetNextRepaymentDate(dtpDisburse.Value));
                    txtMaturity.Text = m.Maturity_Date.ToString("ddd dd-MM-yy");
                    txtAccountNo.Text = m.Account_No;
                    cboPurpose.Value = m.Purpose;
                    cboPaymentSite.Value = m.Payment_Site;
                    cboAgent.SelectedValue = m.Credit_Agent_Id;
                    txtAccountStatus.Text = m.Account_Status;
                    SetStatus(m.Status);
                    // Schedule
                    CurrencyFacade.LoadSetting(cboCurrency.Value);
                    colPrin.DefaultCellStyle.Format = CurrencyFacade.Format;
                    colInt.DefaultCellStyle.Format = CurrencyFacade.Format;
                    colTotal.DefaultCellStyle.Format = CurrencyFacade.Format;
                    colOutstanding.DefaultCellStyle.Format = CurrencyFacade.Format;
                    dgvSchedule.DataSource = ScheduleFacade.GetDataTable(m.Account_No);
                    LockControls();
                    IsDirty = false;
                    SessionLogFacade.Log(Constant.Priority_Information, ModuleName, Constant.Log_View, "View. Id=" + m.Id + ", Account No.=" + m.Account_No);
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
            var prefix = "loan_";
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

            colAccountNo.HeaderText = LabelFacade.Get(prefix + "code") ?? colAccountNo.HeaderText;
            lblName.Text = LabelFacade.Get(prefix + "default_factor") ?? lblName.Text;
            glbGeneral.Caption = LabelFacade.Get(prefix + "general") ?? glbGeneral.Caption;
            glbNote.Caption = LabelFacade.Get(prefix + "note") ?? glbNote.Caption;
            //todo: Label for the rest
        }

        private bool Save()
        {
            if (!IsValidated()) return false;
            Cursor = Cursors.WaitCursor;
            // Loan account
            var m = new Loan();
            var log = new SessionLog { Module = ModuleName };
            m.Id = Id;
            m.Customer_No = customerNo;
            m.Frequency_Unit = cboFrequencyUnit.Value;
            m.Frequency = int.Parse(txtFrequency.Text);
            m.Installment_No = int.Parse(txtInstallmentNo.Text);
            m.Amount = double.Parse(txtAmount.Text);
            m.Currency = cboCurrency.Value;
            m.Interest_Rate = double.Parse(txtInterestRate.Text);
            m.Product = cboProduct.Value;
            m.Disburse_Date = dtpDisburse.Value;
            m.First_Installment_Date = dtpFirstInstallment.Value;
            m.Maturity_Date = dteMaturity;
            m.Account_No = txtAccountNo.Text;
            m.Purpose = cboPurpose.Value;
            m.Payment_Site = cboPaymentSite.Value;
            m.Credit_Agent_Id = int.Parse(cboAgent.Value);
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
                m.Id = LoanFacade.Save(m);
            }
            catch (Exception ex)
            {
                MessageFacade.Show(MessageFacade.error_save + "\r\n" + ex.Message, LabelFacade.sys_save, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrorLogFacade.Log(ex);
            }
            // Schedule
            if (dgvSchedule.RowCount == 0) btnSchedule_Click(null, null);
            //if (Id != 0 && dgvSchedule.Rows[0].Cells[0].Value != null)
            ScheduleFacade.Delete(m.Account_No);    // Better to always delete to make sure, even if not exist yet
            for (int i = 0; i < dgvSchedule.RowCount; i++)
            {
                var row = dgvSchedule.Rows[i];
                var s = new Schedule();
                s.account_no = m.Account_No;
                s.no = int.Parse(row.Cells["colNo"].Value.ToString());
                s.date = (DateTime)row.Cells["colDate"].Value;
                s.principal = double.Parse(row.Cells["colPrin"].Value.ToString());
                s.interest = double.Parse(row.Cells["colInt"].Value.ToString());
                s.total = double.Parse(row.Cells["colTotal"].Value.ToString());
                s.outstanding = double.Parse(row.Cells["colOutstanding"].Value.ToString());
                ScheduleFacade.Save(s);
            }
            if (dgvList.CurrentRow != null) RowIndex = dgvList.CurrentRow.Index;
            RefreshGrid(m.Id);
            LockControls();
            Cursor = Cursors.Default;
            log.Message = "Saved. Id=" + m.Id + ", Account No.=" + m.Account_No;
            SessionLogFacade.Log(log);
            IsDirty = false;
            return true;
        }

        private void frmLoanList_Load(object sender, EventArgs e)
        {
            try
            {
                dgvList.ShowLessColumns(true);
                SetSettings();
                SetLabels();
                DataFacade.LoadList(cboFrequencyUnit, "frequency_unit");
                DataFacade.LoadProduct(cboProduct);
                DataFacade.LoadList(cboPurpose, "loan_purpose");
                DataFacade.LoadList(cboPaymentSite, "payment_site");
                DataFacade.LoadAgent(cboAgent);
                CurrencyFacade.LoadList(cboCurrency);

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
            cboPurpose.Focus();
            cboFrequencyUnit_SelectedIndexChanged(null, null);
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
            SessionLogFacade.Log(Constant.Priority_Information, ModuleName, Constant.Log_SaveAndNew, "Saved and new. Id=" + dgvList.Id + ", Code=" + txtAccountNo.Text);
            btnSave_Click(sender, e);
            if (btnSaveNew.Enabled) return;
            btnNew_Click(sender, e);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var Id = dgvList.Id;
                if (Id == 0) return;
                // If referenced
                //todo: check if exist in ic_item
                // If locked
                var lInfo = LoanFacade.GetLock(Id);
                string msg = "";
                if (lInfo.Locked)
                {
                    msg = string.Format(MessageFacade.delete_locked, lInfo.Lock_By, lInfo.Lock_At);
                    if (!Privilege.CanAccess(Constant.Function_IC_Unit_Measure, "O"))
                    {
                        MessageFacade.Show(msg, LabelFacade.sys_delete, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        SessionLogFacade.Log(Constant.Priority_Caution, ModuleName, Constant.Log_Delete, "Cannot delete. Currently locked by '" + lInfo.Lock_By + "' since '" + lInfo.Lock_At + "' . Id=" + dgvList.Id + ", Code=" + txtAccountNo.Text);
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
                    LoanFacade.SetStatus(Id, Constant.RecordStatus_Deleted);
                }
                catch (Exception ex)
                {
                    MessageFacade.Show(MessageFacade.error_delete + "\r\n" + ex.Message, LabelFacade.sys_delete, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ErrorLogFacade.Log(ex);
                }
                RefreshGrid();
                // log
                SessionLogFacade.Log(Constant.Priority_Warning, ModuleName, Constant.Log_Delete, "Deleted. Id=" + dgvList.Id + ", Code=" + txtAccountNo.Text);
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
            txtAccountNo.Focus();
            LockControls(false);
            cboFrequencyUnit_SelectedIndexChanged(null, null);
            SessionLogFacade.Log(Constant.Priority_Information, ModuleName, Constant.Log_Copy, "Copy from Id=" + dgvList.Id + "Code=" + txtAccountNo.Text);
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
            var Id = dgvList.Id;
            if (Id == 0) return;

            string status = btnActive.Text == LabelFacade.sys_button_inactive ? Constant.RecordStatus_InActive : Constant.RecordStatus_Active;
            // If referenced
            //todo: check if already used in ic_item

            //If locked
            var lInfo = LoanFacade.GetLock(Id);
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
                LoanFacade.SetStatus(Id, status);
            }
            catch (Exception ex)
            {
                MessageFacade.Show(MessageFacade.error_active_inactive + ex.Message, MessageFacade.active_inactive, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrorLogFacade.Log(ex);
            }
            RefreshGrid();
            SessionLogFacade.Log(Constant.Priority_Caution, ModuleName, status == Constant.RecordStatus_InActive ? Constant.Log_Inactive : Constant.Log_Active, "Id=" + dgvList.Id + ", Code=" + txtAccountNo.Text);
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
                LoanFacade.DecrementSrNo(cboFrequencyUnit.Value);
                LockControls(true);
                dgvList.Focus();
                try
                {
                    LoanFacade.ReleaseLock(dgvList.Id);
                }
                catch (Exception ex)
                {
                    MessageFacade.Show(MessageFacade.error_unlock + "\r\n" + ex.Message, LabelFacade.sys_unlock, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ErrorLogFacade.Log(ex);
                    return;
                }
                if (dgvList.CurrentRow != null && !dgvList.CurrentRow.Selected)
                    dgvList.CurrentRow.Selected = true;
                SessionLogFacade.Log(Constant.Priority_Information, ModuleName, Constant.Log_Unlock, "Unlock cancel. Id=" + dgvList.Id + ", Code=" + txtAccountNo.Text);
                btnUnlock.ToolTipText = "Unlock (Ctrl+L)";
                IsDirty = false;
                return;
            }
            // Unlock
            if (Id == 0) return;
            try
            {
                var lInfo = LoanFacade.GetLock(Id);

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
                            SessionLogFacade.Log(Constant.Priority_Caution, ModuleName, Constant.Log_Lock, "Override lock. Id=" + dgvList.Id + ", Code=" + txtAccountNo.Text);
                        else
                            return;
                }
                txtMaturity.SelectionStart = txtMaturity.Text.Length;
                txtMaturity.Focus();
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
                LoanFacade.Lock(dgvList.Id, txtAccountNo.Text);
            }
            catch (Exception ex)
            {
                MessageFacade.Show(MessageFacade.error_lock + "\r\n" + ex.Message, LabelFacade.sys_lock, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrorLogFacade.Log(ex);
                return;
            }
            SessionLogFacade.Log(Constant.Priority_Information, ModuleName, Constant.Log_Lock, "Locked. Id=" + dgvList.Id + ", Code=" + txtAccountNo.Text);
            btnUnlock.ToolTipText = "Cancel (Esc or Ctrl+L)";
            IsDirty = false;
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

        private void frmLoanList_FormClosing(object sender, FormClosingEventArgs e)
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
            //// Check if entered code already exists
            //if (txtNo.ReadOnly) return;
            //if (LoanFacade.Exists(txtNo.Text.Trim()))
            //{
            //    MessageFacade.Show(this, ref fMsg, LabelFacade.sy_msg_prefix + MessageFacade.code_already_exists, LabelFacade.sy_customer);
            //}
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
                splitContainer1.SplitterDistance = ConfigFacade.GetSplitterDistance(Name); //ConfigFacade.ic_unit_measure_splitter_distance;
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
            LoanFacade.Export();
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

        private void cboFrequencyUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (cboFrequencyUnit.UnSpecified || btnNew.Enabled) return;
            //txtAccountNo.Text = LoanFacade.GetNextAccountNo(cboFrequencyUnit.Value); //todo: Format No; from table
        }

        private void txtMaturity_Enter(object sender, EventArgs e)
        {
            CalculateDates();
        }

        private void btnSchedule_Click(object sender, EventArgs e)
        {
            GenerateSchedule();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            GenerateSchedule();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            if (dgvSchedule.RowCount == 0) btnSchedule_Click(null, null);
            var fReport = new frmReport("Repayment Schedule");
            fReport.FileName = "Schedule.rdlc";
            fReport.AddParameter("Path", "file:" + App.StartupPath);
            var sql = "select format, case frequency_unit when 'M' then 'ខែ' when 'W' then 'សប្តាហ៌' when 'D' then 'ថ្ងៃ' end frequency_unit, frequency, " +
                "amount, cy.name_khm currency, interest_rate, da.day_short || ' ' || to_char(disburse_date, 'dd-MM-yy') disburse_date, maturity_date, site.description payment_site, a.name credit_agent_name, phone credit_agent_phone," +
                "\nlast_name || ' ' || first_name customer_name," +
                "\nl.account_no, d.day_short || ' ' || to_char(date, 'dd-MM-yy') date, no, principal, interest, total, outstanding, pay_off" +
                "\nfrom schedule s" +
                "\ninner join loan l on s.account_no = l.account_no" +
                "\ninner join customer c on l.customer_no = c.customer_no" +
                "\ninner join agent a on l.credit_agent_id = a.id" +
                "\ninner join day d on extract(dow from date) = d.code" +
                "\ninner join day da on extract(dow from disburse_date) = da.code" +
                "\ninner join currency cy on l.currency = cy.code" +
                "\ninner join list site on field = 'payment_site' and l.payment_site = site.code" +
                "\nwhere l.account_no = :account_no\norder by no";
            var cmd = new Npgsql.NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue(":account_no", txtAccountNo.Text);
            fReport.ReportSource = SqlFacade.GetDataTable(cmd);
            fReport.PreviewReport();
            Cursor = Cursors.Default;
        }
    }
}