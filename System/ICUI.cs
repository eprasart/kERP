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
    public partial class ICUI : UserControl
    {
        private string Module = "Main Form";

        public ICUI()
        {
            InitializeComponent();
        }

        private void LoadImages()
        {
            lblReceive.Image = ImageFacade.FromFile("Receive");
            lblTransfer.Image = ImageFacade.FromFile("Transfer");
            lblAdjustment.Image = ImageFacade.FromFile("Adjustment");
            lblItem.Image = ImageFacade.FromFile("Item");
            lblLocation.Image = ImageFacade.FromFile("Location");
            lblItemLocation.Image = ImageFacade.FromFile("ItemLocation");
            lblItemSupplier.Image = ImageFacade.FromFile("ItemSupplier");
            lblCategory.Image = ImageFacade.FromFile("Category");
            lblClassification.Image = ImageFacade.FromFile("Classification");
            lblUnitMeasure.Image = ImageFacade.FromFile("UnitMeasure");
        }

        private void ICUI_Load(object sender, EventArgs e)
        {
            LoadImages();
        }

        private void lblLocation_Click(object sender, EventArgs e)
        {
            var f = App.fLocation;
            if (!Privilege.CanAccess("IC_LOC", "V"))
            {
                MessageFacade.Show(MessageFacade.Get("sys_no_access"), LabelFacade.IC_Location);
                return;
            }
            Cursor = Cursors.WaitCursor;
            if (f == null || f.IsDisposed == true)
            {
                f = new frmLocation();
                f.Show();
            }
            if (f.WindowState == FormWindowState.Minimized)
                f.WindowState = FormWindowState.Normal;
            f.Focus();
            SessionLogFacade.Log(Constant.Priority_Information, Module, Constant.Log_Open, f.Text + " opened.");
            Cursor = Cursors.Default;
        }

        private void lblUnitMeasure_Click(object sender, EventArgs e)
        {
            var f = App.fUnitMeasure;
            if (!Privilege.CanAccess("IC_UOM", "V"))
            {
                MessageFacade.Show(MessageFacade.Get("sys_no_access"), LabelFacade.IC_Unit_Measure);
                return;
            }
            Cursor = Cursors.WaitCursor;
            if (f == null || f.IsDisposed == true)
            {
                f = new frmUnitMeasure();
                f.Show();
            }
            if (f.WindowState == FormWindowState.Minimized)
                f.WindowState = FormWindowState.Normal;
            f.Focus();
            SessionLogFacade.Log(Constant.Priority_Information, Module, Constant.Log_Open, f.Text + " opened.");
            Cursor = Cursors.Default;
        }

        private void lblCategory_Click(object sender, EventArgs e)
        {
            var f = App.fCategory;
            if (!Privilege.CanAccess("IC_CAT", "V"))
            {
                MessageFacade.Show(MessageFacade.Get("sys_no_access"), LabelFacade.IC_Category);
                return;
            }
            Cursor = Cursors.WaitCursor;
            if (f == null || f.IsDisposed == true)
            {
                f = new frmCategory();
                f.Show();
            }
            if (f.WindowState == FormWindowState.Minimized)
                f.WindowState = FormWindowState.Normal;
            f.Focus();
            SessionLogFacade.Log(Constant.Priority_Information, Module, Constant.Log_Open, f.Text + " opened.");
            Cursor = Cursors.Default;
        }

        private void lblItem_Click(object sender, EventArgs e)
        {
            var f = App.fItem;
            if (!Privilege.CanAccess("IC_ITM", "V"))
            {
                MessageFacade.Show(MessageFacade.Get("sys_no_access"), LabelFacade.IC_Item);
                return;
            }
            Cursor = Cursors.WaitCursor;
            if (f == null || f.IsDisposed == true)
            {
                f = new frmItem();
                f.Show();
            }
            if (f.WindowState == FormWindowState.Minimized)
                f.WindowState = FormWindowState.Normal;
            f.Focus();
            SessionLogFacade.Log(Constant.Priority_Information, Module, Constant.Log_Open, f.Text + " opened.");
            Cursor = Cursors.Default;
        }

        private void lblClassification_Click(object sender, EventArgs e)
        {
            var f = App.fClassification;
            if (!Privilege.CanAccess("IC_CLS", "V"))
            {
                MessageFacade.Show(MessageFacade.Get("sys_no_access"), LabelFacade.IC_Classification);
                return;
            }
            Cursor = Cursors.WaitCursor;
            if (f == null || f.IsDisposed == true)
            {
                f = new frmClassification();
                f.Show();
            }
            if (f.WindowState == FormWindowState.Minimized)
                f.WindowState = FormWindowState.Normal;
            f.Focus();
            SessionLogFacade.Log(Constant.Priority_Information, Module, Constant.Log_Open, f.Text + " opened.");
            Cursor = Cursors.Default;
        }

        private void lblItemLocation_Click(object sender, EventArgs e)
        {
            var f = App.fItemLocation;
            if (!Privilege.CanAccess("IC_IL", "V"))
            {
                MessageFacade.Show(MessageFacade.Get("sys_no_access"), LabelFacade.IC_Item_Location);
                return;
            }
            Cursor = Cursors.WaitCursor;
            if (f == null || f.IsDisposed == true)
            {
                f = new frmItemLocation();
                f.Show();
            }
            if (f.WindowState == FormWindowState.Minimized)
                f.WindowState = FormWindowState.Normal;
            f.Focus();
            SessionLogFacade.Log(Constant.Priority_Information, Module, Constant.Log_Open, f.Text + " opened.");
            Cursor = Cursors.Default;
        }

        private void lblItemSupplier_Click(object sender, EventArgs e)
        {
            var f = App.fItemSupplier;
            if (!Privilege.CanAccess("IC_IS", "V"))
            {
                MessageFacade.Show(MessageFacade.Get("sys_no_access"), LabelFacade.IC_Item_Supplier);
                return;
            }
            Cursor = Cursors.WaitCursor;
            if (f == null || f.IsDisposed == true)
            {
                f = new frmItemSupplier();
                f.Show();
            }
            if (f.WindowState == FormWindowState.Minimized)
                f.WindowState = FormWindowState.Normal;
            f.Focus();
            SessionLogFacade.Log(Constant.Priority_Information, Module, Constant.Log_Open, f.Text + " opened.");
            Cursor = Cursors.Default;
        }
    }
}
