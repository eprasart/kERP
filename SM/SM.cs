using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Npgsql;
using Dapper;
using kERP.SYS;
using System.Windows.Forms;

namespace kERP
{
    class User : BaseTable
    {
        public string Username { get; set; }
        public string Full_Name { get; set; }
        public string Pwd { get; set; }
        public DateTime? Pwd_Change_On { get; set; }
        public bool Pwd_Change_Force { get; set; }
        public int Time_Level { get; set; }
        public DateTime? Start_On { get; set; }
        public DateTime? End_On { get; set; }
        public int Success { get; set; }
        public int Fail { get; set; }
        public string User_Status { get; set; }
        public string Right { get; set; }
        public string Security_No { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    static class UserFacade
    {
        public static readonly string TableName = "sm_user";
        public static readonly string TitleLabel = LabelFacade.SM_User;

        public static DataTable GetDataTable(string filter = "", string status = "")
        {
            var sql = SqlFacade.SqlSelect(TableName + " u left join list s on u.user_status = s.code and s.field = 'sys_user_status'", "u.id, username, full_name, time_level, start_on, end_on, s.description user_status, phone, email", "1 = 1");
            sql += " and u.status " + (status.Length == 0 ? "<>" : "=") + " :status";
            if (status.Length == 0) status = Constant.RecordStatus_Deleted;
            if (filter.Length > 0)
                sql += " and (" + SqlFacade.SqlILike("username, full_name, phone, email") + ")";
            sql += "\norder by username\nlimit " + ConfigFacade.Select_Limit;
            var cmd = new NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue(":status", status);
            if (filter.Length > 0) cmd.Parameters.AddWithValue(":filter", "%" + filter + "%");
            return SqlFacade.GetDataTable(cmd);
        }

        public static long Save(User m)
        {
            string sql = "username, full_name, pwd_change_force, time_level, start_on, end_on, phone, email, user_status, note, ";
            if (m.Id == 0)
            {
                m.Insert_By = App.session.Username;
                sql += "pwd, insert_by";
                var values = ":" + sql.Replace(", ", ", :");
                values = values.Replace(":pwd,", "crypt(:pwd, gen_salt('bf')),");
                sql = SqlFacade.SqlInsert(TableName, sql, values, true);
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

        public static void ResetPwd(User m)
        {
            var sql = "update " + TableName + "\nset pwd = crypt(:pwd, gen_salt('bf')), user_status = :user_status, " +
                "pwd_change_force = :pwd_change_force, change_by = :change_by, change_at = now(), change_no = change_no + 1\nwhere id = :id";
            m.Change_By = App.session.Username;
            SqlFacade.Connection.Execute(sql, m);
        }

        public static void ClearLocked(long Id)
        {
            var sql = SqlFacade.SqlUpdate(TableName, "user_status", "user_status = 'E'", "id = :id");
            SqlFacade.Connection.Execute(sql, new { Id });
        }

        public static User Select(long Id)
        {
            var sql = SqlFacade.SqlSelect(TableName, "*", "id = :id");
            return SqlFacade.Connection.Query<User>(sql, new { Id }).FirstOrDefault();
        }

        public static User Select(string username)
        {
            var sql = SqlFacade.SqlSelect(TableName, "*", "username = :username");
            return SqlFacade.Connection.Query<User>(sql, new { username }).FirstOrDefault();
        }

        public static User SelectLessCols(string code)
        {
            var sql = SqlFacade.SqlSelect(TableName, "username, full_name", SqlFacade.SqlILike("username, full_name", ":p"));
            return SqlFacade.Connection.Query<User>(sql, new { p = code }).FirstOrDefault();
        }

        public static User SelectLessCols(long Id)
        {
            var sql = SqlFacade.SqlSelect(TableName, "username, full_name", "Id = :Id");
            return SqlFacade.Connection.Query<User>(sql, new { Id }).FirstOrDefault();
        }

        //public static string GetDescription(string code)
        //{
        //    var sql = SqlFacade.SqlSelect(TableName, "description", "code = :code and status = 'A'");
        //    return SqlFacade.Connection.ExecuteScalar<string>(sql, new { code });
        //}

        public static int GetCount(string value)
        {
            var sql = SqlFacade.SqlSelect(TableName, "count (*)", "status = 'A' and (" + SqlFacade.SqlILike("username, full_name", ":p") + ")");
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

        public static bool Exists(string username, long Id = 0)
        {
            var sql = SqlFacade.SqlExists(TableName, "id <> :id and status <> :status and username = :username");
            var bExists = false;
            try
            {
                bExists = SqlFacade.Connection.ExecuteScalar<bool>(sql, new { Id, Status = Constant.RecordStatus_Deleted, username });
            }
            catch (Exception ex)
            {
                MessageFacade.Show(MessageFacade.error_query + "\r\n" + ex.Message, TitleLabel, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrorLogFacade.Log(ex, "Exists");
            }
            return bExists;
        }

        public static bool ExistsBarcode(string barcode, long Id = 0)
        {
            var sql = SqlFacade.SqlExists(TableName, "id <> :id and status <> :status and barcode = :barcode");
            var bExists = false;
            try
            {
                bExists = SqlFacade.Connection.ExecuteScalar<bool>(sql, new { Id, Status = Constant.RecordStatus_Deleted, barcode });
            }
            catch (Exception ex)
            {
                MessageFacade.Show(MessageFacade.error_query + "\r\n" + ex.Message, TitleLabel, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrorLogFacade.Log(ex, "Barcode Exists");
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

        public static void UpdatePwd(User m)
        {
            string sqlPwd = "select crypt(:pwd, gen_salt('bf'))";
            m.Pwd = SqlFacade.Connection.ExecuteScalar<string>(sqlPwd, new { pwd = m.Pwd });
            var sql = SqlFacade.SqlUpdate(TableName, "Pwd = :Pwd", "Id = :Id");
            SqlFacade.Connection.Execute(sql, m);
        }

        public static bool IsPwdCorrect(long id, string pwd)
        {
            string sql = "SELECT (pwd = crypt(:pwd, pwd)) AS pswmatch FROM sm_user where id = :id";
            return SqlFacade.Connection.ExecuteScalar<bool>(sql, new { pwd = pwd, id = id });
        }
    }

    //public class Function
    //{
    //    public long Id { get; set; }
    //    public string Name { get; set; }
    //    public string Code { get; set; }
    //    public string Type { get; set; }
    //    public string Right { get; set; }
    //    public string Note { get; set; }
    //    public String Status { get; set; }
    //    public string LockBy { get; set; }
    //    public DateTime? LockAt { get; set; }
    //    public string InsertBy { get; set; }        
    //    public DateTime? InsertAt { get; set; }
    //    public string ChangeBy { get; set; }
    //    public DateTime? ChangeAt { get; set; }
    //}

    //public class Role
    //{
    //    [AutoIncrement]
    //    public long Id { get; set; }
    //    [Required]
    //    public string Name { get; set; }
    //    public string Note { get; set; }
    //    public String Status { get; set; }
    //    public string LockBy { get; set; }
    //    public DateTime? LockAt { get; set; }
    //    public string InsertBy { get; set; }
    //    [Default(typeof(DateTime), "now()")]
    //    public DateTime? InsertAt { get; set; }
    //    public string ChangeBy { get; set; }
    //    public DateTime? ChangeAt { get; set; }
    //}

    //[Alias("SmRoleFunction")]
    //public class RoleFunction
    //{
    //    [AutoIncrement]
    //    public long Id { get; set; }
    //    [Required]
    //    public long RoleId { get; set; }
    //    public long FunctionId { get; set; }
    //    public string Right { get; set; }
    //    public String Status { get; set; }
    //    public string LockBy { get; set; }
    //    public DateTime? LockAt { get; set; }
    //    public string InsertBy { get; set; }
    //    [Default(typeof(DateTime), "now()")]
    //    public DateTime? InsertAt { get; set; }
    //    public string ChangeBy { get; set; }
    //    public DateTime? ChangeAt { get; set; }
    //}

    //[Alias("SmUserFunction")]
    //class UserFunction
    //{
    //    [AutoIncrement]
    //    public long Id { get; set; }
    //    [Required]
    //    public long UserId { get; set; }
    //    public long FunctionId { get; set; }
    //    public String Status { get; set; }
    //    public string LockBy { get; set; }
    //    public DateTime? LockAt { get; set; }
    //    public string InsertBy { get; set; }
    //    [Default(typeof(DateTime), "now()")]
    //    public DateTime? InsertAt { get; set; }
    //    public string ChangeBy { get; set; }
    //    public DateTime? ChangeAt { get; set; }
    //}

    //[Alias("SmUserRole")]
    //public class UserRole
    //{
    //    [AutoIncrement]
    //    public long Id { get; set; }
    //    public long UserId { get; set; }
    //    public long RoleId { get; set; }
    //    public String Status { get; set; }
    //    public string LockBy { get; set; }
    //    public DateTime? LockAt { get; set; }
    //    public string InsertBy { get; set; }
    //    [Default(typeof(DateTime), "now()")]
    //    public DateTime? InsertAt { get; set; }
    //    public string ChangeBy { get; set; }
    //    public DateTime? ChangeAt { get; set; }
    //}

    public static class Privilege
    {
        public static bool CanAccess(string function, string command)
        {
            // Check in role (user_role > role_function)


            // Check in user_function

            return true;
        }
    }

    class Role : BaseTable
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

    static class RoleFacade
    {
        public static readonly string TableName = "sm_role";
        public static readonly string TitleLabel = LabelFacade.SM_Role;

        public static DataTable GetDataTable(string filter = "", string status = "")
        {
            var sql = SqlFacade.SqlSelect(TableName, "id, code, description", "1 = 1");
            sql += " and status " + (status.Length == 0 ? "<>" : "=") + " :status";
            if (status.Length == 0) status = Constant.RecordStatus_Deleted;
            if (filter.Length > 0)
                sql += " and (" + SqlFacade.SqlILike("code, description, note") + ")";
            sql += "\norder by code\nlimit " + ConfigFacade.Select_Limit;
            var cmd = new NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue(":status", status);
            if (filter.Length > 0)
                cmd.Parameters.AddWithValue(":filter", "%" + filter + "%");
            return SqlFacade.GetDataTable(cmd);
        }

        public static long Save(Role m)
        {
            string sql = "code, description, note";
            if (m.Id == 0)
            {
                m.Insert_By = App.session.Username;
                sql += ", insert_by";
                sql = SqlFacade.SqlInsert(TableName, sql, "", true);
                m.Id = SqlFacade.Connection.ExecuteScalar<long>(sql, m);
            }
            else
            {
                m.Change_By = App.session.Username;
                sql += ", change_by, change_at, change_no";
                sql = SqlFacade.SqlUpdate(TableName, sql, "change_at = now(), change_no = change_no + 1", "id = :id");
                SqlFacade.Connection.Execute(sql, m);
                ReleaseLock(m.Id);  // Unlock
            }
            return m.Id;
        }

        public static Role Select(long Id)
        {
            var sql = SqlFacade.SqlSelect(TableName, "*", "id = :id");
            return SqlFacade.Connection.Query<Role>(sql, new { Id }).FirstOrDefault();
        }

        public static Role SelectLessCols(long Id)
        {
            var sql = SqlFacade.SqlSelect(TableName, "code, description", "Id = :Id");
            return SqlFacade.Connection.Query<Role>(sql, new { Id }).FirstOrDefault();
        }

        public static Role SelectLessCols(string value)
        {
            var sql = SqlFacade.SqlSelect(TableName, "code, description", SqlFacade.SqlILike("code, description", ":p"));
            return SqlFacade.Connection.Query<Role>(sql, new { p = value }).FirstOrDefault();
        }

        public static int GetCount(string value)
        {
            var sql = SqlFacade.SqlSelect(TableName, "count (*)", "status = 'A' and (" + SqlFacade.SqlILike("code, description", ":p") + ")");
            return SqlFacade.Connection.ExecuteScalar<int>(sql, new { p = value });
        }


        public static void LoadList(ComboBox cbo)
        {
            string sql = SqlFacade.SqlSelect(TableName, "code, description", "status = 'A'", "2");
            cbo.DataSource = SqlFacade.GetDataTable(sql);
            cbo.ValueMember = "code";
            cbo.DisplayMember = "description";
        }

        public static string GetDescription(string code)
        {
            var sql = SqlFacade.SqlSelect(TableName, "description", "code = :code and status = 'A'");
            return SqlFacade.Connection.ExecuteScalar<string>(sql, new { code });
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