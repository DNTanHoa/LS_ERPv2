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
    public class UpdateCAC_CHD_EHDSalesOrder_Win_ViewController
        : UpdateCAC_CHD_EHDSalesOrderAction_ViewController
    {
        public override void BrowserUpdateCAC_CHD_EHDSalesOrder_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.BrowserUpdateCAC_CHD_EHDSalesOrder_Execute(sender, e);
            var updateQuantityParam = View.CurrentObject as SalesOrderUpdateCAC_CHD_EHDParam;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Excel Files|*.xlsx";
            fileDialog.ShowDialog();
            updateQuantityParam.File = fileDialog.FileName;
            View.Refresh();
        }
    }
}
