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
    public class ImportSalesContractPopupAction_Win_ViewController : ImportSalesContractPopupAction_ViewController
    {
        public override void BrowserImportSalesContract_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.BrowserImportSalesContract_Execute(sender, e);
            var importParam = View.CurrentObject as SalesContractImportParam;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Excel Files|*.xlsx";
            fileDialog.ShowDialog();
            importParam.ImportFilePath = fileDialog.FileName;

            View.Refresh();
        }
    }
}
