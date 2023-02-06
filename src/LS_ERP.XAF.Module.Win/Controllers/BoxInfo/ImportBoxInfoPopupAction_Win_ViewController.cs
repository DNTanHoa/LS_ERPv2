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
    public class ImportBoxInfoPopupAction_Win_ViewController : ImportBoxInfoPopupAction_ViewController
    {
        public override void BrowserImportBoxInfo_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.BrowserImportBoxInfo_Execute(sender, e);
            var importParam = View.CurrentObject as BoxInfoImportParam;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            fileDialog.ShowDialog();
            importParam.ImportFilePath = fileDialog.FileName;
            View.Refresh();
        }
    }
}
