using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Controllers;
using LS_ERP.XAF.Module.Controllers.DomainComponent;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Process;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ultils.Extensions;
using Ultils.Helpers;
using Message = LS_ERP.XAF.Module.Helpers.Message;


namespace LS_ERP.XAF.Module.Win.Controllers
{
    public class ExportOnhandTransactionByLocation_Win_ViewController : ExportOnhandTransactionByLocation_ViewController
    {
        public override void ExportOnhandByLocation_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            {
                var objectSpace = this.ObjectSpace;
                var viewObject = View.CurrentObject as StorageDetailReportParam;
                ListPropertyEditor listPropertyEditor = ((DetailView)e.PopupWindowView)
                    .FindItem("StorageBinCodes") as ListPropertyEditor;
                var storageBinCodeModels = listPropertyEditor.ListView?.SelectedObjects.Cast<StorageBinCodeModel>().ToList();

                var connectString = Application.ConnectionString ?? string.Empty;
                var customerID = viewObject?.Customer?.ID ?? string.Empty;
                var storageCode = viewObject?.Storage?.Code ?? string.Empty;
                List<StorageDetailsReport> storageDetailsReports = new List<StorageDetailsReport>();
                List<MaterialTransaction> transactionList = new List<MaterialTransaction>();
                using (var db = new QueryFactory(
                    new SqlConnection(connectString), new SqlServerCompiler()))
                {
                    var tableName = "ViewStorageDetailReport";

                    var query = db.Query(tableName)
                        .Where(new
                        {
                            CustomerID = customerID,
                            StorageCode = storageCode
                        })
                        .Where("OnHandQuantity", ">", 0)
                        .WhereIn("StorageBinCode", storageBinCodeModels.Select(s => s.StorageBinCode).ToList());
                     storageDetailsReports = query.Get<StorageDetailsReport>().ToList();

                    //
                    tableName = "MaterialTransaction";
                    var query1 = db.Query(tableName)
                        //.WhereRaw("IssuedNumber is not null")                        
                        .WhereIn("StorageDetailID", storageDetailsReports.Select(s=>s.ID).ToList());
                    transactionList = query1.Get<MaterialTransaction>().ToList();

                }
                if (storageDetailsReports.Count == 0)
                    return;
                //
                
                // export
                SaveFileDialog dialog = new SaveFileDialog();

                dialog.FileName = "Onhand_"+storageCode+"_"+DateTime.Now.ToString("dd-MM-yyyy");

                dialog.Filter = "Excel Files|*.xlsx;*.xls;*.xlsm|All files|*.*";

                MessageOptions options = null;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        switch (storageCode)
                        {
                            case "FB":
                                {
                                    var stream = ExportOnHandByLocationFB_Processor.CreateExcelFile(storageDetailsReports, transactionList);
                                    var buffer = stream as MemoryStream;
                                    File.WriteAllBytes(dialog.FileName, buffer.ToArray());
                                }
                                break;
                            case "AC":
                                {
                                   
                                }
                                break;
                            case "FG":
                                {
                                    
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


                //View.Refresh();
            }
        }
    }
}
