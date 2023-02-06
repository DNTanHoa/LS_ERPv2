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
    public class ImportPurchaseOrderPopup_Win_ViewController : ImportPurchaseOrderPopupAction_ViewController
    {
        public override void BrowserImportPurchaseOrder_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.BrowserImportPurchaseOrder_Execute(sender, e);
            var importParam = View.CurrentObject as PurchaseOrderImportParam;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Excel Files|*.xlsx";
            fileDialog.ShowDialog();
            importParam.ImportFilePath = fileDialog.FileName;
            View.Refresh();
        }
    }
}
