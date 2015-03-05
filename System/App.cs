using System;
using System.IO;
using System.Windows.Forms;
using System.Data;
using Npgsql;
using Dapper;
using System.Reflection;
using kERP.SM;
using System.Diagnostics;

namespace kERP
{
    public static class App
    {
        public static string StartupPath;

        public static frmSplash fSplash = null;

        public static Setting setting = new Setting();
        public static String version;
        public static Session session = new Session();

        //public static frmCompany fCompany;
        public static frmProduct fProduct;
        public static frmHoliday fHoliday;
        public static frmBranch fBranch;
        public static frmCustomer fCustomer;
        public static frmLoan fLoan;

        public static SM.frmUserList fUserList;
        public static SM.frmAuditLog fAuditLog;

        public static FileLog AccessLog = new FileLog();

        public static string ProcessID = System.Diagnostics.Process.GetCurrentProcess().Id.ToString();

        public static bool Init()
        {
            StartupPath = Application.StartupPath;
            if (!StartupPath.EndsWith("\\")) StartupPath += "\\";

            SetVersion();

            // Splash screen
            fSplash = new frmSplash();
            fSplash.SetAppName(" v " + App.version);
            fSplash.Show();
            fSplash.ShowMsg("Initializing the application...");
            fSplash.TopMost = true;
            // Database connection string from setting.ini
            try
            {
                fSplash.ShowMsg("Loading settings...");
                LoadSettings();

                fSplash.ShowMsg("Connecting to the database ...");
                SqlFacade.OpenConnection(); // Database connection: open/test

                fSplash.ShowMsg("Ensuring database setup ...");
                //SqlFacade.EnsureDBSetup();  // Create tables if not exist
            }
            catch (Exception ex)
            {
                fSplash.ShowError(ex.Message);
                fSplash.Visible = false;
                fSplash.ShowDialog();
                ErrorLogFacade.LogToFile(ex);
                return false;
            }

            //Session            
            session.Username = "Visal"; //todo: logged in user (to be removed)
            session.Branch_Code = "032";
            session.Machine_Name = Environment.MachineName;
            session.Machine_User_Name = Environment.UserName;
            session.Version = version;

            fSplash.ShowMsg("Loading configurations...");
            ConfigFacade.Load();
            LabelFacade.Load();
            MessageFacade.Load();            
            // Log
            ErrorLogFacade.logFile.FileName = Path.Combine(App.StartupPath, "Error.log");
            AccessLog.FileName = Path.Combine(App.StartupPath, "Access.log");
            AccessLog.Write(DateTime.Now.ToString("yyy-MM-dd ddd hh:mm:ss tt") + " Application started. Process Id: " + ProcessID + ", Machine: " + session.Machine_Name + ", machine's username: " + session.Machine_User_Name + ", version: " + session.Version);
            session.Id = SessionFacade.Save(session);
            SessionLogFacade.Log(Constant.Priority_Information, "Application", Constant.Log_Launch, "Application started");
            return true;
        }

        private static void SetVersion()
        {
            version = Util.RemoveLastDotZero(Application.ProductVersion);
        }

        private static void LoadSettings()
        {
            setting.Path = Path.Combine(App.StartupPath, "setting.ini");
            try
            {
                SqlFacade.ConnectionString = setting.Get("ConnectionString", @"server=localhost;uid=erp;pwd=erp");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void Close()
        {
            App.AccessLog.Write(DateTime.Now.ToString("yyy-MM-dd ddd hh:mm:ss tt") + " Application quit. Process Id: " + App.ProcessID);
            SM.SessionFacade.UpdateLogout(App.session);

            setting.Save();
            ConfigFacade.Save();
        }
    }
}