using DevExpress.ExpressApp.Actions;
using LS_ERP.XAF.Module.Controllers.DomainComponent;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LS_ERP.XAF.Module.Win.Controllers
{
    public class ImportDailyTargetPopupAction_Win_ViewController : ImportDailyTargetPopupAction_ViewController
    {
        public override void BrowserImportDailyTarget_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.BrowserImportDailyTarget_Execute(sender, e);
            var importParam = View.CurrentObject as DailyTargetImportParam;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Excel Files|*.xlsx";
            fileDialog.ShowDialog();
            importParam.ImportFilePath = fileDialog.FileName;
            View.Refresh();
        }
    }
}
