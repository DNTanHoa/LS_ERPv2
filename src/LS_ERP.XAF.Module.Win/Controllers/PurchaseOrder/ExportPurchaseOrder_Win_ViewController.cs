using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Controllers;
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
    public class ExportPurchaseOrder_Win_ViewController : ExportPurchaseOrder_ViewController
    {
        public override void ExportPurchaseOrderAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.ExportPurchaseOrderAction_Execute(sender, e);

            var viewObject = View.CurrentObject as PurchaseReceivedSearchParam;

            var purchaseOrder = View.CurrentObject as PurchaseReceivedDetails;
            var purchaseOrders = e.SelectedObjects.Cast<PurchaseReceivedDetails>().ToList();

            SaveFileDialog dialog = new SaveFileDialog();

            dialog.FileName = purchaseOrder.CustomerPurchaseOrderNumber.Replace('/', '-'); //export.SalesContracts.Number.Replace('/', '-');

            dialog.Filter = "Excel Files|*.xlsx;*.xls;*.xlsm|All files|*.*";

            MessageOptions options = null;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    switch (purchaseOrder.CustomerID)
                    {
                        case "PU":
                            {
                                var stream = ExportPurchaseOrderProcessor.CreateExcelFile(purchaseOrders);
                                var buffer = stream as MemoryStream;
                                File.WriteAllBytes(dialog.FileName, buffer.ToArray());

                                //var streamIssued = ExportPurchaseOrderProcessor.CreateDocumentIssued_ExcelFile(purchaseOrders);
                                //var bufferIssued = streamIssued as MemoryStream;
                                //File.WriteAllBytes("Document Issued", bufferIssued.ToArray());
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
