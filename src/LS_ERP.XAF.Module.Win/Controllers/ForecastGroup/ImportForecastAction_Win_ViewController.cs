using DevExpress.ExpressApp.Actions;
using LS_ERP.XAF.Module.Controllers;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LS_ERP.XAF.Module.Win.Controllers
{
    public class ImportForecastAction_Win_ViewController : ImportForecastAction_ViewController
    {
        public override void BrowserImportForecastGroup_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.BrowserImportForecastGroup_Execute(sender, e);

            var importParam = View.CurrentObject as ForecastGroupImportParam;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm|All files|*.*";
            fileDialog.ShowDialog();
            importParam.FilePath = fileDialog.FileName;
            View.Refresh();
        }
    }
}
