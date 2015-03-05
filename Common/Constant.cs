using System;

namespace kERP
{
    public static class Constant
    {
        // Record status type
        public static readonly string RecordStatus_Active = "A";
        public static readonly string RecordStatus_InActive = "I";
        public static readonly string RecordStatus_Deleted = "X";

        // Record lock type
        public static string Lock_Locked = "L";
        public static string Lock_Unlock = "";

        // Session log priority
        public static readonly string Priority_Information = "I";
        public static readonly string Priority_Caution = "C";
        public static readonly string Priority_Warning = "W";
        public static readonly string Priority_Error = "E";

        // Session log type
        public static readonly string Log_Insert = "Insert";
        public static readonly string Log_Update = "Update";
        public static readonly string Log_Delete = "Delete";
        public static readonly string Log_New = "New";
        public static readonly string Log_Lock = "Lock";
        public static readonly string Log_Unlock = "Unlock";
        public static readonly string Log_Open = "Open";
        public static readonly string Log_View = "View";
        public static readonly string Log_Copy = "Copy";
        public static readonly string Log_Save = "Save";
        public static readonly string Log_SaveAndNew = "Save and New";
        public static readonly string Log_Active = "Active";
        public static readonly string Log_Inactive = "Inactive";
        public static readonly string Log_ResetPwd = "Password Reset";
        public static readonly string Log_Login = "Login";
        public static readonly string Log_Launch = "Launch";
        public static readonly string Log_NoAccess = "No Access";

        // Module
        //public static readonly string Module_IC_Location = "IC Location";
        //public static readonly string Module_Customer = "Customer";
        //public static readonly string Module_Branch = "Branch";
        //public static readonly string Module_Loan = "Loan";
        //public static readonly string Module_Product = "Product";

        // Function name
        public static readonly string Function_IC_Location = "ICLOC";   // Category
        public static readonly string Function_IC_Item = "ICITM";       // Item
        public static readonly string Function_IC_Category = "ICCAT";   // Category
        public static readonly string Function_IC_Unit_Measure = "ICUOM";   // Unit of Measure

        // Privilege    //todo: privilege codes
        public static readonly string Privilege_New = "N";  // New, Copy, Save
        public static readonly string Privilege_Update = "U";   // Save (update), Unlock
        public static readonly string Privilege_Delete = "D";
        public static readonly string Privilege_ActiveInactive = "A";
        public static readonly string Privilege_EditCode = "C";

        public static readonly string Privilege_UpdatePrice = "P";
        public static readonly string Privilege_Discount = "";
        public static readonly string Privilege_ViewCost = "";

        public static readonly string Privilege_Reverse = "R";
        public static readonly string Privilege_Print = "";
        public static readonly string Privilege_Generate = "";

        public static readonly string Splitter_Distance = "_spitter_distance";
        public static readonly string Window_State = "_window_state";
        public static readonly string Location = "_location";
        public static readonly string Size = "_size";
        public static readonly string Sql_Export = "sql_export_";

    }
}
