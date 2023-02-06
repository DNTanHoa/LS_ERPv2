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
    public class UpdateSampleQuantity_Win_ViewController
        : UpdateSampleQuantityAction_ViewController
    {
        public override void BrowserUpdateSampleQuantitySalesOrder_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.BrowserUpdateSampleQuantitySalesOrder_Execute(sender, e);
            var updateQuantityParam = View.CurrentObject as SalesOrderUpdateSampleQuantityParam;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Excel Files|*.xlsx";
            fileDialog.ShowDialog();
            updateQuantityParam.File = fileDialog.FileName;
            View.Refresh();
        }
    }
}
