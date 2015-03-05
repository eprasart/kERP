using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using kERP.SYS;

namespace kERP
{
    public partial class frmMsg : Form
    {
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        public string Title
        {
            get { return lblTitle.Text; }
            set { lblTitle.Text = value; }
        }

        public string Message
        {
            get { return txtMsg.Text; }
            set { txtMsg.Text = value; }
        }

        public frmMsg()
        {
            InitializeComponent();
        }

        public frmMsg(string msg, string title = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.Information,
            MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
            : this()
        {
            lblTitle.Text = title;
            txtMsg.Text = msg;
            btnOK.Visible = false;
            switch (buttons)
            {
                case MessageBoxButtons.AbortRetryIgnore:
                    btnAbort.Visible = true;
                    btnRetry.Visible = true;
                    btnIgnore.Visible = true;
                    break;
                case MessageBoxButtons.OK:
                    btnOK.Visible = true;
                    break;
                case MessageBoxButtons.OKCancel:
                    btnOK.Visible = true;
                    btnCancel.Visible = true;
                    break;
                case MessageBoxButtons.RetryCancel:
                    btnRetry.Visible = true;
                    btnCancel.Visible = true;
                    break;
                case MessageBoxButtons.YesNo:
                    btnYes.Visible = true;
                    btnNo.Visible = true;
                    break;
                case MessageBoxButtons.YesNoCancel:
                    btnYes.Visible = true;
                    btnNo.Visible = true;
                    btnCancel.Visible = true;
                    break;
            }
            switch (icon)
            {
                case MessageBoxIcon.Error:
                    picIcon.Image = Properties.Resources.Error;
                    break;
                case MessageBoxIcon.Exclamation:
                    picIcon.Image = Properties.Resources.Exclamation;
                    break;
                case MessageBoxIcon.Question:
                    picIcon.Image = Properties.Resources.Question;
                    break;
                case MessageBoxIcon.None:
                    picIcon.Image = null;
                    break;
            }
            //todo: default button
        }

        private void frm_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void frm_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void frm_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void btn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txtMsg_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = txtMsg;
            Size tS = TextRenderer.MeasureText(tb.Text, tb.Font);
            bool Hsb = tb.ClientSize.Height < tS.Height + Convert.ToInt32(tb.Font.Size);
            bool Vsb = tb.ClientSize.Width < tS.Width;

            if (Hsb && Vsb)
                tb.ScrollBars = ScrollBars.Both;
            else if (!Hsb && !Vsb)
                tb.ScrollBars = ScrollBars.None;
            else if (Hsb && !Vsb)
                tb.ScrollBars = ScrollBars.Vertical;
            else if (!Hsb && Vsb)
                tb.ScrollBars = ScrollBars.Horizontal;
        }

        private void frmMsgValidation_Load(object sender, EventArgs e)
        {
            btnAbort.Text = LabelFacade.sys_button_abort ?? btnAbort.Text;
            btnCancel.Text = LabelFacade.sys_button_cancel ?? btnCancel.Text;
            btnIgnore.Text = LabelFacade.sys_button_ignore ?? btnIgnore.Text;
            btnNo.Text = LabelFacade.sys_button_no ?? btnNo.Text;
            btnYes.Text = LabelFacade.sys_button_yes ?? btnYes.Text;
            btnOK.Text = LabelFacade.sys_button_ok ?? btnOK.Text;
            btnRetry.Text = LabelFacade.sys_button_retry ?? btnRetry.Text;

            txtMsg_TextChanged(null, null);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                    if (btnCancel.Visible)
                    {
                        DialogResult = DialogResult.Cancel;
                        Close();
                    }
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
