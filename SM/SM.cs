using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Npgsql;
using Dapper;
using kERP.SYS;
using System.Windows.Forms;

namespace kERP.SM
{
    public class User
    {
        public long Id { get; set; }
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
        public bool Locked { get; set; }
        public string Right { get; set; }
        public string Security_No { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
        public String Status { get; set; }
        public long Session_Id { get; set; }
        public string Lock_By { get; set; }
        public DateTime? Lock_At { get; set; }
        public string Insert_By { get; set; }
        public DateTime? Insert_At { get; set; }
        public string Change_By { get; set; }
        public DateTime? Change_At { get; set; }
    }

    public static class UserFacade
    {
        const string TableName = "sm_user";

        //public static DataTable GetDataTable(string filter = "", string status = "")
        //{
        //    var sql = "select id, username, full_name, phone, email from sm_user where 1 = 1";
        //    if (status.Length > 0)
        //        sql += " and status = '" + status + "'";
        //    if (filter.Length > 0)
        //        sql += " and (username ~* :filter or full_name ~* :filter or phone ~* :filter or email ~* :filter or note ~* :filter)";
        //    sql += "\norder by username";
        //    var cmd = new NpgsqlCommand(sql, new NpgsqlConnection(SqlFacade.ConnectionString));
        //    if (filter.Length > 0)
        //        cmd.Parameters.AddWithValue(":filter", filter);
        //    var da = new NpgsqlDataAdapter(cmd);
        //    var dt = new DataTable();
        //    try
        //    {
        //        da.Fill(dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error while loading data.\n" + ex.Message, "Location", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        ErrorLogFacade.Log(ex);
        //    }
        //    return dt;
        //}

        //public static long Save(User m)
        //{
        //    var sql = "";
        //    if (m.Id == 0)
        //    {
        //        m.Status = Type.RecordStatus_Active;
        //        m.Insert_By = App.session.Username;
        //        m.Insert_At = ts;
        //        string sqlPwd = "select crypt('" + m.Pwd + "', gen_salt('bf'))";    // Blowfish algorithm
        //        m.Pwd = SqlFacade.ExcuteString(sqlPwd);

        //        sql=SqlFacade.SqlInsert(TableName,)
        //            m.Id = SqlFacade.Connection.Insert(m, true); // New inserted sequence
        //    }
        //    else
        //    {
        //        m.Change_By = App.session.Username;
        //        m.Change_At = ts;

        //        SqlFacade.Connection.UpdateOnly(m, p => new { p.Username, p.FullName, p.StartOn, p.EndOn, p.Phone, p.Email, p.Note, p.ChangeBy, p.ChangeAt },
        //            p => p.Id == m.Id);
        //        // If record is locked then unlock
        //        if (IsLocked(m.Id)) ReleaseLock(m.Id);
        //    }
        //    return m.Id;
        //}

        public static User Select(long Id)
        {
            return SqlFacade.Connection.Query<User>("select * from " + TableName + " where id=@Id", new { Id = Id }).FirstOrDefault();
        }

        public static User Select(string usr)
        {
            return SqlFacade.Connection.Query<User>("select * from " + TableName + " where username=@usr", new { usr = usr }).FirstOrDefault();
        }

        //public static void SetStatus(long Id, string s)
        //{
        //    DateTime? ts = SqlFacade.GetCurrentTimeStamp();
        //    SqlFacade.Connection.UpdateOnly(new User { Status = s, ChangeBy = App.session.Username, ChangeAt = ts }, p => new { p.Status, p.ChangeBy, p.ChangeAt }, p => p.Id == Id);
        //}

        //public static bool IsLocked(long Id)
        //{
        //    return SqlFacade.Connection.Exists<User>("Id = @Id and Lock_By = @LockBy", new { Id = Id, LockBy = App.session.Username });
        //}

        //public static LockInfo GetLockInfo(long Id)
        //{
        //    var m = Select(Id);
        //    var l = new LockInfo();
        //    l.Id = Id;
        //    l.LockBy = m.LockBy;
        //    l.LockAt = m.LockAt;
        //    return l;
        //}

        //public static void Lock(long Id)
        //{
        //    DateTime ts = SqlFacade.GetCurrentTimeStamp();
        //    SqlFacade.Connection.UpdateOnly(new User { LockBy = App.session.Username, LockAt = ts }, p => new { p.LockBy, p.LockAt }, p => p.Id == Id);
        //}

        //public static void ReleaseLock(long Id)
        //{
        //    if (Id == 0) return;
        //    DateTime ts = SqlFacade.GetCurrentTimeStamp();
        //    SqlFacade.Connection.UpdateOnly(new User { LockBy = null }, p => p.LockBy, p => p.Id == Id);
        //}

        //public static bool IsExist(string Username, long Id = 0)
        //{
        //    return SqlFacade.Connection.Exists<User>("Id <> @Id and Username = @Username", new { Id = Id, Username = Username });
        //}

        public static void UpdatePwd(User m)
        {
            string sqlPwd = "select crypt('" + m.Pwd + "', gen_salt('bf'))";
            m.Pwd = SqlFacade.Connection.ExecuteScalar<string>(sqlPwd);
            var sql = SqlFacade.SqlUpdate(TableName, "Pwd = @Pwd", "Id = @Id");
            SqlFacade.Connection.Execute(sql, m);
        }

        public static bool IsPwdCorrect(long id, string pwd)
        {
            string sql = "SELECT (pwd = crypt(@pwd, pwd)) AS pswmatch FROM sm_user where id = @id";
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
}