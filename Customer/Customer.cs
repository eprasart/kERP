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
    class Customer
    {
        public long Id { get; set; }
        public string Customer_No { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Gender { get; set; }
        public DateTime Date_of_Birth { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public string Branch_Code { get; set; }
        public string Id_Type1 { get; set; }
        public string Id_Value1 { get; set; }
        public string Id_Type2 { get; set; }
        public string Id_Value2 { get; set; }
        public string Id_Type3 { get; set; }
        public string Id_Value3 { get; set; }
        public string Contact_Type1 { get; set; }
        public string Contact_Value1 { get; set; }
        public string Contact_Type2 { get; set; }
        public string Contact_Value2 { get; set; }
        public string Contact_Type3 { get; set; }
        public string Contact_Value3 { get; set; }
        public string Contact_Type4 { get; set; }
        public string Contact_Value4 { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Commune { get; set; }
        public string Village { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
        public string Insert_By { get; set; }
        public DateTime? Insert_At { get; set; }
        public string Change_By { get; set; }
        public DateTime? Change_At { get; set; }
    }

    static class CustomerFacade
    {
        public static readonly string TableName = "customer";
        public static readonly string TitleLabel = LabelFacade.sys_customer;

        public static DataTable GetDataTable(string filter = "", string status = "")
        {
            var sql = SqlFacade.SqlSelect(TableName, "id, customer_no, last_name || ' ' || first_name as name, gender, date_of_birth", "1 = 1");
            if (status.Length == 0)
                sql += " and status <> '" + Constant.RecordStatus_Deleted + "'";
            else
                sql += " and status = '" + status + "'";
            if (filter.Length > 0)
                sql += " and (" + SqlFacade.SqlILike("no, last_name || ' ' || first_name ") + ")";
            sql += "\norder by customer_no\nlimit " + ConfigFacade.Select_Limit; //ConfigFacade.sy_select_limit;

            var cmd = new NpgsqlCommand(sql);
            if (filter.Length > 0)
                cmd.Parameters.AddWithValue(":filter", "%" + filter + "%");

            return SqlFacade.GetDataTable(cmd);
        }

        public static long Save(Customer m)
        {
            string sql = "";
            if (m.Id == 0)
            {
                m.Insert_By = App.session.Username;
                sql = "customer_no, first_name, last_name, gender, date_of_birth, type, category, branch_code, " +
                    "id_type1, id_value1, id_type2, id_value2, id_type3, id_value3, contact_type1, contact_value1, contact_type2, contact_value2, contact_type3, contact_value3, contact_type4, contact_value4, " +
                    "address, province, district, commune, village, note, insert_by";
                sql = SqlFacade.SqlInsert(TableName, sql, "", true);
                m.Id = SqlFacade.Connection.ExecuteScalar<long>(sql, m);
            }
            else
            {
                m.Change_By = App.session.Username;
                sql = "customer_no, first_name, last_name, gender, date_of_birth, type, category, branch_code, " +
                   "id_type1, id_value1, id_type2, id_value2, id_type3, id_value3, contact_type1, contact_value1, contact_type2, contact_value2, contact_type3, contact_value3, contact_type4, contact_value4, " +
                   "address, province, district, commune, village, note, change_by, change_at, change_no";
                sql = SqlFacade.SqlUpdate(TableName, sql, "change_at = now(), change_no = change_no + 1", "id = :id");
                SqlFacade.Connection.Execute(sql, m);
                ReleaseLock(m.Id);  // Unlock
            }
            return m.Id;
        }

        public static Customer Select(long Id)
        {
            var sql = SqlFacade.SqlSelect(TableName, "*", "id = :id");
            return SqlFacade.Connection.Query<Customer>(sql, new { Id }).FirstOrDefault();
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

        public static bool Exists(string customer_no, long Id = 0)
        {
            var sql = SqlFacade.SqlExists(TableName, "id <> :id and status <> :status and customer_no = :customer_no");
            var bExists = false;
            try
            {
                bExists = SqlFacade.Connection.ExecuteScalar<bool>(sql, new { Id, Status = Constant.RecordStatus_Deleted, customer_no });
            }
            catch (Exception ex)
            {
                MessageFacade.Show(MessageFacade.error_query + "\r\n" + ex.Message,TitleLabel, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrorLogFacade.Log(ex, "Exists");
            }
            return bExists;
        }

        public static void Export()
        {
            var cols = "*";
            cols = ConfigFacade.Get(Constant.Sql_Export + TableName, cols);
            string sql = SqlFacade.SqlSelect(TableName, cols, "status <> '" + Constant.RecordStatus_Deleted + "'", "customer_no");
            SqlFacade.ExportToCSV(sql);
        }

        public static void SaveSrNo(string branch_code, long running_no = 1)
        {
            var sql = SqlFacade.SqlInsert("customer_srno", "branch_code, running_no", "");
            SqlFacade.Connection.Execute(sql, new { branch_code, running_no });
        }

        public static void IncrementSrNo(string branch_code)
        {
            var sql = SqlFacade.SqlUpdate("customer_srno", "running_no", "running_no = running_no + 1", "branch_code = :branch_code");
            SqlFacade.Connection.Execute(sql, new { branch_code });
        }

        public static void DecrementSrNo(string branch_code)    // When cancel; //todo: but when multi user???
        {
            var sql = SqlFacade.SqlUpdate("customer_srno", "running_no", "running_no = running_no - 1", "branch_code = :branch_code");
            SqlFacade.Connection.Execute(sql, new { branch_code });
        }

        public static string GetNextCustomerNo(string branch_code)
        {
            var sql = SqlFacade.SqlSelect("customer_srno", "running_no", "branch_code = :branch_code");
            var lNo = SqlFacade.Connection.ExecuteScalar<long>(sql, new { branch_code });
            if (lNo != 0)
                IncrementSrNo(branch_code);
            else
            {
                SaveSrNo(branch_code, 2);
                lNo = 1;
            }
            var sNo = branch_code + "-" + lNo.ToString(ConfigFacade.Get("sys_customer_no_format"));
            return sNo;
        }
    }
}