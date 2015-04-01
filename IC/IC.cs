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
    class Location : BaseTable
    {
        public string Branch_Code { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
    }

    static class LocationFacade
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

    class Category : BaseTable
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

    static class CategoryFacade
    {
        public static readonly string TableName = "ic_category";
        public static readonly string TitleLabel = LabelFacade.IC_Category;

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

        public static long Save(Category m)
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

        public static Category Select(long Id)
        {
            var sql = SqlFacade.SqlSelect(TableName, "*", "id = :id");
            return SqlFacade.Connection.Query<Category>(sql, new { Id }).FirstOrDefault();
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

    class UnitMeasure : BaseTable
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public double Default_Factor { get; set; }
    }

    static class UnitMeasureFacade
    {
        public static readonly string TableName = "ic_unit_measure";
        public static readonly string TitleLabel = LabelFacade.IC_Category;

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

        public static long Save(UnitMeasure m)
        {
            string sql = "code, description, note, default_factor";
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

        public static UnitMeasure Select(long Id)
        {
            var sql = SqlFacade.SqlSelect(TableName, "*", "id = :id");
            return SqlFacade.Connection.Query<UnitMeasure>(sql, new { Id }).FirstOrDefault();
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

    class Classification : BaseTable
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Parent { get; set; }
    }

    static class ClassificationFacade
    {
        public static readonly string TableName = "ic_classification";
        public static readonly string TitleLabel = LabelFacade.IC_Category;

        public static DataTable GetDataTable(string filter = "", string status = "")
        {
            var sql = SqlFacade.SqlSelect(TableName + " c\nleft join ic_classification p on p.code = c.parent", "c.id, c.code, c.description, p.description parent, c.note", "1 = 1");
            sql += " and c.status " + (status.Length == 0 ? "<>" : "=") + " :status";
            if (status.Length == 0) status = Constant.RecordStatus_Deleted;
            if (filter.Length > 0)
                sql += " and (" + SqlFacade.SqlILike("c.code, c.description, c.note") + ")";
            sql += "\norder by c.code\nlimit " + ConfigFacade.Select_Limit;
            var cmd = new NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue(":status", status);
            if (filter.Length > 0)
                cmd.Parameters.AddWithValue(":filter", "%" + filter + "%");
            return SqlFacade.GetDataTable(cmd);
        }

        public static long Save(Classification m)
        {
            string sql = "code, description, note, parent";
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

        public static Classification Select(long Id)
        {
            var sql = SqlFacade.SqlSelect(TableName, "*", "id = :id");
            return SqlFacade.Connection.Query<Classification>(sql, new { Id }).FirstOrDefault();
        }

        public static void LoadList(ComboBox cbo, long Id = 0)
        {
            string sql = SqlFacade.SqlSelect(TableName, "'' code, '' description union all\nselect code, description", "id <> :id and status = 'A'", "description");
            var cmd = new NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue("id", Id);
            cbo.DataSource = SqlFacade.GetDataTable(cmd);
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

    class Item : BaseTable
    {
        public string Branch_Code { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public string Classification { get; set; }
        public string Barcode { get; set; }
        public string Currency { get; set; }
        public double Price { get; set; }
        public string UPC_Code { get; set; }
        public string ABC_Code { get; set; }
        public string Allow_Discount { get; set; }
        public byte[] Picture { get; set; }
    }

    static class ItemFacade
    {
        public static readonly string TableName = "ic_item";
        public static readonly string TitleLabel = LabelFacade.IC_Item;

        public static DataTable GetDataTable(string filter = "", string status = "")
        {
            var sql = SqlFacade.SqlSelect(TableName + " i\ninner join list d on i.allow_discount = d.code\ninner join list t on i.type = t.code\n" +
                "left join ic_category ca on ca.code = i.category\nleft join ic_classification cl on cl.code = i.classification",
                "i.id, i.code, i.description, description2, barcode, currency, price, d.description allow_discount, t.description as type, ca.description category, cl.description classification", "1 = 1");
            sql += " and i.status " + (status.Length == 0 ? "<>" : "=") + " :status";
            if (status.Length == 0) status = Constant.RecordStatus_Deleted;
            if (filter.Length > 0)
                sql += " and (" + SqlFacade.SqlILike("i.code, i.description, description2, barcode, upc_code") + ")";
            sql += "\norder by i.code\nlimit " + ConfigFacade.Select_Limit;
            var cmd = new NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue(":status", status);
            if (filter.Length > 0) cmd.Parameters.AddWithValue(":filter", "%" + filter + "%");
            return SqlFacade.GetDataTable(cmd);
        }

        public static long Save(Item m)
        {
            string sql = "code, description, description2, barcode, currency, price, type, category, classification, upc_code, abc_code, allow_discount, ";
            if (m.Picture != null) sql += "picture, ";
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

        public static Item Select(long Id)
        {
            var sql = SqlFacade.SqlSelect(TableName, "*", "id = :id");
            return SqlFacade.Connection.Query<Item>(sql, new { Id }).FirstOrDefault();
        }

        public static Item SelectLessCols(string code)
        {
            var sql = SqlFacade.SqlSelect(TableName, "code, description, barcode", SqlFacade.SqlILike("code, description, barcode", ":p"));
            return SqlFacade.Connection.Query<Item>(sql, new { p = code }).FirstOrDefault();
        }

        public static Item SelectLessCols(long Id)
        {
            var sql = SqlFacade.SqlSelect(TableName, "code, description, barcode", "Id = :Id");
            return SqlFacade.Connection.Query<Item>(sql, new { Id }).FirstOrDefault();
        }


        //public static string GetDescription(string code)
        //{
        //    var sql = SqlFacade.SqlSelect(TableName, "description", "code = :code and status = 'A'");
        //    return SqlFacade.Connection.ExecuteScalar<string>(sql, new { code });
        //}

        public static int GetCount(string value)
        {
            var sql = SqlFacade.SqlSelect(TableName, "count (*)", "status = 'A' and (" + SqlFacade.SqlILike("code, description, barcode", ":p") + ")");
            return SqlFacade.Connection.ExecuteScalar<int>(sql, new { p = value });
        }

        public static void ClearPicture(long Id)
        {
            var sql = SqlFacade.SqlUpdate(TableName, "picture, change_by, change_at", "picture = null, change_at = now()", "id = :id");
            SqlFacade.Connection.Execute(sql, new { Change_By = App.session.Username, Id });
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
    }

    class ItemLocation : BaseTable
    {
        public string item_code { get; set; }
        public string location_code { get; set; }
        public string default_supplier_code { get; set; }
        public string last_supplier_code { get; set; }
        public double avg_cost { get; set; }
        public double std_cost { get; set; }
        public double last_cost { get; set; }
        public double ptd_usage_val { get; set; }
        public double ptd_sale_val { get; set; }
        public double ptd_usage_qty { get; set; }
        public double ptd_sale_qty { get; set; }
        public double ytd_usage_val { get; set; }
        public double ytd_sale_val { get; set; }
        public double ytd_usage_qty { get; set; }
        public double ytd_sale_qty { get; set; }
        public double lsoaloc { get; set; }
        public double lwoaloc { get; set; }
        public double onhand { get; set; }
        public double order_point { get; set; }
        public double order_qty { get; set; }
        public double delivery_lead_time { get; set; }
        public DateTime last_sale { get; set; }
        public DateTime last_receipt { get; set; }
        public DateTime last_po { get; set; }
        public DateTime last_count { get; set; }
        public string count_flag { get; set; }
    }

    static class ItemLocationFacade
    {
        public static readonly string TableName = "ic_item_location";
        public static readonly string TitleLabel = LabelFacade.IC_Item_Location;

        public static DataTable GetDataTable(string filter, string status, string searchCols)
        {
            var sql = SqlFacade.SqlSelect(TableName + " il\nleft join ic_item i on i.code = il.item_code\nleft join ic_location l on l.code = il.location_code\n" +
                "left join ap_vendor v on v.code = il.default_supplier_code\nleft join sys_config c on c.code = 'sys_code_description_separator'",
                "il.id, item_code || c.value || i.description item, location_code || c.value || l.description as location, default_supplier_code || c.value || v.description supplier, " +
                "il.std_cost, il.last_cost, il.onhand, order_point, order_qty, delivery_lead_time", "1 = 1");
            sql += " and il.status " + (status.Length == 0 ? "<>" : "=") + " :status";
            if (status.Length == 0) status = Constant.RecordStatus_Deleted;
            if (filter.Length > 0)
                sql += " and (" + SqlFacade.SqlILike(searchCols) + ")";
            sql += "\norder by item_code\nlimit " + ConfigFacade.Select_Limit;

            var cmd = new NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue(":status", status);
            if (filter.Length > 0)
                cmd.Parameters.AddWithValue(":filter", "%" + filter + "%");

            return SqlFacade.GetDataTable(cmd);
        }

        public static long Save(ItemLocation m)
        {
            string sql = "item_code, location_code, default_supplier_code, last_supplier_code, avg_cost, std_cost, last_cost, ptd_usage_val, ptd_sale_val, ptd_usage_qty, ptd_sale_qty, ytd_usage_val, ytd_sale_val, ytd_usage_qty, ytd_sale_qty, lsoaloc, lwoaloc, onhand, order_point, order_qty, delivery_lead_time, last_sale, last_receipt, last_po, note, ";
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

        public static ItemLocation Select(long Id)
        {
            var sql = SqlFacade.SqlSelect(TableName, "*", "id = :id");
            return SqlFacade.Connection.Query<ItemLocation>(sql, new { Id }).FirstOrDefault();
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

        public static bool Exists(string item_code, string location_code, long Id = 0)
        {
            var sql = SqlFacade.SqlExists(TableName, "id <> :id and status <> :status and item_code = :item_code and location_code = :location_code");
            var bExists = false;
            try
            {
                bExists = SqlFacade.Connection.ExecuteScalar<bool>(sql, new { Id, Status = Constant.RecordStatus_Deleted, item_code, location_code });
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