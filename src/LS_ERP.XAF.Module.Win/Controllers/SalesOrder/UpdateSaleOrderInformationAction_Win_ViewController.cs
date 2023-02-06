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
    public class UpdateSaleOrderInformationAction_Win_ViewController 
        : UpdateSaleOrderInformationAction_ViewController
    {
        public override void BrowserSalesOrderUpdateInformationFile_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.BrowserSalesOrderUpdateInformationFile_Execute(sender, e);
            var saleOrderUpdateInformationParam = View.CurrentObject as SalesOrderUpdateInformationParam;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Excel Files|*.xlsx";
            fileDialog.ShowDialog();
            saleOrderUpdateInformationParam.FilePath = fileDialog.FileName;
            View.Refresh();
        }
    }
}
