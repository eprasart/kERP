using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kERP
{
    class Validator
    {
        public static frmMsg fMsg = null;
        StringBuilder Msg = new StringBuilder();
        Control cFocus = null;
        public static Form frm = null;
        string codePrefix = "";

        public Validator(Form f, string codePre)
        {
            frm = f;
            codePrefix = codePre + "_";
        }

        public void Add(Control c, string messageCode)
        {
            Msg.AppendLine(LabelFacade.sys_msg_prefix + MessageFacade.Get(codePrefix + messageCode));
            if (cFocus == null) cFocus = c;
        }

        public bool Show(string title="")
        {            
            if (Msg.Length > 0)
            {
                MessageFacade.Show(frm, ref fMsg, Msg.ToString(), title );
                cFocus.Focus();
                return false;
            }
            return true;
        }

        public static void Close(Form f)
        {
            if (frm == f && fMsg != null) fMsg.Close();
        }
    }
}
