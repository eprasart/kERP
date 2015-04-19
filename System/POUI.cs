using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kERP
{
    public partial class POUI : UserControl
    {
        private string Module = "Main Form";

        public POUI()
        {
            InitializeComponent();
        }

        private void LoadImages()
        {            
            lblPO.Image = ImageFacade.FromFile("PO");
        }

        private void ICUI_Load(object sender, EventArgs e)
        {
            LoadImages();
        }       
    }
}
