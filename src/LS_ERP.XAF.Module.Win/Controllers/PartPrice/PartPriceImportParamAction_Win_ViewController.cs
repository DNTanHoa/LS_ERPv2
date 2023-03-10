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
    public class PartPriceImportParamAction_Win_ViewController : PartPriceImportAction_ViewController
    {
        public override void BrowsePartPriceImportFile_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.BrowsePartPriceImportFile_Execute(sender, e);
            var importParam = View.CurrentObject as ImportPartPriceParam;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Excel Files|*.xlsx";
            fileDialog.ShowDialog();
            importParam.FilePath = fileDialog.FileName;
            View.Refresh();
        }
    }
}
