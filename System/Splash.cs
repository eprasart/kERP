using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kERP
{
    public partial class frmSplash : Form
    {
        public frmSplash()
        {
            InitializeComponent();
        }

        private void frmSplash_Load(object sender, EventArgs e)
        {

        }

        public void SetAppName(string name)
        {
            lblAppName.Text += name;
        }

        public void ShowMsg(string msg)
        {
            lblMsg.Text = msg;
            Application.DoEvents();
        }

        public void ShowError(string msg)
        {
            lblError.Text = msg;
            Application.DoEvents();
            btnQuit.Visible = true;
            btnQuit.Focus();
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void lblError_Click(object sender, EventArgs e)
        {
            if (lblError.Text.Length > 0) Clipboard.SetText(lblError.Text);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Close();
        }

        public void StartTimer()
        {
            timer1.Enabled = true;
        }
    }
}
