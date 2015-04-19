using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//todo: set icon, save form state...
namespace kERP
{
    public partial class frmImageViewer : Form
    {
        public frmImageViewer()
        {
            InitializeComponent();
        }

        public frmImageViewer(Image img)
            : this()
        {
            pictureBox1.Image = img;
            pictureBox1.Size = img.Size;
        }
    }
}
