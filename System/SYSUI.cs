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
    public partial class SYSUI : UserControl
    {
        private string Module = "Main Form";

        public SYSUI()
        {
            InitializeComponent();
        }

        private void LoadImages()
        {           
            lblCompany.Image = ImageFacade.FromFile("Company");
            lblBranch.Image = ImageFacade.FromFile("Branch");
            //btnLabelMessage.Image = ImageFacade.FromFile(""); 
        }

        private void ICUI_Load(object sender, EventArgs e)
        {
            LoadImages();
        }
    }
}
