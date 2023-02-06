using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
using System.Collections.Generic;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Win.Utils;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.ClientServer;
using Microsoft.EntityFrameworkCore;
using DevExpress.ExpressApp.EFCore;
using DevExpress.EntityFrameworkCore.Security;
using LS_ERP.XAF.Module;
using LS_ERP.XAF.Module.BusinessObjects;
using System.Data.Common;
using System.Reflection;
using System.IO;
using System.Drawing;
using LS_ERP.Forms;
using DevExpress.XtraEditors;

namespace LS_ERP.XAF.Win
{
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Win.WinApplication._members
    public partial class XAFWindowsFormsApplication : WinApplication
    {
        public XAFWindowsFormsApplication()
        {
            InitializeComponent();
            WindowsFormsSettings.SetDPIAware();
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            Stream splashImageStream = executingAssembly.GetManifestResourceStream(
                executingAssembly.GetName().Name + "Images.logosoftware720x450.png");
            SplashScreen = new DXSplashScreen(typeof(AppSplashScreen));
        }
        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args)
        {
            var efCoreObjectSpaceProvider = new SecuredEFCoreObjectSpaceProvider((ISelectDataSecurityProvider)Security, typeof(XAFEFCoreDbContext), TypesInfo, args.ConnectionString,
                (builder, connectionString) =>
                {
                    // Uncomment this code to use an in-memory database. This database is recreated each time the server starts. With the in-memory database, you don't need to make a migration when the data model is changed.
                    // Do not use this code in production environment to avoid data loss.
                    // We recommend that you refer to the following help topic before you use an in-memory database: https://docs.microsoft.com/en-us/ef/core/testing/in-memory
                    //builder.UseInMemoryDatabase("InMemory");
                    builder.UseSqlServer(connectionString);
                });

            args.ObjectSpaceProviders.Add(efCoreObjectSpaceProvider);
            args.ObjectSpaceProviders.Add(new NonPersistentObjectSpaceProvider(TypesInfo, null));
        }
        private void XAFWindowsFormsApplication_CustomizeLanguagesList(object sender, CustomizeLanguagesListEventArgs e)
        {
            string userLanguageName = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            if (userLanguageName != "en-US" && e.Languages.IndexOf(userLanguageName) == -1)
            {
                e.Languages.Add(userLanguageName);
            }
        }
        private void XAFWindowsFormsApplication_DatabaseVersionMismatch(object sender, DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs e)
        {
#if EASYTEST
            e.Updater.Update();
            e.Handled = true;
#else
            if (System.Diagnostics.Debugger.IsAttached)
            {
                e.Updater.Update();
                e.Handled = true;
            }
            else
            {
                string message = "The application cannot connect to the specified database, " +
                    "because the database doesn't exist, its version is older " +
                    "than that of the application or its schema does not match " +
                    "the ORM data model structure. To avoid this error, use one " +
                    "of the solutions from the https://www.devexpress.com/kb=T367835 KB Article.";

                if (e.CompatibilityError != null && e.CompatibilityError.Exception != null)
                {
                    message += "\r\n\r\nInner exception: " + e.CompatibilityError.Exception.Message;
                }
                throw new InvalidOperationException(message);
            }
#endif
        }
    }
}
