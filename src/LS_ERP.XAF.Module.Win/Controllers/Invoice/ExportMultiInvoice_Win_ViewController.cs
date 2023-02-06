using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Controllers.DomainComponent;
using LS_ERP.XAF.Module.DomainComponent;
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
    public class ExportMultiInvoice_Win_ViewController : LoadInvoicesFromSearchParam_ViewController
    {
        public override void ExportMultiInvoice_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            base.ExportMultiInvoice_Execute(sender, e);

            var viewObject = View.CurrentObject as InvoiceSearchParam;
            ListPropertyEditor listPropertyEditor = ((DetailView)View)
               .FindItem("Invoices") as ListPropertyEditor;

            var exportInvoices = listPropertyEditor.ListView
                .SelectedObjects.Cast<Invoice>().ToList();

            SaveFileDialog dialog = new SaveFileDialog();

            var excelName = "";
            exportInvoices.ForEach(i =>
            {
                if (string.IsNullOrEmpty(excelName))
                {
                    excelName = i.Code;
                }
                else
                {
                    var len = i.Code.Trim().Length;
                    excelName += "-" + (len <= 4 ? i.Code.Trim() : i.Code.Substring(len - 4, 4));
                }
            });

            dialog.FileName = excelName; //exportInvoices.FirstOrDefault().Code.Replace('/', '-'); //export.SalesContracts.Number.Replace('/', '-');

            dialog.Filter = "Excel Files|*.xlsx;*.xls;*.xlsm|All files|*.*";

            MessageOptions options = null;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    switch (exportInvoices?.FirstOrDefault().CustomerID)
                    {
                        case "GA":
                            {
                                var stream = ExportInvoiceGA_Processor.CreateExcelFile(exportInvoices);
                                var buffer = stream as MemoryStream;
                                File.WriteAllBytes(dialog.FileName, buffer.ToArray());
                            }
                            break;
                    }



                    options = Message.GetMessageOptions("Export successfully. ", "Successs", InformationType.Success, null, 5000);
                }
                catch (Exception EE)
                {
                    options = Message.GetMessageOptions("Export failed. " + EE.Message, "Error",
                   InformationType.Error, null, 5000);

                }
                Application.ShowViewStrategy.ShowMessage(options);
                View.Refresh();
            }
        }
    }
}
