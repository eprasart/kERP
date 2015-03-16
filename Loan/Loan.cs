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
    class Product
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Calculation_Method { get; set; }
        public string Principal_Round_Rule { get; set; }
        public string Interest_Round_Rule { get; set; }
        public string Total_Round_Rule { get; set; }
        public string Never_On { get; set; }
        public string Non_Working_Day_Move { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
        public string Insert_By { get; set; }
        public DateTime? Insert_At { get; set; }
        public string Change_By { get; set; }
        public DateTime? Change_At { get; set; }
    }

    static class ProductFacade
    {
        public static readonly string TableName = "product";
        public static readonly string TitleLabel = LabelFacade.sys_product;

        public static DataTable GetDataTable(string filter = "", string status = "")
        {
            var sql = "select p.id, p.code, name, method.description calculation_method, prin.description principal_round_rule, int.description interest_round_rule, total.description total_round_rule" +
                "\nfrom product p" +
                "\njoin list method on field = 'calculation_method' and method.code = calculation_method" +
                "\ninner join list prin on prin.field = 'round_rule' and prin.code = principal_round_rule" +
                "\ninner join list int on int.field = 'round_rule' and int.code = interest_round_rule" +
                "\ninner join list total on total.field = 'round_rule' and total.code = total_round_rule" +
                "\nwhere 1 = 1";
            if (status.Length == 0)
                sql += " and p.status <> '" + Constant.RecordStatus_Deleted + "'";  // todo: sql injection is possible => a better way
            else
                sql += " and p.status = '" + status + "'";
            if (filter.Length > 0)
                sql += " and (" + SqlFacade.SqlILike("p.code, name, p.note") + ")";
            sql += "\norder by name\nlimit " + ConfigFacade.Select_Limit;

            var cmd = new NpgsqlCommand(sql);
            if (filter.Length > 0)
                cmd.Parameters.AddWithValue(":filter", "%" + filter + "%");

            return SqlFacade.GetDataTable(cmd);
        }

        public static long Save(Product m)
        {
            string sql = "";
            if (m.Id == 0)
            {
                m.Insert_By = App.session.Username;
                sql = "code, name, calculation_method, principal_round_rule, interest_round_rule, total_round_rule, never_on, non_working_day_move, note, insert_by";
                sql = SqlFacade.SqlInsert(TableName, sql, "", true);
                m.Id = SqlFacade.Connection.ExecuteScalar<long>(sql, m);
            }
            else
            {
                m.Change_By = App.session.Username;
                sql = "code, name, calculation_method, principal_round_rule, interest_round_rule, total_round_rule, never_on, non_working_day_move, note, change_by, change_at, change_no";
                sql = SqlFacade.SqlUpdate(TableName, sql, "change_at = now(), change_no = change_no + 1", "id = :id");
                SqlFacade.Connection.Execute(sql, m);
                ReleaseLock(m.Id);  // Unlock
            }
            return m.Id;
        }

        public static Product Select(long Id)
        {
            var sql = SqlFacade.SqlSelect(TableName, "*", "id = :id");
            return SqlFacade.Connection.Query<Product>(sql, new { Id }).FirstOrDefault();
        }

        public static Product Select(string code)
        {
            var sql = SqlFacade.SqlSelect(TableName, "*", "code = :code");
            return SqlFacade.Connection.Query<Product>(sql, new { code }).FirstOrDefault();
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

        public static bool Exists(string name, long Id = 0)
        {
            var sql = SqlFacade.SqlExists(TableName, "id <> :id and status <> :status and name = :name");
            var bExists = false;
            try
            {
                bExists = SqlFacade.Connection.ExecuteScalar<bool>(sql, new { Id, Status = Constant.RecordStatus_Deleted, name });
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
            string sql = SqlFacade.SqlSelect(TableName, cols, "status <> '" + Constant.RecordStatus_Deleted + "'", "name");
            SqlFacade.ExportToCSV(sql, TableName);
        }
    }

    class Loan
    {
        public long Id { get; set; }
        public string Account_No { get; set; }
        public string Customer_No { get; set; }
        public string Branch_Code { get; set; }
        public string Frequency_Unit { get; set; }
        public int Frequency { get; set; }
        public int Installment_No { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public double Interest_Rate { get; set; }
        public string Product { get; set; }
        public DateTime Disburse_Date { get; set; }
        public DateTime First_Installment_Date { get; set; }
        public DateTime Maturity_Date { get; set; }
        public string Never_On { get; set; }
        public string Non_Working_Day_Move { get; set; }
        public string Purpose { get; set; }
        public string Payment_Site { get; set; }
        public int Credit_Agent_Id { get; set; }
        public string Account_Status { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
        public string Insert_By { get; set; }
        public DateTime? Insert_At { get; set; }
        public string Change_By { get; set; }
        public DateTime? Change_At { get; set; }
    }

    static class LoanFacade
    {
        public static readonly string TableName = "loan";
        public static readonly string TitleLabel = LabelFacade.sys_loan;

        public static DataTable GetDataTable(string filter = "", string status = "")
        {
            var sql = "select l.id, account_no, last_name || ' ' || first_name full_name, frequency_unit, frequency, installment_no, amount, currency, interest_rate, disburse_date" +
                "\nfrom loan l\ninner join customer c on c.customer_no = l.customer_no";
            if (status.Length == 0)
                sql += " and l.status <> '" + Constant.RecordStatus_Deleted + "'";
            else
                sql += " and l.status = '" + status + "'";
            if (filter.Length > 0)
                sql += " and (" + SqlFacade.SqlILike("account_no, l.customer_no, l.branch_code") + ")";
            sql += "\norder by account_no\nlimit " + ConfigFacade.Select_Limit;

            var cmd = new NpgsqlCommand(sql);
            if (filter.Length > 0)
                cmd.Parameters.AddWithValue(":filter", "%" + filter + "%");

            return SqlFacade.GetDataTable(cmd);
        }

        public static long Save(Loan m)
        {
            string sql = "";
            if (m.Id == 0)
            {
                m.Insert_By = App.session.Username;
                m.Branch_Code = App.session.Branch_Code;
                sql = "account_no, customer_no, branch_code, frequency_unit, frequency, installment_no, amount, currency, interest_rate, product, " +
                  "disburse_date, first_installment_date, maturity_date, never_on, non_working_day_move, purpose, payment_site, credit_agent_id, " +
                  "note, insert_by";
                sql = SqlFacade.SqlInsert(TableName, sql, "", true);

                m.Id = SqlFacade.Connection.ExecuteScalar<long>(sql, m);
            }
            else
            {
                m.Change_By = App.session.Username;
                sql = "account_no, customer_no, branch_code, frequency_unit, frequency, installment_no, amount, currency, interest_rate, product, disburse_date, first_installment_date, " +
                    "maturity_date, never_on, non_working_day_move, purpose, payment_site, credit_agent_id, note, change_by, change_at, change_no";
                sql = SqlFacade.SqlUpdate(TableName, sql, "change_at = now(), change_no = change_no + 1", "id = :id");
                SqlFacade.Connection.Execute(sql, m);
                ReleaseLock(m.Id);  // Unlock
            }
            return m.Id;
        }

        public static Loan Select(long Id)
        {
            var sql = SqlFacade.SqlSelect(TableName, "*", "id = :id");
            return SqlFacade.Connection.Query<Loan>(sql, new { Id }).FirstOrDefault();
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
            string sql = SqlFacade.SqlSelect(TableName, cols, "status <> '" + Constant.RecordStatus_Deleted + "'", "account_no");
            SqlFacade.ExportToCSV(sql, TableName);
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

        public static string GetNextAccountNo(string customer_no)
        {
            var sql = SqlFacade.SqlSelect(TableName, ":customer_no || '-' || count(*) + 1", "customer_no = :customer_no");
            var sNo = SqlFacade.Connection.ExecuteScalar<string>(sql, new { customer_no });
            return sNo;
        }
    }

    class Schedule
    {
        public long Id { get; set; }
        public string account_no { get; set; }
        public DateTime date { get; set; }
        public int no { get; set; }
        public double principal { get; set; }
        public double interest { get; set; }
        public double total { get; set; }
        public double outstanding { get; set; }
        public double pay_off { get; set; }
        public string note { get; set; }
        public string status { get; set; }
        public string Insert_By { get; set; }
        public DateTime? Insert_At { get; set; }
        public string Change_By { get; set; }
        public DateTime? Change_At { get; set; }
    }

    static class ScheduleFacade
    {
        public static readonly string TableName = "schedule";

        public static DataTable GetDataTable(string account_no)
        {
            var sql = SqlFacade.SqlSelect(TableName, "id, date, no, principal, interest, total, outstanding", "account_no = :account_no", "no");
            var cmd = new NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue(":account_no", account_no);
            return SqlFacade.GetDataTable(cmd);
        }

        public static long Save(Schedule m)
        {
            string sql = "";
            if (m.Id == 0)
            {
                m.Insert_By = App.session.Username;
                sql = "account_no, date, no, principal, interest, total, outstanding, insert_by";
                sql = SqlFacade.SqlInsert(TableName, sql, "", true);
                m.Id = SqlFacade.Connection.ExecuteScalar<long>(sql, m);
            }
            else
            {
                m.Change_By = App.session.Username;
                sql = "date, no, principal, interest, total, outstanding, change_by, change_at, change_no";
                sql = SqlFacade.SqlUpdate(TableName, sql, "change_at = now(), change_no = change_no + 1", "id = :id");
                SqlFacade.Connection.Execute(sql, m);
                //ReleaseLock(m.Id);  // Unlock
            }
            return m.Id;
        }

        public static void Delete(string account_no)
        {
            var sql = SqlFacade.SqlDelete(TableName, "account_no = :account_no");
            SqlFacade.Connection.Execute(sql, new { account_no });
        }


        public static double EMI(double amount, double r, int n)
        {   // PMT
            var pow = (double)Math.Pow((double)(1 + r), (double)n);
            return amount * r * pow / (pow - 1);
        }
        //public static Loan Select(long Id)
        //{
        //    var sql = SqlFacade.SqlSelect(TableName, "*", "id = :id");
        //    return SqlFacade.Connection.Query<Loan>(sql, new { Id }).FirstOrDefault();
        //}

        //public static void SetStatus(long Id, string status)
        //{
        //    var sql = SqlFacade.SqlUpdate(TableName, "status, change_by, change_at", "change_at = now()", "id = :id");
        //    SqlFacade.Connection.Execute(sql, new { status, Change_By = App.session.Username, Id });
        //}

        //public static Lock GetLock(long Id)
        //{
        //    return LockFacade.Select(TableName, Id);
        //}

        //public static void Lock(long Id, string code)
        //{
        //    var m = new Lock { Table_Name = TableName, Lock_Id = Id, Ref = code };
        //    LockFacade.Save(m);
        //}

        //public static void ReleaseLock(long Id)
        //{
        //    LockFacade.Delete(TableName, Id);
        //}

        //public static bool Exists(string account_no, long Id = 0)
        //{
        //    var sql = SqlFacade.SqlExists(TableName, "id <> :id and status <> :status and account_no = :account_no");
        //    var bExists = false;
        //    try
        //    {
        //        bExists = SqlFacade.Connection.ExecuteScalar<bool>(sql, new { Id, Status = Type.RecordStatus_Deleted, No = account_no });
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageFacade.Show(MessageFacade.error_query + "\r\n" + ex.Message, LabelFacade.sy_customer, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        ErrorLogFacade.Log(ex, "Exists");
        //    }
        //    return bExists;
        //}

        //public static void Export()
        //{
        //    string sql = SqlFacade.SqlSelect(TableName, ConfigFacade.sy_sql_export_customer, "status <> '" + Type.RecordStatus_Deleted + "'", "account_no");
        //    SqlFacade.ExportToCSV(sql);
        //}

        //public static void SaveSrNo(string branch_code, long running_no = 1)
        //{
        //    var sql = SqlFacade.SqlInsert("customer_srno", "branch_code, running_no", "");
        //    SqlFacade.Connection.Execute(sql, new { branch_code, running_no });
        //}

        //public static void IncrementSrNo(string branch_code)
        //{
        //    var sql = SqlFacade.SqlUpdate("customer_srno", "running_no", "running_no = running_no + 1", "branch_code = :branch_code");
        //    SqlFacade.Connection.Execute(sql, new { branch_code });
        //}

        //public static void DecrementSrNo(string branch_code)    // When cancel; //todo: but when multi user???
        //{
        //    var sql = SqlFacade.SqlUpdate("customer_srno", "running_no", "running_no = running_no - 1", "branch_code = :branch_code");
        //    SqlFacade.Connection.Execute(sql, new { branch_code });
        //}

        //public static string GetNextAccountNo(string customer_no)
        //{
        //    var sql = SqlFacade.SqlSelect(TableName, ":customer_no || '-' || count(*) + 1", "customer_no = :customer_no");
        //    var sNo = SqlFacade.Connection.ExecuteScalar<string>(sql, new { customer_no });
        //    return sNo;
        //}
    }
}