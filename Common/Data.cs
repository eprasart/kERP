using kUI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kERP
{
    class BaseTable
    {
        public long Id { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
        public string Insert_By { get; set; }
        public DateTime? Insert_At { get; set; }
        public string Change_By { get; set; }
        public DateTime? Change_At { get; set; }
    }

    public static class DataFacade
    {
        public static void LoadList(ComboBox cbo, string field)
        {
            string sql = SqlFacade.SqlSelect("list", "code, description, is_default", "status = 'A' and field = '" + field + "'", "seq, description");
            DataTable dt = SqlFacade.GetDataTable(sql);
            cbo.DataSource = dt;
            cbo.DisplayMember = "description";
            cbo.ValueMember = "code";
            var dr = dt.Select("is_default = 'Y'");
            if (dr.Length > 0)
                cbo.SelectedValue = dr[0][0];
            else
                cbo.SelectedIndex = -1;
        }       

        public static void LoadAgent(ComboBox cbo)
        {
            string sql = SqlFacade.SqlSelect("agent", "id, name", "status = 'A' and branch_code = '" + App.session.Branch_Code + "'", "name");
            cbo.DataSource = SqlFacade.GetDataTable(sql);
            cbo.ValueMember = "id";
            cbo.DisplayMember = "name";
        }

        public static void LoadProduct(ComboBox cbo)
        {
            string sql = SqlFacade.SqlSelect("product", "code, name", "status = 'A'", "name");
            cbo.DataSource = SqlFacade.GetDataTable(sql);
            cbo.ValueMember = "code";
            cbo.DisplayMember = "name";
        }

        public static void LoadRegional(ComboBox cbo, string type, object parent = null)
        {
            if (parent == null && !type.Contains("P")) return;
            string sWhere = "status = 'A' and type in (" + type + ")";
            if (parent != null) sWhere += " and parent = '" + parent.ToString() + "'";
            cbo.DataSource = SqlFacade.GetDataTable(SqlFacade.SqlSelect("regional", "code, name", sWhere, "name"));
            cbo.ValueMember = "code";
            cbo.DisplayMember = "name";
        }

        public static void LoadBranch(ComboBox cbo, bool includeEmpty = true)
        {
            string sql = SqlFacade.SqlSelect("branch", "code, code || ' ' || name as name", "status = 'A'", "code");
            if (includeEmpty) sql = "select '' as code, '' as name\nunion\n" + sql;
            cbo.DataSource = SqlFacade.GetDataTable(sql);
            cbo.ValueMember = "code";
            cbo.DisplayMember = "name";
        }
    }
}
