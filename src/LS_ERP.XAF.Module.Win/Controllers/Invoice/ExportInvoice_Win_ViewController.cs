using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Controllers.DomainComponent;
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
    public class ExportInvoice_Win_ViewController : ExportInvoice_ViewController
    {
        public override void ExportInvoice_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            base.ExportInvoice_Execute(sender, e);
            var exportInvoice = View.CurrentObject as Invoice;

            SaveFileDialog dialog = new SaveFileDialog();

            dialog.FileName = exportInvoice.Code.Replace('/', '-'); //export.SalesContracts.Number.Replace('/', '-');

            dialog.Filter = "Excel Files|*.xlsx;*.xls;*.xlsm|All files|*.*";

            MessageOptions options = null;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    switch (exportInvoice.CustomerID)
                    {
                        case "IFG":
                            {
                                var units = ObjectSpace.GetObjects<Unit>().ToList();
                                var sizes = ObjectSpace.GetObjects<Size>().Where( x=> x.CustomerID == "IFG").ToList();

                                var stream = ExportInvoiceIFG_Processor.CreateExcelFile(exportInvoice, units, sizes);
                                var buffer = stream as MemoryStream;
                                File.WriteAllBytes(dialog.FileName, buffer.ToArray());
                            }
                            break;
                        case "GA":
                            {
                                List<Invoice> invoices = new List<Invoice>();
                                invoices.Add(exportInvoice);
                                var stream = ExportInvoiceGA_Processor.CreateExcelFile(invoices);
                                var buffer = stream as MemoryStream;
                                File.WriteAllBytes(dialog.FileName, buffer.ToArray());
                            }
                            break;
                        case "DE":
                            {
                                //var partPrices = objectSpace.GetObjects<PartPrice>().Where(p=>p.CustomerID == exportInvoice.CustomerID).ToList() ;

                                List<Invoice> invoices = new List<Invoice>();
                                invoices.Add(exportInvoice);
                                var stream = ExportInvoiceDE_Processor.CreateExcelFile(invoices);
                                var buffer = stream as MemoryStream;
                                File.WriteAllBytes(dialog.FileName.Replace(".xlsx", "-INV & PKL.xlsx"), buffer.ToArray());
                                //
                                var productionDept = objectSpace.GetObjects<ProductionDept>().ToList();
                                //
                                var stream1 = ExportInvoiceDE_Processor.CreatePaymentExcelFile(invoices,productionDept);
                                var buffer1 = stream1 as MemoryStream;
                                File.WriteAllBytes(dialog.FileName.Replace(".xlsx","-PAYMENT.xlsx"), buffer1.ToArray());
                            }
                            break;
                        case "PU":
                            {
                                List<Invoice> invoices = new List<Invoice>();
                                invoices.Add(exportInvoice);

                                var stream = ExportInvoicePU_Processor.CreateExcelFile(invoices);
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
