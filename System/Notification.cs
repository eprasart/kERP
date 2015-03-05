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
    public partial class frmNotification : Form
    {
        public string Message
        {
            get { return lblMsg.Text; }
            set { lblMsg.Text = value; }
        }

        public frmNotification()
        {
            InitializeComponent();
        }

        public frmNotification(string msg)
            : this()
        {
            Message = msg;
        }

        public void ShowMsg(string msg)
        {
            Message = msg;
            Application.DoEvents();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
