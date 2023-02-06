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
    public class ImportFabricPopupAction_Win_ViewController : ImportFabricPopupAction_ViewController
    {
        public override void BrowserImportFabricPurchaseOrder_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.BrowserImportFabricPurchaseOrder_Execute(sender, e);
            var importParam = View.CurrentObject as FabricPurchaseOrderImportParam;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Excel Files|*.xlsx";
            fileDialog.ShowDialog();
            importParam.ImportFilePath = fileDialog.FileName;
            View.Refresh();
        }
    }
}
