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
    public class ImportItemPricePopupAction_Win_ViewController
        : ImportItemPricePopupAction_ViewController
    {
        public override void BrowserImportItemPrice_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.BrowserImportItemPrice_Execute(sender, e);
            var importParam = View.CurrentObject as ItemPriceImportParam;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm|All files|*.*";
            fileDialog.ShowDialog();
            importParam.FilePath = fileDialog.FileName;
            View.Refresh();
        }
    }
}
