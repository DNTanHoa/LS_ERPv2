using DevExpress.ExpressApp.Win.Utils;
using DevExpress.XtraSplashScreen;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LS_ERP.Forms
{
    public partial class AppSplashScreen : SplashScreen
    {
        public AppSplashScreen()
        {
            InitializeComponent();
            this.lblCopyright.Text = "Copyright LS © 2020-" + DateTime.Now.Year.ToString();
        }

        #region Overrides

        public override void ProcessCommand(Enum cmd, object arg)
        {
            base.ProcessCommand(cmd, arg);
            if ((UpdateSplashCommand)cmd == UpdateSplashCommand.Description)
            {
                labelCommand.Text = (string)arg;
            }
        }

        #endregion

        public enum SplashScreenCommand
        {
        }

    }
}