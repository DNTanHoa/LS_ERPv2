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
    public class UpdateShipQuantitySalesOrder_Win_ViewController
        : UpdateShipQuantitySalesOrderAction_ViewController
    {
        public override void BrowserUpdateShipQuantitySalesOrder_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.BrowserUpdateShipQuantitySalesOrder_Execute(sender, e);
            var updateQuantityParam = View.CurrentObject as SalesOrderUpdateShipQuantityParam;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Excel Files|*.xlsx";
            fileDialog.ShowDialog();
            updateQuantityParam.File = fileDialog.FileName;
            View.Refresh();
        }
    }
}
