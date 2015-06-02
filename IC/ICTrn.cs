using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Npgsql;
using Dapper;
using kERP.SYS;
using kERP.SM;
using System.Windows.Forms;

namespace kERP
{
    class ICMaster : BaseTable
    {
        public string Branch_Code { get; set; }
        public long Doc_No { get; set; }
        public string Reference { get; set; }
        public string Trn_Type { get; set; }
        public DateTime Trn_On { get; set; }
        public string App_Code { get; set; }
        public string Org_Type { get; set; }
        public string Originate_No { get; set; }
    }

    class ICTrn : BaseTable
    {
        public string Branch_Code { get; set; }
        public string Doc_No { get; set; }
        public int Line_No { get; set; }
        public string Part_No { get; set; }
        public string Sr_No { get; set; }
        public string Location_Code { get; set; }
        public string Item_Code { get; set; }
        public string Item_Description { get; set; }
        public string Lot_No { get; set; }
        public string Tier { get; set; }
        public double Begin_Qty { get; set; }
        public double Qty { get; set; }
        public double End_Qty { get; set; }
        public string Unit_Mease_Code { get; set; }
        public double Factor { get; set; }
        public string Currency { get; set; }
        public double Exchange_Rate { get; set; }
        public double Cost { get; set; }
        public double Price { get; set; }
        public double Margin { get; set; }
        public double Origin_Price { get; set; }
        public double Discount { get; set; }
        public string Tax_Type { get; set; }
        public double Tax_Rate { get; set; }
        public string Store { get; set; }
        public string Bin { get; set; }
    }

    static class ReceiptFacade
    {
        public static readonly string TableName = "ic_location";
        public static readonly string TitleLabel = LabelFacade.IC_Location;

        public static DataTable GetDataTable(string filter = "", string status = "")
        {
            var sql = SqlFacade.SqlSelect(TableName, "id, branch_code, code, description, type, name, phone, fax, email, address", "1 = 1");
            sql += " and status " + (status.Length == 0 ? "<>" : "=") + " :status";
            if (status.Length == 0) status = Constant.RecordStatus_Deleted;
            if (filter.Length > 0)
                sql += " and (" + SqlFacade.SqlILike("code, description, phone, fax, email, address, note") + ")";
            sql += "\norder by code\nlimit " + ConfigFacade.Select_Limit;

            var cmd = new NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue(":status", status);
            if (filter.Length > 0)
                cmd.Parameters.AddWithValue(":filter", "%" + filter + "%");

            return SqlFacade.GetDataTable(cmd);
        }

        public static long Save(Location m)
        {
            string sql = "code, description, type, address, name, phone, fax, email, note, ";
            if (m.Id == 0)
            {
                m.Insert_By = App.session.Username;
                sql += "insert_by";
                sql = SqlFacade.SqlInsert(TableName, sql, "", true);
                m.Id = SqlFacade.Connection.ExecuteScalar<long>(sql, m);
            }
            else
            {
                m.Change_By = App.session.Username;
                sql += "change_by, change_at, change_no";
                sql = SqlFacade.SqlUpdate(TableName, sql, "change_at = now(), change_no = change_no + 1", "id = :id");
                SqlFacade.Connection.Execute(sql, m);
                ReleaseLock(m.Id);  // Unlock
            }
            return m.Id;
        }

        public static Location Select(long Id)
        {
            var sql = SqlFacade.SqlSelect(TableName, "*", "id = :id");
            return SqlFacade.Connection.Query<Location>(sql, new { Id }).FirstOrDefault();
        }

        public static Location SelectLessCols(long Id)
        {
            var sql = SqlFacade.SqlSelect(TableName, "code, description", "Id = :Id");
            return SqlFacade.Connection.Query<Location>(sql, new { Id }).FirstOrDefault();
        }

        public static Location SelectLessCols(string value)
        {
            var sql = SqlFacade.SqlSelect(TableName, "code, description", SqlFacade.SqlILike("code, description", ":p"));
            return SqlFacade.Connection.Query<Location>(sql, new { p = value }).FirstOrDefault();
        }

        public static string GetDescription(string code)
        {
            var sql = SqlFacade.SqlSelect(TableName, "description", "code = :code and status = 'A'");
            return SqlFacade.Connection.ExecuteScalar<string>(sql, new { code });
        }

        public static int GetCount(string value)
        {
            var sql = SqlFacade.SqlSelect(TableName, "count (*)", "status = 'A' and (" + SqlFacade.SqlILike("code, description", ":p") + ")");
            return SqlFacade.Connection.ExecuteScalar<int>(sql, new { p = value });
        }

        public static void SetStatus(long Id, string status)
        {
            var sql = SqlFacade.SqlUpdate(TableName, "status, change_by, change_at", "change_at = now()", "id = :id");
            SqlFacade.Connection.Execute(sql, new { status, Change_By = App.session.Username, Id });
        }

        public static Lock GetLock(long Id)
        {
            return LockFacade.Select(TableName, Id);
        }

        public static void Lock(long Id, string code)
        {
            var m = new Lock { Table_Name = TableName, Lock_Id = Id, Ref = code };
            LockFacade.Save(m);
        }

        public static void ReleaseLock(long Id)
        {
            LockFacade.Delete(TableName, Id);
        }

        public static bool Exists(string code, long Id = 0)
        {
            var sql = SqlFacade.SqlExists(TableName, "id <> :id and status <> :status and code = :code");
            var bExists = false;
            try
            {
                bExists = SqlFacade.Connection.ExecuteScalar<bool>(sql, new { Id, Status = Constant.RecordStatus_Deleted, code });
            }
            catch (Exception ex)
            {
                MessageFacade.Show(MessageFacade.error_query + "\r\n" + ex.Message, TitleLabel, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrorLogFacade.Log(ex, "Exists");
            }
            return bExists;
        }

        public static void Export()
        {
            var cols = "*";
            cols = ConfigFacade.Get(Constant.Sql_Export + TableName, cols);
            string sql = SqlFacade.SqlSelect(TableName, cols, "status <> '" + Constant.RecordStatus_Deleted + "'", "code");
            SqlFacade.ExportToCSV(sql, TableName);
        }
    }
}