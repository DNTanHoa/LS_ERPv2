using DevExpress.ExpressApp.Actions;
using LS_ERP.XAF.Module.Controllers;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LS_ERP.XAF.Module.Win.Controllers
{
    public class ImportPartRevision_Win_ViewController
        : ImportPartRevisionAction_ViewController
    {
        public override void BrowserImportPartRevision_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.BrowserImportPartRevision_Execute(sender, e);
            var importParam = View.CurrentObject as ImportPartRevisionParam;
            string serverName = ConfigurationManager.AppSettings.Get("ServerName").ToString();
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm|All files|*.*";
            fileDialog.ShowDialog();
            importParam.FilePath = fileDialog.FileName;
            importParam.FileName = fileDialog.FileName;
            importParam.FileNameServer = serverName + fileDialog.FileName;
            View.Refresh();
        }
    }
}
