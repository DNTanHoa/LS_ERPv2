using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Controllers;
using LS_ERP.XAF.Module.Process;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Message = LS_ERP.XAF.Module.Helpers.Message;

namespace LS_ERP.XAF.Module.Win.Controllers
{
    public class ExportPartRevision_Win_ViewController
        : ExportPartRevision_ViewController
    {
        public override void ExportPartRevision_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.ExportPartRevision_Execute(sender, e);
            var export = View.CurrentObject as PartRevision;


            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = export.PartNumber.Replace('/', '-') + "-" + 
                export.Season + "-" + export.RevisionNumber;

            dialog.Filter = "Excel Files|*.xlsx;*.xls;*.xlsm|All files|*.*";

            MessageOptions options = null;

            try
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Stream stream = null;
                    switch (export.CustomerID)
                    {
                        case "DE":
                            stream = PartRevisionProcess.CreateExcelFileDE(export);
                            break;
                        default:
                            stream = PartRevisionProcess.CreateExcelFileDefault(export);
                            break;
                    }
                    var buffer = stream as MemoryStream;
                    File.WriteAllBytes(dialog.FileName, buffer.ToArray());
                }
                options = Message.GetMessageOptions("Export successfully. ", "Successs", 
                    InformationType.Success, null, 5000);
            }
            catch (Exception ex)
            {
                options = Message.GetMessageOptions("Export failed. " + ex.Message, "Error",
                   InformationType.Error, null, 5000);
            }

            Application.ShowViewStrategy.ShowMessage(options);
            View.Refresh();
        }
    }
}
