using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Npgsql;
using Dapper;
using System.Drawing;
using System.Windows.Forms;
using System.Linq.Expressions;

namespace kERP
{
    public static class SY
    {

    }

    class ConfigItem
    {
        const string TableName = "sys_config";
        private string _value;

        public long Id { private get; set; }
        public string Code { get; set; }
        public string Username { get; set; }
        public bool Changed { private get; set; }

        public ConfigItem()
        { }

        public ConfigItem(string code, string username, string defaultValue)
        {
            Code = code;
            Username = username;
            _value = defaultValue;    // Not "Value = defaultValue" to avoid "Changed = True"
            Get();
            //Changed = false;
        }

        public string Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    Changed = true;
                }
            }
        }

        public void Save()
        {
            if (!Changed) return;
            var sql = SqlFacade.SqlUpdate(TableName, "value", "value = :value", "id = :id");
            Id = SqlFacade.Connection.ExecuteScalar<long>(sql, new { Value, Id });
        }

        private void Get()
        {
            var sWhere = "code ~* :code";
            if (Username.Length > 0)
                sWhere = "username ~* :username and " + sWhere;
            var sql = SqlFacade.SqlSelect(TableName, "id, value", sWhere);

            ConfigItem result = null;
            if (Username.Length > 0)
                result = SqlFacade.Connection.Query<ConfigItem>(sql, new { Username, Code }).FirstOrDefault();
            else
                result = SqlFacade.Connection.Query<ConfigItem>(sql, new { Code }).FirstOrDefault();
            if (result == null)
                Add();
            else
            {
                Id = result.Id;
                _value = result.Value;
            }
        }

        private void Add()
        {
            var sql = SqlFacade.SqlInsert(TableName, "username, code, value", "", true);
            Id = SqlFacade.Connection.ExecuteScalar<long>(sql, new { Username, Code, Value });
        }
    }

    class Config
    {
        const string TableName = "sys_config";

        private string _value;

        private bool Changed { get; set; }

        public long Id { get; set; }
        public string Username { get; set; }
        public string Code { get; set; }

        public string Value
        {
            get { return _value; }
            set
            {
                if (_value != value && value != null)
                {
                    _value = value;
                    Changed = true;
                }
            }
        }

        public string Note { get; set; }
        public String Status { get; set; }

        public Config() { }

        public Config(string username, string code, string defaultValue, string note)
        {
            Username = username;
            Code = code.Substring(1); // skip the _
            Value = defaultValue;
            Note = note;
            Get();
            Changed = false;
        }

        public override string ToString()
        {
            return Value;
        }

        public int ValueInt
        {
            get { return int.Parse(Value); }
        }

        public bool ValueBool
        {
            get { return Value == "Y" || Value == "T" ? true : false; }
        }

        public Point ValuePoint
        {
            get
            {
                if (Value == "") return new Point(-1, -1);
                Value = Util.RemoveCharacters(Value.ToUpper(), "{}XY= "); // X Y
                string[] coords = Value.Split(',');
                return new Point(int.Parse(coords[0]), int.Parse(coords[1]));
            }
        }

        public Size ValueSize
        {
            get
            {
                if (Value == "") return new Size(-1, -1);
                Value = Util.RemoveCharacters(Value.ToUpper(), "{}WIDTHEG= ");    // WIDTH HEIGH
                string[] coords = Value.Split(',');
                return new Size(int.Parse(coords[0]), int.Parse(coords[1]));
            }
        }

        private void Add()
        {
            var sql = SqlFacade.SqlInsert(TableName, "username, code, value, note", "", true);
            Id = SqlFacade.Connection.ExecuteScalar<long>(sql, new { Username, Code, Value, Note });
        }

        private void Get()
        {
            var sWhere = "code ~* :code";
            if (Username.Length > 0)
                sWhere = "username ~* :username and " + sWhere;
            var sql = SqlFacade.SqlSelect(TableName, "id, value as value", sWhere);

            Config result = null;
            if (Username.Length > 0)
                result = SqlFacade.Connection.Query<Config>(sql, new { Username, Code }).FirstOrDefault();
            else
                result = SqlFacade.Connection.Query<Config>(sql, new { Code }).FirstOrDefault();
            if (result == null)
                Add();
            else
            {
                Id = result.Id;
                Value = result.Value;
            }
        }

        public void Save()
        {
            if (!Changed) return;
            var sql = SqlFacade.SqlUpdate(TableName, "value", "", "id = :id");
            try
            {
                SqlFacade.Connection.Execute(sql, new { Value, Id });
            }
            catch (Exception ex)
            {
                ErrorLogFacade.LogToFile(ex, "sql='" + sql + "')");
            }
        }
    }

    static class ConfigFacade
    {
        const string TableName = "sys_config";
        static Dictionary<string, ConfigItem> configList = new Dictionary<string, ConfigItem>();

        public static string Language;
        public static string Select_Limit;
        public static string Code_Casing;
        public static string Code_Description_Separator;
        public static CharacterCasing Character_Casing;
        public static int Code_Max_Length;

        public static bool Log_Activity;
        public static bool Log_Missing_Label;

        public static string Toolbar_Icon_Display_Type;
        public static ToolStripItemDisplayStyle Toolbar_Icon_Display_Style;

        public static string Export_Delimiter;
        public static bool Export_Open_File_After;

        private static void SetCharacterCasing()
        {
            CharacterCasing cs;
            switch (ConfigFacade.Code_Casing)
            {
                case "U":
                    cs = CharacterCasing.Upper;
                    break;
                case "L":
                    cs = CharacterCasing.Lower;
                    break;
                default:
                    cs = CharacterCasing.Normal;
                    break;
            }
            Character_Casing = cs;
        }

        private static void SetToolStripItemDisplayStyle()
        {
            ToolStripItemDisplayStyle ds;
            switch (Toolbar_Icon_Display_Type)
            {
                case "I":
                    ds = ToolStripItemDisplayStyle.Image;
                    break;
                case "T":
                    ds = ToolStripItemDisplayStyle.Text;
                    break;
                default:
                    ds = ToolStripItemDisplayStyle.ImageAndText;
                    break;
            }
            Toolbar_Icon_Display_Style = ds;
        }

        public static void Load()
        {
            Language = Get("sys_language", "ENG");
            Select_Limit = Get("sys_select_limit", "1000");
            Code_Casing = Get("sys_code_casing", "N");
            Code_Description_Separator = Get("sys_code_description_separator", " - ");
            SetCharacterCasing();
            Code_Max_Length = GetInt("sys_code_max_length", "15");

            Log_Activity = GetBool("sys_log_activity", "Y");    // Session log
            Log_Missing_Label = GetBool("sys_log_missing_label", App.session.Username, "N");    // Speed up by not not write too much to db unless turn on

            Toolbar_Icon_Display_Type = Get("sys_toolbar_icon_display_type", App.session.Username, "IT");
            SetToolStripItemDisplayStyle();
            Export_Delimiter = Get("sys_export_delimiter", ",");
            Export_Open_File_After = GetBool("sys_export_open_file_after", App.session.Username, "Y");
        }

        private static void Insert(string username, string code, string value)
        {
            var sql = SqlFacade.SqlInsert(TableName, "username, code, value", "", true);
            SqlFacade.Connection.ExecuteScalar<long>(sql, new { username, code, value });
        }

        public static string Get(string code, string username, string defaultValue = "")
        {
            ConfigItem s;
            if (!configList.TryGetValue(code, out s))
            {
                s = new ConfigItem(code, username, defaultValue);
                configList.Add(code, s);
            }
            return s.Value;
        }

        public static string GetByUser(string code, string defaultValue = "")
        {
            return Get(code, App.session.Username, defaultValue);
        }
        
        public static void Set(string code, string value)
        {
            if (configList.ContainsKey(code))
                if (configList[code].Value != value)
                    configList[code].Value = value;
        }

        public static void Set(string code, int value)
        {
            Set(code, value.ToString());
        }

        public static string Get(string code, string defaultValue = "")
        {
            return Get(code, "", defaultValue);
        }

        public static string GetUpper(string code, string defaultValue = "")
        {
            return Get(code, defaultValue).ToUpper();
        }

        public static int GetInt(string code, string defaultValue = "0")
        {
            return int.Parse(Get(code, defaultValue));
        }

        public static bool GetBool(string code, string defaultValue = "")
        {
            return GetBool(code, "", defaultValue);
        }

        public static bool GetBool(string code, string username, string defaultValue = "")
        {
            return (Get(code, username, defaultValue) == "Y");
        }

        public static int GetSplitterDistance(string frmName, string defaultValue = "230")
        {
            return int.Parse(Get(frmName + Constant.Splitter_Distance, App.session.Username, defaultValue));
        }

        public static void SetSplitterDistance(string frmName, int value)
        {
            Set(frmName + Constant.Splitter_Distance, value);
        }

        public static FormWindowState GetWindowState(string code, string defaultValue = "")
        {
            return (FormWindowState)int.Parse(Get(code, App.session.Username, defaultValue));
        }

        public static void Set(string code, FormWindowState value)
        {
            string s = ((int)value).ToString();
            Set(code, s);
        }

        public static Size GetSize(string code, string defaultValue = "")
        {
            string s = Get(code, App.session.Username, defaultValue);
            int width = -1, height = -1;
            if (s.Contains(","))
            {
                string[] dims = s.Split(',');
                int.TryParse(dims[0], out width);
                int.TryParse(dims[1], out height);
            }
            return new System.Drawing.Size(width, height);
        }

        public static void Set(string code, Size value)
        {
            string s = string.Format("{0}, {1}", value.Width, value.Height);
            Set(code, s);
        }

        public static Point GetPoint(string code, string defaultValue = "")
        {
            string s = Get(code, App.session.Username, defaultValue);
            int x = -1, y = -1;
            if (s.Contains(","))
            {
                string[] dims = s.Split(',');
                int.TryParse(dims[0], out x);
                int.TryParse(dims[1], out y);
            }
            return new System.Drawing.Point(x, y);
        }

        public static void Set(string code, Point value)
        {
            string s = string.Format("{0}, {1}", value.X, value.Y);
            Set(code, s);
        }

        // Save [only] changes to database
        public static void Save()
        {
            foreach (KeyValuePair<string, ConfigItem> p in configList)
            {
                p.Value.Save();
            }
        }
    }

    class LabelFacade
    {
        const string TableName = "sys_label";

        public static string sys_msg_prefix;

        // Form
        public static string SYS_Branch;

        public static string IC_Category;
        public static string IC_Classification;
        public static string IC_Location;
        public static string IC_Item_Location;
        public static string IC_Item_Supplier;
        public static string IC_Item;
        public static string IC_Unit_Measure;

        public static string AP_Supplier;

        public static string SM_User;
        public static string SM_Role;
        public static string SM_Function;
        public static string SM_UserRole;
        public static string SM_UserFunction;
        public static string SM_AuditLog;


        // Button
        public static string sys_cancel;
        public static string sys_close;
        public static string sys_copy;
        public static string sys_delete;

        public static string sys_lock;
        public static string sys_new;
        public static string sys_save;
        public static string sys_unlock;

        // Toolbar
        public static string sys_button_new;
        public static string sys_button_copy;
        public static string sys_button_cancel;
        public static string sys_button_unlock;
        public static string sys_button_save;
        public static string sys_button_save_new;
        public static string sys_button_active;
        public static string sys_button_inactive;
        public static string sys_button_delete;
        public static string sys_button_mode;
        public static string sys_export;

        // Find
        public static string sys_button_find;
        public static string sys_button_clear;
        public static string sys_button_filter;
        public static string sys_search_place_holder;

        // Message Box Buttons
        public static string sys_button_abort;
        public static string sys_button_retry;
        public static string sys_button_ignore;
        public static string sys_button_ok;
        public static string sys_button_yes;
        public static string sys_button_no;


        public static string Get(string code)
        {
            var language = ConfigFacade.Language;
            var sql = SqlFacade.SqlSelect(TableName, "value", "code = lower(:code) and language = :language");
            var label = SqlFacade.Connection.ExecuteScalar<string>(sql, new { code, language });
            if (label == null && ConfigFacade.Log_Missing_Label)
                ErrorLogFacade.Log("Label: code=" + code + " not exist");
            return label;
        }

        private static void Insert(string code, string value)
        {
            var sql = SqlFacade.SqlInsert(TableName, "code, language, value", "", true);
            SqlFacade.Connection.ExecuteScalar<long>(sql, new { code, language = ConfigFacade.Language, value });
        }

        public static string Get(string code, string defaultValue)
        {
            string label = Get(code);
            if (label == null)
            {
                Insert(code, defaultValue);
                label = defaultValue;
            }
            return label;
        }

        public static void Load()
        {
            sys_msg_prefix = Get("sys_msg_prefix", "- ");
            SYS_Branch = Get("sys_branch");

            IC_Category = Get("ic_category");
            IC_Classification = Get("ic_classification");
            IC_Unit_Measure = Get("ic_unit_measure");
            IC_Location = Get("ic_location");
            IC_Item = Get("ic_item");
            IC_Item_Location = Get("ic_item_location");
            IC_Item_Supplier = Get("ic_item_supplier");
            AP_Supplier = Get("ap_supplier");


            SM_User = Get("sm_user");
            SM_Role = Get("sm_role");
            SM_Function = Get("sm_function");
            SM_UserRole = Get("sm_userrole");
            SM_UserFunction = Get("sm_userfunction");
            SM_AuditLog = Get("sm_audit_log");

            sys_cancel = Get("sys_cancel");
            sys_close = Get("sys_close");
            sys_copy = Get("sys_copy");
            sys_delete = Get("sys_delete");

            sys_lock = Get("sys_lock");
            sys_new = Get("sys_new");
            sys_save = Get("sys_save");
            sys_unlock = Get("sys_unlock");

            sys_button_new = Get("sys_button_new");
            sys_button_copy = Get("sys_button_copy");
            sys_button_cancel = Get("sys_button_cancel");
            sys_button_unlock = Get("sys_button_unlock");
            sys_button_save = Get("sys_button_save");
            sys_button_save_new = Get("sys_button_save_new");
            sys_button_active = Get("sys_button_active");
            sys_button_inactive = Get("sys_button_inactive");
            sys_button_delete = Get("sys_button_delete");
            sys_button_mode = Get("sys_button_mode");
            sys_export = Get("sys_export");

            sys_button_find = Get("sys_button_find");
            sys_button_clear = Get("sys_button_clear");
            sys_button_filter = Get("sys_button_filter");

            sys_button_abort = Get("sys_button_abort");
            sys_button_retry = Get("sys_button_retry");
            sys_button_ignore = Get("sys_button_ignore");
            sys_button_ok = Get("sys_button_ok");
            sys_button_yes = Get("sys_button_yes");
            sys_button_no = Get("sys_button_no");
            sys_search_place_holder = Get("sys_search_place_holder");
        }
    }

    class MessageFacade
    {
        const string TableName = "sys_message";

        public static string active_inactive;
        public static string error_active_inactive;
        public static string error_retrieve_data;

        public static string delete_confirmation;
        public static string error_delete;
        public static string delete_locked;
        public static string lock_currently;
        public static string error_lock;
        public static string lock_override;
        public static string privilege_no_access;
        public static string proceed_confirmation;
        public static string error_load_record;
        public static string save_confirmation;
        public static string error_save;
        public static string error_unlock;
        public static string error_load_form;
        public static string error_query;
        public static string error_export;

        public static string code_already_exists;
        public static string code_not_empty;

        public static string email_not_valid;
        public static string location_type_not_empty;

        public static string export_exporting;
        public static string export_opening;
        public static string file_being_used_try_again;

        public static DialogResult Show(string msg, string title = "", MessageBoxButtons buttons = MessageBoxButtons.OK,
            MessageBoxIcon icon = MessageBoxIcon.Information, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
        {
            var fMsg = new frmMsg(msg, title, buttons, icon, defaultButton);
            if (title == null || title.Length == 0) title = LabelFacade.sys_save;
            fMsg.Text = title;
            DialogResult dResult = DialogResult.OK;
            if (buttons == MessageBoxButtons.OK && icon == MessageBoxIcon.Information)
                fMsg.Show();
            else
                dResult = fMsg.ShowDialog();
            return dResult;
        }

        public static void Show(IWin32Window owner, ref frmMsg fMsg, string msg, string title)
        {
            if (title == null || title.Length == 0) title = LabelFacade.sys_save;
            if (fMsg == null || fMsg.IsDisposed)
            {
                fMsg = new frmMsg(msg, title);
                fMsg.Show(owner);
            }
            else
            {
                fMsg.Title = title;
                fMsg.Message = msg;
            }
            //if (fMsg.Visible == false) fMsg.Visible = true;
        }

        public static void Load()
        {
            //todo: reload when language changed
            active_inactive = Get("active_inactive"); //Get(Util.GetMemberName(() => error_active_inactive));
            error_active_inactive = Get("error_active_inactive");
            error_retrieve_data = Get("error_retrieve_data");
            delete_confirmation = Get("delete_confirmation");
            error_delete = Get("error_delete");
            delete_locked = Get("delete_locked");
            lock_currently = Get("lock_currently");
            error_lock = Get("error_lock");
            lock_override = Get("lock_override");
            privilege_no_access = Get("privilege_no_access");
            proceed_confirmation = Get("proceed_confirmation");
            error_load_record = Get("error_load_record");
            save_confirmation = Get("save_confirmation");
            error_save = Get("error_save");
            error_unlock = Get("error_unlock");
            error_load_form = Get("error_load_form");
            error_query = Get("error_query");
            error_export = Get("error_export");

            code_already_exists = Get("code_already_exists");
            code_not_empty = Get("code_not_empty");

            email_not_valid = Get("email_not_valid");
            location_type_not_empty = Get("location_type_not_empty");

            export_exporting = Get("export_exporting");
            export_opening = Get("export_opening");
            file_being_used_try_again = Util.EscapeNewLine(Get("file_being_used_try_again"));
        }

        public static string Get(string code)
        {
            var language = ConfigFacade.Language;
            var sql = SqlFacade.SqlSelect(TableName, "value", "code = lower(:code) and language = :language");
            var message = SqlFacade.Connection.ExecuteScalar<string>(sql, new { code, language });
            if (message == null)
            {
                ErrorLogFacade.Log("Message: code=" + code + " not exist");
                message = code;
            }
            return message;
        }
    }
}