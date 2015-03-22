using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kERP
{
    class Util
    {

        public static string RemoveLastDotZero(string v)
        {
            string s = v;
            if (v.EndsWith(".0"))
                return RemoveLastDotZero(v.Substring(0, v.Length - 2));
            else
                return s;
        }

        public static bool IsFileLocked(string path)
        {
            if (!File.Exists(path)) return false;
            FileStream stream = null;
            try
            {
                stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            //file is not locked
            return false;
        }

        public static string ReadTextFile(string path)
        {
            if (!File.Exists(path)) return "";
            var content = "";
            using (var sr = new StreamReader(path))
            {
                content = sr.ReadToEnd();
                sr.Close();
            }
            return content;
        }

        public static string RemoveCharacters(string value, string characters)
        {
            return new Regex("[" + characters + "]").Replace(value, "");
        }

        public static bool IsEmailValid(string email)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(email);
            return match.Success;
        }

        public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
        {
            MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
            return expressionBody.Member.Name;
        }

        public static string EscapeNewLine(string value)
        {
            if (value == null) return null;
            return value.Replace(@"\r\n", Environment.NewLine);
        }

        public static bool IsInteger(string value)
        {
            int isNumber = 0;
            return int.TryParse(value, out isNumber);
        }

        public static bool IsDouble(string value)
        {
            double isNumber = 0;
            return double.TryParse(value, out isNumber);
        }

        public static string ConcatCodeDescription(string code, string description)
        {
            return string.Format("{0}{1}{2}", code, ConfigFacade.Code_Description_Separator, description);
        }
    }

    public static class FormFacade
    {
        static bool isPointVisibleOnAScreen(Point p)
        {
            foreach (Screen s in Screen.AllScreens)
            {
                if (p.X > s.Bounds.Right && p.X > s.Bounds.Left && p.Y > s.Bounds.Top && p.Y < s.Bounds.Bottom)
                    return true;
            }
            return false;
        }

        public static void SaveFormSate(Form frm, string prefix = "")
        {
            if (prefix.Length == 0) prefix = frm.Name;
            ConfigFacade.Set(prefix + Constant.Location, frm.Location);
            ConfigFacade.Set(prefix + Constant.Window_State, frm.WindowState);
            if (frm.WindowState == FormWindowState.Normal && (frm.FormBorderStyle == FormBorderStyle.Sizable || frm.FormBorderStyle == FormBorderStyle.SizableToolWindow))
                ConfigFacade.Set(prefix + Constant.Size, frm.Size);
        }

        public static void SetFormState(Form frm, string prefix = "")
        {
            frm.Icon = Properties.Resources.Icon;
            if (prefix.Length == 0) prefix = frm.Name;
            var lo = ConfigFacade.GetPoint(prefix + Constant.Location);
            if (lo != new Point(-1, -1) && isPointVisibleOnAScreen(lo))
                frm.Location = lo;
            //else
            //todo: record for future find out
            if (frm.FormBorderStyle == FormBorderStyle.Sizable || frm.FormBorderStyle == FormBorderStyle.SizableToolWindow)
            {
                var si = ConfigFacade.GetSize(prefix + Constant.Size);
                if (si != new System.Drawing.Size(-1, -1))
                    frm.Size = si;
            }
            frm.WindowState = ConfigFacade.GetWindowState(prefix + Constant.Window_State, "0");
        }
    }

    class Currency
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        // todo: more here
        public string Note { get; set; }
        public string Status { get; set; }
        public string Insert_By { get; set; }
        public DateTime? Insert_At { get; set; }
        public string Change_By { get; set; }
        public DateTime? Change_At { get; set; }
    }

    static class CurrencyFacade
    {
        public static readonly string TableName = "currency";
        private static string Currency;
        private static double RoundUnit; // Temporary store the round unit of the currency
        //private static string RoundRule;
        public static string Format;

        public static string GetBase()
        {
            var sql = SqlFacade.SqlSelect(TableName, "code", "is_base = 'Y'");
            return SqlFacade.Connection.ExecuteScalar<string>(sql);
        }

        public static void LoadSetting(string currency)
        {
            Currency = currency;

            var sql = SqlFacade.SqlSelect(TableName, "round_unit, format", "code = :code");
            //RoundUnit =SqlFacade.Connection.ExecuteScalar<double>(sql, new { code = currency });
            var dr = SqlFacade.Connection.ExecuteReader(sql, new { code = currency });
            dr.Read();
            RoundUnit = double.Parse(dr["round_unit"].ToString());
            Format = dr["format"].ToString();
            dr.Close();
        }

        public static double Round(double value, string roundRule)
        {
            double result = 0;
            switch (roundRule)
            {
                case "R":
                    result = RoundUpDown(value);
                    break;
                case "U":
                    result = RoundUp(value);
                    break;
                case "D":
                    result = RoundDown(value);
                    break;
            }
            return result;
        }

        private static double RoundUp(double value)
        {
            double result = Math.Ceiling(value / RoundUnit) * RoundUnit;
            return result;
        }

        private static double RoundDown(double value)
        {
            double result = Math.Floor(value / RoundUnit) * RoundUnit;
            return result;
        }

        private static double RoundUpDown(double value)
        {
            double result = Math.Round(value / RoundUnit) * RoundUnit;
            return result;
        }

        public static DataTable GetDataTable(string filter = "", string status = "")
        {
            var sql = SqlFacade.SqlSelect(TableName, "id, code, name", "1 = 1");
            sql += " and status " + (status.Length == 0 ? "<>" : "=") + " :status";
            if (status.Length == 0) status = Constant.RecordStatus_Deleted;
            if (filter.Length > 0)
                sql += " and (" + SqlFacade.SqlILike("code, name") + ")";
            sql += "\norder by code\nlimit " + ConfigFacade.Select_Limit;
            var cmd = new NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue(":status", status);
            if (filter.Length > 0)
                cmd.Parameters.AddWithValue(":filter", "%" + filter + "%");

            return SqlFacade.GetDataTable(cmd);
        }

        public static Branch Select(long Id)
        {
            var sql = SqlFacade.SqlSelect(TableName, "*", "id = :id");
            return SqlFacade.Connection.Query<Branch>(sql, new { Id }).FirstOrDefault();
        }

        public static void LoadList(ComboBox cbo)
        {
            string sql = SqlFacade.SqlSelect("currency", "code, name, is_base", "status = 'A'", "code");
            var dt = SqlFacade.GetDataTable(sql);
            cbo.DataSource = dt;
            cbo.ValueMember = "code";
            cbo.DisplayMember = "name";
            var dr = dt.Select("is_base = 'Y'");
            if (dr.Length > 0)
                cbo.SelectedValue = dr[0][0];
            else
                cbo.SelectedIndex = -1;
        }

        public static void SetStatus(long Id, string status)
        {
            var sql = SqlFacade.SqlUpdate(TableName, "status, change_by, change_at", "change_at = now()", "id = :id");
            SqlFacade.Connection.Execute(sql, new { status, Change_By = App.session.Username, Id });
        }

        public static bool Exists(string Code, long Id = 0)
        {
            var sql = SqlFacade.SqlExists(TableName, "id <> :id and status <> :status and code = :code");
            var bExists = false;
            try
            {
                bExists = SqlFacade.Connection.ExecuteScalar<bool>(sql, new { Id, Status = Constant.RecordStatus_Deleted, Code });
            }
            catch (Exception ex)
            {
                MessageFacade.Show(MessageFacade.error_query + "\r\n" + ex.Message, LabelFacade.sys_branch, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

    public static class DateFacade
    {
        public static bool IsSaturday(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday;
        }

        public static bool IsSunday(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Sunday;
        }

        public static bool IsWeekend(DateTime date)
        {
            return IsSaturday(date) || IsSunday(date);
        }

        public static bool IsHoliday(DateTime date)
        {
            var sql = "select id\nfrom holiday\nwhere extract(year from date) IN(1900, :year) and to_char(date, 'MMdd') = :MMdd";
            return SqlFacade.Connection.ExecuteScalar<bool>(sql, new { year = date.Year, MMdd = date.ToString("MMdd") });
        }

        public static DateTime GetNextWorkingDay(DateTime date, string neverOn, string move)
        {
            bool skipSat = neverOn.Contains("6");
            bool skipSun = neverOn.Contains("0");
            bool skipHoliday = neverOn.Contains("H");
            int direction = (move == "F" ? 1 : -1);
            while ((skipSat && IsSaturday(date)) || (skipSun && IsSunday(date)) || skipHoliday && IsHoliday(date))
            {
                date = date.AddDays(direction);
            }
            return date;
        }
    }

    public static class ImageFacade
    {
        public static byte[] GetBytes(string path)
        {
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            var br = new BinaryReader(fs);
            byte[] bytes = br.ReadBytes((int)fs.Length);
            br.Close();
            fs.Close();
            return bytes;
        }
    }
}
