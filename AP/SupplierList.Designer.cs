﻿namespace kERP
{
    partial class frmSupplier
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnSelect = new System.Windows.Forms.ToolStripButton();
            this.btnNew = new System.Windows.Forms.ToolStripButton();
            this.btnCopy = new System.Windows.Forms.ToolStripButton();
            this.btnUnlock = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnSaveNew = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.btnActive = new System.Windows.Forms.ToolStripButton();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnMode = new System.Windows.Forms.ToolStripButton();
            this.btnExport = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lblSearch = new System.Windows.Forms.Label();
            this.btnFind = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnFilter = new System.Windows.Forms.Button();
            this.dgvList = new kUI.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtFind = new kUI.TextBox(this.components);
            this.glbGeneral = new kUI.GroupLabel();
            this.glbNote = new kUI.GroupLabel();
            this.txtNote = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.cboType = new kUI.ComboBox(this.components);
            this.lblCode = new System.Windows.Forms.Label();
            this.txtEmail = new kUI.TextBox(this.components);
            this.groupLabel1 = new kUI.GroupLabel();
            this.txtFax = new kUI.TextBox(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.txtPhone = new kUI.TextBox(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.txtName = new kUI.TextBox(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtAddress = new kUI.TextBox(this.components);
            this.txtDescription = new kUI.TextBox(this.components);
            this.txtCode = new kUI.TextBox(this.components);
            this.mnuShow = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuShowA = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuShowI = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvList)).BeginInit();
            this.mnuShow.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.White;
            this.toolStrip1.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSelect,
            this.btnNew,
            this.btnCopy,
            this.btnUnlock,
            this.btnSave,
            this.btnSaveNew,
            this.toolStripSeparator,
            this.btnActive,
            this.btnDelete,
            this.toolStripSeparator1,
            this.btnMode,
            this.btnExport});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(993, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnSelect
            // 
            this.btnSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(50, 22);
            this.btnSelect.Text = "Select";
            this.btnSelect.ToolTipText = "New (Ctrl+N)";
            this.btnSelect.Visible = false;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnNew
            // 
            this.btnNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(39, 22);
            this.btnNew.Text = "&New";
            this.btnNew.ToolTipText = "New (Ctrl+N)";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Enabled = false;
            this.btnCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(44, 22);
            this.btnCopy.Text = "Cop&y";
            this.btnCopy.ToolTipText = "Copy (Ctrl+Y)";
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnUnlock
            // 
            this.btnUnlock.Enabled = false;
            this.btnUnlock.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUnlock.Name = "btnUnlock";
            this.btnUnlock.Size = new System.Drawing.Size(54, 22);
            this.btnUnlock.Text = "Unl&ock";
            this.btnUnlock.ToolTipText = "Unlock (Ctrl+L)";
            this.btnUnlock.Click += new System.EventHandler(this.btnUnlock_Click);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(44, 22);
            this.btnSave.Text = "&Save";
            this.btnSave.ToolTipText = "Save (Ctrl+S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSaveNew
            // 
            this.btnSaveNew.Enabled = false;
            this.btnSaveNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSaveNew.Name = "btnSaveNew";
            this.btnSaveNew.Size = new System.Drawing.Size(100, 22);
            this.btnSaveNew.Text = "Save and Ne&w";
            this.btnSaveNew.ToolTipText = "Save and New (Ctrl+W)";
            this.btnSaveNew.Click += new System.EventHandler(this.btnSaveNew_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // btnActive
            // 
            this.btnActive.Enabled = false;
            this.btnActive.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnActive.Name = "btnActive";
            this.btnActive.Size = new System.Drawing.Size(58, 22);
            this.btnActive.Text = "Inactiv&e";
            this.btnActive.ToolTipText = "Active/Inactive (Ctrl+E)";
            this.btnActive.Click += new System.EventHandler(this.btnActive_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(52, 22);
            this.btnDelete.Text = "Delete";
            this.btnDelete.ToolTipText = "Delete (Del)";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnMode
            // 
            this.btnMode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMode.Name = "btnMode";
            this.btnMode.Size = new System.Drawing.Size(72, 22);
            this.btnMode.Text = "List/Detail";
            this.btnMode.ToolTipText = "Toggle between list and detail mode (F9)";
            this.btnMode.Click += new System.EventHandler(this.btnMode_Click);
            // 
            // btnExport
            // 
            this.btnExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(50, 22);
            this.btnExport.Text = "Export";
            this.btnExport.ToolTipText = "Export all data to CSV file (F12)";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lblSearch);
            this.splitContainer1.Panel1.Controls.Add(this.btnFind);
            this.splitContainer1.Panel1.Controls.Add(this.btnClear);
            this.splitContainer1.Panel1.Controls.Add(this.btnFilter);
            this.splitContainer1.Panel1.Controls.Add(this.dgvList);
            this.splitContainer1.Panel1.Controls.Add(this.txtFind);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.glbGeneral);
            this.splitContainer1.Panel2.Controls.Add(this.glbNote);
            this.splitContainer1.Panel2.Controls.Add(this.txtNote);
            this.splitContainer1.Panel2.Controls.Add(this.label10);
            this.splitContainer1.Panel2.Controls.Add(this.lblDescription);
            this.splitContainer1.Panel2.Controls.Add(this.label11);
            this.splitContainer1.Panel2.Controls.Add(this.cboType);
            this.splitContainer1.Panel2.Controls.Add(this.lblCode);
            this.splitContainer1.Panel2.Controls.Add(this.txtEmail);
            this.splitContainer1.Panel2.Controls.Add(this.groupLabel1);
            this.splitContainer1.Panel2.Controls.Add(this.txtFax);
            this.splitContainer1.Panel2.Controls.Add(this.label5);
            this.splitContainer1.Panel2.Controls.Add(this.txtPhone);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.txtName);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.txtAddress);
            this.splitContainer1.Panel2.Controls.Add(this.txtDescription);
            this.splitContainer1.Panel2.Controls.Add(this.txtCode);
            this.splitContainer1.Panel2.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(8, 4, 8, 8);
            this.splitContainer1.Panel2MinSize = 0;
            this.splitContainer1.Size = new System.Drawing.Size(993, 435);
            this.splitContainer1.SplitterDistance = 228;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 1;
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.lblSearch.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSearch.ForeColor = System.Drawing.Color.DimGray;
            this.lblSearch.Location = new System.Drawing.Point(6, 5);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(51, 17);
            this.lblSearch.TabIndex = 6;
            this.lblSearch.Text = "Search";
            this.lblSearch.Click += new System.EventHandler(this.lblSearch_Click);
            // 
            // btnFind
            // 
            this.btnFind.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFind.Location = new System.Drawing.Point(1, 29);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(68, 24);
            this.btnFind.TabIndex = 5;
            this.btnFind.Text = "     Find";
            this.btnFind.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // btnClear
            // 
            this.btnClear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClear.Location = new System.Drawing.Point(69, 29);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(68, 24);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "     Clear";
            this.btnClear.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnFilter
            // 
            this.btnFilter.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFilter.Location = new System.Drawing.Point(137, 29);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(68, 24);
            this.btnFilter.TabIndex = 5;
            this.btnFilter.Text = "     Filter";
            this.btnFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFilter.UseVisualStyleBackColor = true;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // dgvList
            // 
            this.dgvList.AllowUserToAddRows = false;
            this.dgvList.AllowUserToDeleteRows = false;
            this.dgvList.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Honeydew;
            this.dgvList.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvList.BackgroundColor = System.Drawing.Color.White;
            this.dgvList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.colCode,
            this.colDescription,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column6});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.Teal;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvList.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgvList.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgvList.Location = new System.Drawing.Point(1, 54);
            this.dgvList.Margin = new System.Windows.Forms.Padding(4);
            this.dgvList.MultiSelect = false;
            this.dgvList.Name = "dgvList";
            this.dgvList.ReadOnly = true;
            this.dgvList.RowHeadersVisible = false;
            this.dgvList.RowHeadersWidth = 35;
            this.dgvList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvList.ShowEditingIcon = false;
            this.dgvList.Size = new System.Drawing.Size(223, 377);
            this.dgvList.TabIndex = 1;
            this.dgvList.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvList_CellDoubleClick);
            this.dgvList.SelectionChanged += new System.EventHandler(this.dgvList_SelectionChanged);
            this.dgvList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvList_KeyDown);
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "Id";
            this.Column1.HeaderText = "Id";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Visible = false;
            // 
            // colCode
            // 
            this.colCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colCode.DataPropertyName = "code";
            this.colCode.HeaderText = "Code";
            this.colCode.Name = "colCode";
            this.colCode.ReadOnly = true;
            this.colCode.Width = 66;
            // 
            // colDescription
            // 
            this.colDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colDescription.DataPropertyName = "description";
            dataGridViewCellStyle3.NullValue = null;
            this.colDescription.DefaultCellStyle = dataGridViewCellStyle3;
            this.colDescription.HeaderText = "Description";
            this.colDescription.Name = "colDescription";
            this.colDescription.ReadOnly = true;
            this.colDescription.Width = 101;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column2.DataPropertyName = "name";
            this.Column2.HeaderText = "Contact Name";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 118;
            // 
            // Column3
            // 
            this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column3.DataPropertyName = "phone";
            dataGridViewCellStyle4.Format = "dd-MM-yy";
            this.Column3.DefaultCellStyle = dataGridViewCellStyle4;
            this.Column3.HeaderText = "Phone";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 72;
            // 
            // Column4
            // 
            this.Column4.DataPropertyName = "fax";
            this.Column4.HeaderText = "Fax";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            // 
            // Column5
            // 
            this.Column5.DataPropertyName = "email";
            this.Column5.HeaderText = "Email";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            // 
            // Column6
            // 
            this.Column6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column6.DataPropertyName = "address";
            this.Column6.HeaderText = "Address";
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            // 
            // txtFind
            // 
            this.txtFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFind.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFind.Format = null;
            this.txtFind.Location = new System.Drawing.Point(1, 1);
            this.txtFind.Margin = new System.Windows.Forms.Padding(4);
            this.txtFind.Name = "txtFind";
            this.txtFind.Numeric = false;
            this.txtFind.Size = new System.Drawing.Size(224, 25);
            this.txtFind.TabIndex = 4;
            this.txtFind.TabOnEnter = false;
            this.toolTip1.SetToolTip(this.txtFind, "Enter query to search for");
            this.txtFind.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.txtFind.Enter += new System.EventHandler(this.txtFind_Enter);
            this.txtFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            this.txtFind.Leave += new System.EventHandler(this.txtFind_Leave);
            // 
            // glbGeneral
            // 
            this.glbGeneral.Caption = "General";
            this.glbGeneral.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.glbGeneral.Location = new System.Drawing.Point(12, 12);
            this.glbGeneral.Margin = new System.Windows.Forms.Padding(4, 13, 4, 4);
            this.glbGeneral.Name = "glbGeneral";
            this.glbGeneral.Size = new System.Drawing.Size(347, 22);
            this.glbGeneral.TabIndex = 0;
            this.glbGeneral.TabStop = false;
            // 
            // glbNote
            // 
            this.glbNote.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.glbNote.Caption = "Note";
            this.glbNote.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.glbNote.Location = new System.Drawing.Point(12, 207);
            this.glbNote.Margin = new System.Windows.Forms.Padding(4, 13, 4, 4);
            this.glbNote.Name = "glbNote";
            this.glbNote.Size = new System.Drawing.Size(723, 22);
            this.glbNote.TabIndex = 28;
            this.glbNote.TabStop = false;
            // 
            // txtNote
            // 
            this.txtNote.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNote.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNote.Location = new System.Drawing.Point(12, 238);
            this.txtNote.Margin = new System.Windows.Forms.Padding(4);
            this.txtNote.Multiline = true;
            this.txtNote.Name = "txtNote";
            this.txtNote.ReadOnly = true;
            this.txtNote.Size = new System.Drawing.Size(723, 182);
            this.txtNote.TabIndex = 16;
            this.txtNote.TextChanged += new System.EventHandler(this.Dirty_TextChanged);
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(19, 81);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(103, 18);
            this.label10.TabIndex = 2;
            this.label10.Text = "Description";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblDescription
            // 
            this.lblDescription.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescription.Location = new System.Drawing.Point(415, 46);
            this.lblDescription.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(88, 18);
            this.lblDescription.TabIndex = 8;
            this.lblDescription.Text = "Name";
            this.lblDescription.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(19, 115);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(103, 18);
            this.label11.TabIndex = 4;
            this.label11.Text = "Type";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cboType
            // 
            this.cboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboType.Enabled = false;
            this.cboType.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboType.FormattingEnabled = true;
            this.cboType.Location = new System.Drawing.Point(129, 112);
            this.cboType.Name = "cboType";
            this.cboType.Size = new System.Drawing.Size(230, 25);
            this.cboType.TabIndex = 5;
            this.cboType.TabOnEnter = true;
            this.cboType.Value = "";
            this.cboType.TextChanged += new System.EventHandler(this.Dirty_TextChanged);
            // 
            // lblCode
            // 
            this.lblCode.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCode.Location = new System.Drawing.Point(22, 46);
            this.lblCode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(99, 18);
            this.lblCode.TabIndex = 0;
            this.lblCode.Text = "Code";
            this.lblCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtEmail
            // 
            this.txtEmail.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEmail.Format = null;
            this.txtEmail.Location = new System.Drawing.Point(511, 148);
            this.txtEmail.Margin = new System.Windows.Forms.Padding(4);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Numeric = false;
            this.txtEmail.ReadOnly = true;
            this.txtEmail.Size = new System.Drawing.Size(230, 25);
            this.txtEmail.TabIndex = 15;
            this.txtEmail.TabOnEnter = true;
            this.txtEmail.TextChanged += new System.EventHandler(this.Dirty_TextChanged);
            this.txtEmail.Enter += new System.EventHandler(this.SwitchToEN_Enter);
            // 
            // groupLabel1
            // 
            this.groupLabel1.Caption = "Contact";
            this.groupLabel1.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupLabel1.Location = new System.Drawing.Point(418, 12);
            this.groupLabel1.Margin = new System.Windows.Forms.Padding(4, 13, 4, 4);
            this.groupLabel1.Name = "groupLabel1";
            this.groupLabel1.Size = new System.Drawing.Size(323, 22);
            this.groupLabel1.TabIndex = 0;
            this.groupLabel1.TabStop = false;
            // 
            // txtFax
            // 
            this.txtFax.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFax.Format = null;
            this.txtFax.Location = new System.Drawing.Point(511, 113);
            this.txtFax.Margin = new System.Windows.Forms.Padding(4);
            this.txtFax.Name = "txtFax";
            this.txtFax.Numeric = false;
            this.txtFax.ReadOnly = true;
            this.txtFax.Size = new System.Drawing.Size(230, 25);
            this.txtFax.TabIndex = 13;
            this.txtFax.TabOnEnter = true;
            this.txtFax.TextChanged += new System.EventHandler(this.Dirty_TextChanged);
            this.txtFax.Enter += new System.EventHandler(this.SwitchToEN_Enter);
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(18, 149);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(103, 18);
            this.label5.TabIndex = 6;
            this.label5.Text = "Address";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPhone
            // 
            this.txtPhone.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPhone.Format = null;
            this.txtPhone.Location = new System.Drawing.Point(511, 78);
            this.txtPhone.Margin = new System.Windows.Forms.Padding(4);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Numeric = false;
            this.txtPhone.ReadOnly = true;
            this.txtPhone.Size = new System.Drawing.Size(230, 25);
            this.txtPhone.TabIndex = 11;
            this.txtPhone.TabOnEnter = true;
            this.txtPhone.TextChanged += new System.EventHandler(this.Dirty_TextChanged);
            this.txtPhone.Enter += new System.EventHandler(this.SwitchToEN_Enter);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(415, 116);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 18);
            this.label2.TabIndex = 12;
            this.label2.Text = "Fax";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Format = null;
            this.txtName.Location = new System.Drawing.Point(511, 42);
            this.txtName.Margin = new System.Windows.Forms.Padding(4);
            this.txtName.Name = "txtName";
            this.txtName.Numeric = false;
            this.txtName.ReadOnly = true;
            this.txtName.Size = new System.Drawing.Size(230, 25);
            this.txtName.TabIndex = 9;
            this.txtName.TabOnEnter = true;
            this.txtName.TextChanged += new System.EventHandler(this.Dirty_TextChanged);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(415, 151);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 18);
            this.label3.TabIndex = 14;
            this.label3.Text = "Email";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(415, 81);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 18);
            this.label1.TabIndex = 10;
            this.label1.Text = "Phone";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAddress
            // 
            this.txtAddress.AcceptsReturn = true;
            this.txtAddress.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAddress.Format = null;
            this.txtAddress.Location = new System.Drawing.Point(129, 146);
            this.txtAddress.Margin = new System.Windows.Forms.Padding(4);
            this.txtAddress.Multiline = true;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Numeric = false;
            this.txtAddress.ReadOnly = true;
            this.txtAddress.Size = new System.Drawing.Size(230, 44);
            this.txtAddress.TabIndex = 7;
            this.txtAddress.TabOnEnter = false;
            this.txtAddress.TextChanged += new System.EventHandler(this.Dirty_TextChanged);
            this.txtAddress.Leave += new System.EventHandler(this.txtCode_Leave);
            // 
            // txtDescription
            // 
            this.txtDescription.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDescription.Format = null;
            this.txtDescription.Location = new System.Drawing.Point(129, 78);
            this.txtDescription.Margin = new System.Windows.Forms.Padding(4);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Numeric = false;
            this.txtDescription.ReadOnly = true;
            this.txtDescription.Size = new System.Drawing.Size(230, 25);
            this.txtDescription.TabIndex = 3;
            this.txtDescription.TabOnEnter = true;
            this.txtDescription.TextChanged += new System.EventHandler(this.Dirty_TextChanged);
            this.txtDescription.Leave += new System.EventHandler(this.txtCode_Leave);
            // 
            // txtCode
            // 
            this.txtCode.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCode.Format = null;
            this.txtCode.Location = new System.Drawing.Point(129, 42);
            this.txtCode.Margin = new System.Windows.Forms.Padding(4);
            this.txtCode.Name = "txtCode";
            this.txtCode.Numeric = false;
            this.txtCode.ReadOnly = true;
            this.txtCode.Size = new System.Drawing.Size(230, 25);
            this.txtCode.TabIndex = 1;
            this.txtCode.TabOnEnter = true;
            this.txtCode.TextChanged += new System.EventHandler(this.Dirty_TextChanged);
            this.txtCode.Enter += new System.EventHandler(this.SwitchToEN_Enter);
            this.txtCode.Leave += new System.EventHandler(this.txtCode_Leave);
            // 
            // mnuShow
            // 
            this.mnuShow.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuShowA,
            this.mnuShowI});
            this.mnuShow.Name = "contextMenuStrip1";
            this.mnuShow.Size = new System.Drawing.Size(148, 48);
            // 
            // mnuShowA
            // 
            this.mnuShowA.Checked = true;
            this.mnuShowA.CheckOnClick = true;
            this.mnuShowA.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuShowA.Name = "mnuShowA";
            this.mnuShowA.Size = new System.Drawing.Size(147, 22);
            this.mnuShowA.Text = "Show Active";
            this.mnuShowA.CheckedChanged += new System.EventHandler(this.mnuShow_CheckedChanged);
            // 
            // mnuShowI
            // 
            this.mnuShowI.CheckOnClick = true;
            this.mnuShowI.Name = "mnuShowI";
            this.mnuShowI.Size = new System.Drawing.Size(147, 22);
            this.mnuShowI.Text = "Show Inactive";
            this.mnuShowI.CheckedChanged += new System.EventHandler(this.mnuShow_CheckedChanged);
            // 
            // frmSupplier
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(993, 460);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Noto Sans Khmer", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1009, 390);
            this.Name = "frmSupplier";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AP Supplier";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSupplierList_FormClosing);
            this.Load += new System.EventHandler(this.frmSupplierList_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvList)).EndInit();
            this.mnuShow.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnNew;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnCopy;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripButton btnDelete;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripButton btnSaveNew;
        private System.Windows.Forms.TextBox txtNote;
        private System.Windows.Forms.ContextMenuStrip mnuShow;
        private System.Windows.Forms.ToolStripButton btnActive;
        private System.Windows.Forms.ToolStripButton btnUnlock;
        private System.Windows.Forms.ToolStripMenuItem mnuShowA;
        private System.Windows.Forms.ToolStripMenuItem mnuShowI;
        private kUI.DataGridView dgvList;
        private System.Windows.Forms.Label lblCode;
        private kUI.GroupLabel glbGeneral;
        private kUI.TextBox txtAddress;
        private kUI.TextBox txtName;
        private System.Windows.Forms.Label lblDescription;
        private kUI.GroupLabel glbNote;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripButton btnMode;
        private System.Windows.Forms.ToolStripButton btnExport;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private kUI.TextBox txtFind;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Label lblSearch;
        private kUI.GroupLabel groupLabel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private kUI.ComboBox cboType;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private kUI.TextBox txtDescription;
        private kUI.TextBox txtCode;
        private kUI.TextBox txtEmail;
        private kUI.TextBox txtFax;
        private kUI.TextBox txtPhone;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.ToolStripButton btnSelect;
    }
}

