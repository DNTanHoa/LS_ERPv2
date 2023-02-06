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
    public class DocumentUpload_Win_ViewController : UploadDocumentAction_ViewController
    {
        public override void BrowserUploadDocumentInvoice_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.BrowserUploadDocumentInvoice_Execute(sender, e);

            var importParam = View.CurrentObject as InvoiceDocumentParam;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Excel Files|*.xlsx";
            fileDialog.ShowDialog();
            importParam.ImportFilePath = fileDialog.FileName;
            View.Refresh();
        }
    }
}
