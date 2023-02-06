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
    public class ImportUPCPopupAction_Win_ViewController : ImportUPCPopupAction_ViewController
    {
        public override void BrowserImportUPC_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.BrowserImportUPC_Execute(sender, e);
            var importParam = View.CurrentObject as UPCImportParam;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm|All files|*.*";
            fileDialog.ShowDialog();
            importParam.FilePath = fileDialog.FileName;
            View.Refresh();
        }
    }
}
