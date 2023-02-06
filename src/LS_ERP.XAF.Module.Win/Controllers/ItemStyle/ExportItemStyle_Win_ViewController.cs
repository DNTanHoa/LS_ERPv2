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
    public class ExportItemStyle_Win_ViewController : ExportItemStyle_ViewController
    {
        public override void ExportItemStyle_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.ExportItemStyle_Execute(sender, e);
            var itemStyle = View.CurrentObject as ItemStyle;
            var saleOrder = itemStyle.SalesOrder;
            var itemList = e.SelectedObjects.Cast<ItemStyle>().ToList();

            SaveFileDialog dialog = new SaveFileDialog();

            dialog.FileName = saleOrder.ID.Replace('/', '-'); //export.SalesContracts.Number.Replace('/', '-');

            dialog.Filter = "Excel Files|*.xlsx;*.xls;*.xlsm|All files|*.*";

            MessageOptions options = null;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    switch (saleOrder.CustomerID)
                    {
                        case "HA":
                            {
                                var stream = SalesOrderProcess.CreateExcelFile(saleOrder, itemList);
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
