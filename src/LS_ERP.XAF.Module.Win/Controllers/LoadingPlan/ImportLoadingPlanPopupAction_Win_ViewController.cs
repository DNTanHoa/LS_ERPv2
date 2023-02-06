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
    public class ImportLoadingPlanPopupAction_Win_ViewController : ImportLoadingPlanPopupAction_ViewController
    {
        public override void BrowserImportLoadingPlan_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.BrowserImportLoadingPlan_Execute(sender, e);
            var importParam = View.CurrentObject as LoadingPlanImportParam;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Excel Files|*.xlsx";
            fileDialog.ShowDialog();
            importParam.ImportFilePath = fileDialog.FileName;
            View.Refresh();
        }
    }
}
