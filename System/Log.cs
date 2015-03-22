using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Npgsql;
using Dapper;
using kERP.SYS;
using System.IO;

namespace kERP
{
    class ErrorLog
    {
        public long Id { get; set; }
        public long Session_Id { get; set; }
        public DateTime? At { get; set; }
        public string Message { get; set; }
        public string Trace { get; set; }
        public string Info { get; set; }
        public string Status { get; set; }
    }

    class ErrorLogFacade
    {
        public static FileLog logFile = new FileLog();

        public static string Form_Load = "Form Load";

        public string FileLogPath
        {
            get { return logFile.FileName; }
            set { logFile.FileName = value; }
        }

        public static DataTable GetDataTable(string where, string filter = "")
        {
            var sql = "select l.id, username, login_at, logout_at, version, machine_name, machine_user_name, log_at, priority, module, type, message\n" +
                "from sm_session s\nleft join sm_session_log l on s.id = l.session_id where 1 = 1";
            //if (status.Length > 0)
            //    sql += " and status = '" + status + "'";
            if (filter.Length > 0)
                sql += " and (message ~* :filter or type ~* :filter or module ~* :filter)";
            sql += "\norder by login_at desc, log_at desc";
            var cmd = new NpgsqlCommand(sql, SqlFacade.Connection);
            if (filter.Length > 0)
                cmd.Parameters.AddWithValue(":filter", filter);
            var da = new NpgsqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        private static void Save(ErrorLog m)
        {
            try
            {
                m.Session_Id = App.session.Id;
                var sql = "insert into sys_error_log (session_id, message, trace, info) values (:session_id, :message, :trace, :info)";
                SqlFacade.Connection.Execute(sql, m);
            }
            catch (Exception ex)
            {
                LogToFile(ex);
            }
        }

        public static ErrorLog Select(long Id)
        {
            var sql = "select * from sys_error_log where id=@id";
            return SqlFacade.Connection.Query<ErrorLog>(sql, new { Id }).FirstOrDefault();
        }

        public static void SetStatus(long Id, string s)
        {
            var sql = "update sys_error_log set status = @Status where id = @Id";
            SqlFacade.Connection.Execute(sql, new { Status = s, Id });
        }

        /// <summary>
        /// Log error to database
        /// </summary>
        /// <param name="ex">Exception variable</param>
        /// <param name="info">Extra information</param>
        public static void Log(Exception ex, string info = "")
        {
            var log = new ErrorLog()
            {
                Message = ex.Message,
                Trace = ex.StackTrace,
                Info = info
            };
            Log(log);
        }

        public static void Log(string message)
        {
            var log = new ErrorLog()
            {
                Message = message
            };
            Log(log);
        }

        public static void Log(ErrorLog log)
        {
            Save(log);
        }

        public static void LogToFile(Exception ex, string info = "")
        {
            string log = "\r\n" + DateTime.Now.ToString("yyy-MM-dd ddd hh:mm:ss tt");
            log += "\r\n" + ex.ToString() + "\r\n" + ex.StackTrace;
            if (info.Length > 0) log += "\r\n" + info;
            logFile.Write(log);
        }
    }

    public class FileLog
    {
        private string mPath = "";

        public string FileName
        {
            get { return mPath; }
            set { mPath = value; Maintain(); }
        }

        public void Write(string content)
        {
            if (mPath.Length == 0) return;
            try
            {
                using (StreamWriter sr = new StreamWriter(mPath, true))
                {
                    sr.WriteLine(content);
                }
            }
            catch { }
        }

        /// <summary>
        /// Maintain log file. If file size > 3M then rename it. 
        /// </summary>
        private void Maintain(long size = 3)
        {
            if (!File.Exists(mPath)) return;
            double fSize = (new FileInfo(mPath).Length / 1024f) / 1024f;
            if (fSize > 3)
            {
                string sFile = Path.Combine(Path.GetDirectoryName(mPath), Path.GetFileNameWithoutExtension(mPath) + DateTime.Today.ToString("_yyMMdd") + Path.GetExtension(mPath));
                File.Move(mPath, sFile);
            }
        }
    }
}