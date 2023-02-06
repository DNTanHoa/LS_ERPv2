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
    public class UpdateSalesOrder_Win_ViewController
        : UpdateSalesOrderAction_ViewController
    {
        public override void BrowserUpdateSalesOrder_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.BrowserUpdateSalesOrder_Execute(sender, e);
            var updateQuantityParam = View.CurrentObject as SalesOrderUpdateParam;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Excel Files|*.xlsx";
            fileDialog.ShowDialog();
            updateQuantityParam.File = fileDialog.FileName;
            View.Refresh();
        }
    }
}
