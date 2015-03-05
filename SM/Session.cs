using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Npgsql;
using Dapper;
using kERP.SYS;

namespace kERP.SM
{
    public class Session
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Username { get; set; }
        public string Branch_Code { get; set; }
        public DateTime? Login_At { get; set; }
        public DateTime? Logout_At { get; set; }
        public string Version { get; set; }
        public string Machine_Name { get; set; }
        public string Machine_User_Name { get; set; }
        public String Status { get; set; }
    }

    class SessionLog
    {
        public long Id { get; set; }
        public DateTime? Log_At { get; set; }
        //[References(typeof(Session))]
        public long Session_Id { get; set; }
        public string Priority { get; set; }
        public string Module { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
    }

    static class SessionFacade
    {
        const string TableName = "sm_session";

        public static DataTable GetDataTable(string filter = "", string status = "")
        {
            var sql = "select id, username, branch_code, login_at, logout_at, version, machine_name, machine_user_name from sm_session where 1 = 1";
            if (status.Length > 0)
                sql += " and status = '" + status + "'";
            //if (filter.Length > 0)
            //    sql += " and (username ~* :filter or full_name ~* :filter or phone ~* :filter or email ~* :filter or note ~* :filter)";
            sql += "\norder by username";
            var cmd = new NpgsqlCommand(sql, SqlFacade.Connection);
            if (filter.Length > 0)
                cmd.Parameters.AddWithValue(":filter", filter);
            var da = new NpgsqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public static long Save(Session m)
        {
            long seq = 0;   // New inserted sequence
            if (m.Id == 0)
            {
                var sql = SqlFacade.SqlInsert(TableName, "username, branch_code, version, machine_name, machine_user_name", "", true);
                seq = SqlFacade.Connection.ExecuteScalar<long>(sql, m);
            }
            return seq;
        }

        public static void UpdateLogout(Session m)
        {
            //DateTime? ts = SqlFacade.GetCurrentTimeStamp();
            //m.LogoutAt = ts;
            //SqlFacade.Connection.UpdateOnly(m, p => new { p.LogoutAt }, p => p.Id == m.Id);
        }

        public static Session Select(long Id)
        {
            var sql = SqlFacade.SqlSelect(TableName, "*", "id = :id");
            return SqlFacade.Connection.Query<Session>(sql, new { Id }).FirstOrDefault();
        }
    }

    static class SessionLogFacade
    {
        public static DataTable GetDataTable(string where, string filter = "")
        {
            var sql = "select l.id, username, login_at, logout_at, version, machine_name, machine_user_name, log_at, priority, module, type, message\n" +
                "from sm_session s\nleft join sm_session_log l on s.id = l.session_id where 1 = 1";
            //if (status.Length > 0)
            //    sql += " and status = '" + status + "'";
            if (filter.Length > 0)
                sql += " and (message ~* :filter or type ~* :filter or module ~* :filter)";
            sql += "\norder by login_at desc, log_at desc";
            var cmd = new NpgsqlCommand(sql, new NpgsqlConnection(SqlFacade.ConnectionString));
            if (filter.Length > 0)
                cmd.Parameters.AddWithValue(":filter", filter);
            var da = new NpgsqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        private static void Save(SessionLog m)
        {
            try
            {
                if (m.Id == 0)
                {
                    var sql = "insert into sm_session_log (session_id, priority, module, type, message)\n" +
                        "values (:Session_Id, :Priority, :Module, :Type, :Message)";
                    m.Session_Id = App.session.Id;
                    SqlFacade.Connection.Execute(sql, m);
                }
                else
                {
                    //SqlFacade.Connection.UpdateOnly(m, p => new { p.Username }, p => p.Id == m.Id);
                }
            }
            catch (Exception ex)
            {
                ErrorLogFacade.LogToFile(ex);
            }
        }

        //public static SessionLog Select(long Id)
        //{
        //    return SqlFacade.Connection.SingleById<SessionLog>(Id);
        //}

        public static void Log(string priority, string module, string type, string message)
        {
            var log = new SessionLog()
            {
                Priority = priority,
                Module = module,
                Type = type,
                Message = message
            };
            Log(log);
        }

        public static void Log(SessionLog log)
        {
            Save(log);
        }
    }

    public class Lock
    {
        public long Id { get; set; }
        public string Table_Name { get; set; }
        public string Branch_Code { get; set; }
        public long Lock_Id { get; set; }
        public string Ref { get; set; }
        public string Lock_By { get; set; }
        public string Machine_Name { get; set; }
        public string Machine_Username { get; set; }
        public DateTime? Lock_At { get; set; }
        public bool Locked { get; set; }
    }

    static class LockFacade
    {
        private static readonly string TableName = "sm_lock";

        public static DataTable GetDataTable(string filter = "", string status = "")
        {
            var sql = "select id, code, description, name, phone, fax, email, address from " + TableName + "\nwhere 1 = 1";
            if (status.Length == 0)
                sql += " and status <> '" + Constant.RecordStatus_Deleted + "'";
            else
                sql += " and status = '" + status + "'";
            if (filter.Length > 0)
                sql += " and (code ilike :filter or description ilike :filter or phone ilike :filter or fax ilike :filter or email ilike :filter or address ilike :filter or note ilike :filter)";
            sql += "\norder by code\nlimit " + ConfigFacade.Select_Limit;

            var cmd = new NpgsqlCommand(sql);
            if (filter.Length > 0)
                cmd.Parameters.AddWithValue(":filter", "%" + filter + "%");

            return SqlFacade.GetDataTable(cmd);
        }

        public static long Save(Lock m)
        {
            m.Lock_By = App.session.Username;
            m.Branch_Code = App.session.Branch_Code; 
            m.Machine_Name = App.session.Machine_Name;
            m.Machine_Username = App.session.Machine_User_Name;            
            var sql = SqlFacade.SqlInsert(TableName, "table_name, branch_code, lock_id, ref, lock_by, machine_name, machine_username", "", true);
            m.Id = SqlFacade.Connection.ExecuteScalar<long>(sql, m);
            return m.Id;
        }

        public static Lock Select(long Id)
        {
            var sql = SqlFacade.SqlSelect(TableName, "*", "id=:Id");
            return SqlFacade.Connection.Query<Lock>(sql, new { Id }).FirstOrDefault();
        }

        public static Lock Select(string table, long lockId)
        {
            var sql = SqlFacade.SqlSelect(TableName, "*", "table_name = :Table and lock_id = :LockId", "lock_at desc", 1);
            var m = SqlFacade.Connection.Query<Lock>(sql, new { table, lockId }).FirstOrDefault();
            if (m == null)
                m = new Lock();  // Locked = false
            else
                m.Locked = true;
            return m;
        }

        public static void Delete(string table, long lockId)
        {
            var sql = SqlFacade.SqlDelete(TableName, "table_name = :table and lock_id = :LockId");
            SqlFacade.Connection.Execute(sql, new { table, lockId });
        }

        public static void Export()
        {
            string sql = SqlFacade.SqlSelect(TableName, "id \"Id\", code \"Code\", description \"Description\", address \"Address\", name \"Contact Name\", phone \"Phone\", fax \"Fax\", " +
                "email \"Email\", note \"Note\", status \"Status\", insert_by \"Inserted By\", insert_at \"Inserted At\", change_by \"Changed By\", change_at \"Changed At\"",
                "status <> '" + Constant.RecordStatus_Deleted + "'", "code");
            SqlFacade.ExportToCSV(sql);
        }
    }
}