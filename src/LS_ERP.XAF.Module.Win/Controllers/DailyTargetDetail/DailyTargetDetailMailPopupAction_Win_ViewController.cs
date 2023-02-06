using DevExpress.ExpressApp.Actions;
using LS_ERP.XAF.Module.Controllers.DomainComponent;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Windows.Forms;
using System.Drawing;

namespace LS_ERP.XAF.Module.Win.Controllers
{
    public class DailyTargetDetailMailPopupAction_Win_ViewController : DailyTargetDetailMailPopupAction_ViewController
    {
        public override void BrowserDailyTargetDetailAttachFile_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.BrowserDailyTargetDetailAttachFile_Execute(sender, e);
            var importParam = View.CurrentObject as DailyTargetDetailMailParam;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Excel Files|*.xlsx";
            fileDialog.ShowDialog();
            importParam.ImportFilePath = fileDialog.FileName;
            View.Refresh();
        }
        
    }
}

