﻿using System;
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
}