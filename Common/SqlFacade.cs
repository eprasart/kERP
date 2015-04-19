using System;
using System.Data;
using System.Windows.Forms;
using kERP;
using Npgsql;
using Dapper;
using System.Text;
using System.IO;
using kERP.SYS;

namespace kERP
{
    class SqlFacade
    {
        private static string _ConnectionString;
        public static NpgsqlConnectionStringBuilder ConnectionStringBldr = new NpgsqlConnectionStringBuilder();

        public static string ConnectionString
        {
            get { return _ConnectionString; }
            set
            {
                ConnectionStringBldr.ConnectionString = value;
                // Check if username and password encrypted
                var usr = ConnectionStringBldr.UserName;
                string usrEncrypted = "", pwdEncrypted = "";

                if (Security.IsEncrypted(usr))
                {
                    usrEncrypted = usr;
                    ConnectionStringBldr.UserName = Security.DecryptString(usr);
                }
                else
                    usrEncrypted = Security.EncryptString(usr);
                var pwd = Security.ByteArrayToString(ConnectionStringBldr.PasswordAsByteArray);
                if (Security.IsEncrypted(pwd))
                {
                    pwdEncrypted = pwd;
                    ConnectionStringBldr.Password = Security.DecryptString(pwd);
                }
                else
                    pwdEncrypted = Security.EncryptString(pwd);
                _ConnectionString = ConnectionStringBldr.ConnectionString;

                var nsb = new NpgsqlConnectionStringBuilder(value);
                nsb.UserName = usrEncrypted;
                nsb.Password = pwdEncrypted;
                if (nsb.ConnectionString != value)
                    App.setting.Set("ConnectionString", nsb.ConnectionString); // Save back after changed/encrypted
            }
        }

        public static NpgsqlConnection Connection = null;

        #region "Sql Query Builder"
        /// <summary>
        /// Select SQL
        /// </summary>
        /// <param name="table">Table name</param>
        /// <param name="columns">Columns separated by comma</param>
        /// <param name="where">Full where clause</param>
        /// <param name="orderby"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static string SqlSelect(string table, string columns, string where = "", string orderby = "", long limit = 0, long offset = 0)
        {
            var sql = "select " + columns + "\nfrom " + table;
            if (where.Length > 0) sql += "\nwhere " + where;
            if (orderby.Length > 0) sql += "\norder by " + orderby;
            if (limit > 0) sql += "\nlimit " + limit;
            if (offset > 0) sql += " offset " + offset;
            return sql;
        }

        public static string SqlILike(string columns, string parameter = ":filter")
        {
            //code ilike :filter or description ilike :filter or phone ilike :filter or fax ilike :filter or email ilike :filter or address ilike :filter or note ilike :filter
            //code, description, phone => code ilike :filter or description ilike :filter

            var sLike = " ilike " + parameter;
            var sql = columns.Replace(", ", sLike + " or ");
            sql += sLike;
            return sql;
        }

        public static string SqlExists(string table, string where)
        {
            var sql = SqlSelect(table, "1", where);
            sql = "select exists(" + sql + ")";
            return sql;
        }

        /// <summary>
        /// Insert SQL
        /// </summary>
        /// <param name="table">Table name</param>
        /// <param name="columns">Columns, separated by comma</param>
        /// <param name="values">Values parameters. Make it empty for default, otherwise specify the complete one</param>
        /// <param name="returnSeq">True if want to return new inserted Id</param>
        /// <returns></returns>
        public static string SqlInsert(string table, string columns, string values, bool returnSeq = false)
        {
            if (values.Length == 0) values = ":" + columns.Replace(", ", ", :");    // if values is blank then will make it as parameter (:) of all columns
            var sql = string.Format("insert into {0} ({1})\nvalues ({2})", table, columns, values);
            if (returnSeq) sql += "\nreturning id";
            return sql;
        }

        public static string SqlInsert2(string table, string columns, bool returnSeq = false)
        {
            var sql = string.Format("insert into {0} ({1})\nvalues ({2})", table, columns);
            if (returnSeq) sql += "\nreturning id";
            return sql;
        }

        public static string SqlUpdate0(string table, string sets, string where = "")
        {
            var sql = "update " + table + " set " + sets;
            if (where.Length > 0) sql += "\nwhere " + where;
            return sql;
        }

        /// <summary>
        /// Update SQL
        /// </summary>
        /// <param name="table">Table name</param>
        /// <param name="columns">Columns to update, separated by comma</param>
        /// <param name="values">Blank for default or specify the one(s) with values. E.g. "change_at = now()"</param>
        /// <param name="where">Full where clause, e.g. "id = :id"</param>
        /// <returns></returns>
        public static string SqlUpdate(string table, string columns, string values, string where = "")
        {
            var sql = "update " + table + " set ";

            var cols = columns.Replace(" ", "").Split(',');
            var sVals = values.Replace(" ", "").Split(',');
            for (var i = 0; i < cols.Length; i++)
            {
                var bFound = false;
                foreach (var v in sVals)
                {
                    if (cols[i].Equals(v.Substring(0, v.IndexOf('='))))
                    {
                        bFound = true;
                        cols[i] += " = " + v.Substring(v.IndexOf('=') + 1);
                        break;
                    }
                }
                if (!bFound)
                    cols[i] += " = :" + cols[i];
            }
            sql += string.Join(", ", cols);
            if (where.Length > 0) sql += "\nwhere " + where;
            return sql;
        }

        public static string SqlDelete(string table, string where = "")
        {
            var sql = "delete from " + table;
            if (where.Length > 0) sql += " where " + where;
            return sql;
        }

        #endregion

        public static void EnsureDBSetup()
        {
            var ScriptPath = Path.Combine(App.StartupPath, "1_script_init.sql");
            var sql = Util.ReadTextFile(ScriptPath);
            if (sql.Length == 0) return;
            SqlFacade.Connection.Execute(sql);
        }

        public static void OpenConnection()
        {
            //var connection = new NpgsqlConnection(ConnectionString);
            //connection.Open();
            //return connection;            
            Connection = new NpgsqlConnection(ConnectionString);
            Connection.Open();
        }

        public static DateTime GetCurrentTimeStamp()
        {
            return Connection.ExecuteScalar<DateTime>("select now()");
        }

        public static string ExecuteString(string sql)
        {
            return Connection.ExecuteScalar<string>(sql);
        }

        public static DataTable GetDataTable(string sql)
        {
            return GetDataTable(new NpgsqlCommand(sql));
        }

        public static DataTable GetDataTable(NpgsqlCommand cmd)
        {
            cmd.Connection = new NpgsqlConnection(SqlFacade.ConnectionString);
            var da = new NpgsqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public static NpgsqlDataReader GetDataReader(string sql)
        {
            return GetDataReader(new NpgsqlCommand(sql));
        }

        public static NpgsqlDataReader GetDataReader(NpgsqlCommand cmd)
        {
            cmd.Connection = new NpgsqlConnection(SqlFacade.ConnectionString);
            cmd.Connection.Open();
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public static dynamic ExecuteQuery(string sql)
        {
            return Connection.Query(sql);
        }

        public static int ExportToCSV(string sql, string tableName)
        {
            //sql = "COPY (" + sql + ") TO '" + path + "' DELIMITER '" + delimiter + "' CSV HEADER ENCODING 'UTF8';";
            //Connection.ExecuteNonQuery(sql);            
            var result = 0;

            var sfd = new SaveFileDialog
            {
                CheckPathExists = true,
                OverwritePrompt = true,
                Filter = "CSV File (*.csv)|*.csv|All Files (*.*)|*.*",
                Title = "Export to CSV",
                FileName = tableName + "_" + DateTime.Now.ToString("yyMMdd_HHmmss")
            };
            if (sfd.ShowDialog() != DialogResult.OK) return result;
            var path = sfd.FileName;
            var fileName = Path.GetFileName(path);

            var fNotification = new frmNotification(string.Format(MessageFacade.export_exporting, fileName));
            fNotification.Show();
            Application.DoEvents();

            StringBuilder sb = new StringBuilder();
            try
            {
                var dr = GetDataReader(sql);

                var sLine = "";
                if (!dr.HasRows) return 1;  // No record
                for (int i = 0; i < dr.FieldCount; i++) // Column headers
                {
                    sLine += dr.GetName(i);
                    if (i < dr.FieldCount - 1) sLine += ConfigFacade.Export_Delimiter;
                }
                sb.AppendLine(sLine);

                while (dr.Read())   // Rows
                {
                    sLine = "";
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        sLine += "\"" + dr[i].ToString() + "\"";
                        if (i < dr.FieldCount - 1) sLine += ConfigFacade.Export_Delimiter;
                    }
                    sb.AppendLine(sLine);
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageFacade.Show(MessageFacade.error_export + "\r\n" + ex.Message, LabelFacade.sys_export, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrorLogFacade.Log(ex);
                return -1;
            }
            while (Util.IsFileLocked(path))   // Check if file is being used
            {
                if (MessageFacade.Show(string.Format(MessageFacade.file_being_used_try_again, fileName), LabelFacade.sys_export, MessageBoxButtons.RetryCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    fNotification.Close();
                    return 0;
                }
                continue;
            }
            using (var sw = new StreamWriter(path, false, Encoding.UTF8))   // Write to file  
            {
                sw.Write(sb);
            }
            fNotification.ShowMsg(string.Format(MessageFacade.export_opening, fileName));
            if (ConfigFacade.Export_Open_File_After) System.Diagnostics.Process.Start(path);   // Open file
            fNotification.Close();
            return result;
        }
    }
}
