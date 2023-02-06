using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.Ultilities.Helpers;
using LS_ERP.XAF.Module.Controllers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Message = LS_ERP.XAF.Module.Helpers.Message;

namespace LS_ERP.XAF.Module.Win.Controllers
{
    public class DownloadDocument_Win_ViewController : DownloadDocument_ViewController
    {
        public override void DownloadDocument_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.DownloadDocument_Execute(sender, e);

            var document = e.CurrentObject as InvoiceDocument;
            string serverName = ConfigurationManager.AppSettings.Get("ServerName").ToString();
            string path = serverName + document.FileName;
            var data = SaveFileHelpers.Dowload(path);

            SaveFileDialog dialog = new SaveFileDialog();

            dialog.FileName = document.FileName.Replace('/', '-'); //export.SalesContracts.Number.Replace('/', '-');
            //dialog.FileName = "To_confirm (14)20220624140352168.xlsx"; //export.SalesContracts.Number.Replace('/', '-');

            dialog.Filter = "All files|*.*";

            MessageOptions options = null;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllBytes(dialog.FileName, data);

                    options = Message.GetMessageOptions("Download successfully. ", "Successs", InformationType.Success, null, 5000);
                }
                catch (Exception EE)
                {
                    options = Message.GetMessageOptions("Download failed. " + EE.Message, "Error",
                   InformationType.Error, null, 5000);

                }
                Application.ShowViewStrategy.ShowMessage(options);
                View.Refresh();
            }

        }
    }
}
