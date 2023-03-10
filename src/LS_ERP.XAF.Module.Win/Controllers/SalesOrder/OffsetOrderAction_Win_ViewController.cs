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
    public class OffsetOrderAction_Win_ViewController
        : OffetOrderAction_ViewController
    {
        public override void BrowserOffsetSalesOrder_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.BrowserOffsetSalesOrder_Execute(sender, e);
            var importParam = View.CurrentObject as SalesOrderOffsetParam;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Excel Files|*.xlsx";
            fileDialog.ShowDialog();
            importParam.FilePath = fileDialog.FileName;
            View.Refresh();
        }
    }
}
