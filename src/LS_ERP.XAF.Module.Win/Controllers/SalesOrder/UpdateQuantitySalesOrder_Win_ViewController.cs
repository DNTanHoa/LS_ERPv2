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
    public class UpdateQuantitySalesOrder_Win_ViewController
        : UpdateQuantitySalesOrderAction_ViewController
    {
        public override void BrowserUpdateQuantitySalesOrder_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.BrowserUpdateQuantitySalesOrder_Execute(sender, e);
            var updateQuantityParam = View.CurrentObject as SalesOrderUpdateQuantityParam;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Excel Files|*.xlsx";
            fileDialog.ShowDialog();
            updateQuantityParam.File = fileDialog.FileName;
            View.Refresh();
        }
    }
}
