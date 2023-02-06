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
    public class ExportShippingPlan_Win_ViewController : ExportShippingPlan_ViewController
    {
        public override void ExportShippingPlan_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.ExportShippingPlan_Execute(sender, e);
            var exportShippingPlan = View.CurrentObject as ShippingPlan;

            SaveFileDialog dialog = new SaveFileDialog();

            dialog.FileName = exportShippingPlan.Title; 

            dialog.Filter = "Excel Files|*.xlsx;*.xls;*.xlsm|All files|*.*";

            MessageOptions options = null;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    switch (exportShippingPlan.CustomerID)
                    {
                        case "IFG":
                            {
                                var stream = ExportShippingPlanIFG_Processor.CreateExcelFile(exportShippingPlan);
                                var buffer = stream as MemoryStream;
                                File.WriteAllBytes(dialog.FileName, buffer.ToArray());
                            }
                            break;
                        case "GA":
                            {
                                //List<ShippingPlan> ShippingPlans = new List<ShippingPlan>();
                                //ShippingPlans.Add(exportShippingPlan);
                                var stream = ExportShippingPlanGA_Processor.CreateExcelFile(exportShippingPlan);
                                var buffer = stream as MemoryStream;
                                File.WriteAllBytes(dialog.FileName, buffer.ToArray());
                            }
                            break;
                        case "DE":
                            {                                
                                var stream = ExportShippingPlanDE_Processor.CreateExcelFile(exportShippingPlan);
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
