using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class CalculateNetGrossWeightPackingLine_ViewController 
        : ObjectViewController<ListView, PackingLine>
    {
        public CalculateNetGrossWeightPackingLine_ViewController()
        {
            InitializeComponent();
            SimpleAction exchangePackingLineAction = new SimpleAction(this, "CalculateNetGrossWeightPackingLineAction", PredefinedCategory.Unspecified);
            exchangePackingLineAction.ImageName = "CalculateNow";
            exchangePackingLineAction.Caption = "Calculate Weight (Ctrl + W)";
            exchangePackingLineAction.TargetObjectType = typeof(PackingLine);
            exchangePackingLineAction.TargetViewType = ViewType.ListView;
            exchangePackingLineAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            exchangePackingLineAction.Shortcut = "CtrlW";

            exchangePackingLineAction.Execute += CalculateNetGrossWeightPackingLineAction_Execute;
        }

        private void CalculateNetGrossWeightPackingLineAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var selectedRows = View.SelectedObjects.Cast<PackingLine>();
            MessageOptions options = null;

            if (selectedRows.Count() >= 2)
            {
                decimal netWeight = 0;
                decimal grossWeight = 0;
                decimal quantityPerCarton = 0;
                decimal totalQuantity = 0;

                var packingLines = (View as ListView).CollectionSource.List
                    .Cast<PackingLine>().ToList().OrderBy(x => x.SequenceNo);

                Dictionary<int, PackingLine> dicPackingLineSelected = selectedRows.ToDictionary(x => x.ID);

                var LSStyle = string.Empty;
                var LSStyles = packingLines.First().PackingList.LSStyles.Split(";");

                foreach (var packingLine in selectedRows)
                {
                    if (packingLine.LSStyle != LSStyle || LSStyles.Count() == 1)
                    {
                        netWeight += (decimal)packingLine.NetWeight;
                        quantityPerCarton += (decimal)packingLine.QuantityPerCarton;
                        totalQuantity += (decimal)packingLine.TotalQuantity;
                        //grossWeight += (decimal)netWeight;
                        LSStyle = packingLine.LSStyle;
                    }
                }
                grossWeight += (decimal)packingLines.First().BoxDimension.Weight + netWeight;

                View.Refresh();
                (View as ListView).EditView?.Refresh();

                foreach (PackingLine line in packingLines)
                {
                    if (dicPackingLineSelected.TryGetValue(line.ID, out PackingLine rs))
                    {
                        line.NetWeight = netWeight;
                        line.GrossWeight = grossWeight;
                        line.QuantityPerCarton = quantityPerCarton;
                        line.TotalQuantity = totalQuantity;
                        View.Refresh();
                        (View as ListView).EditView?.Refresh();
                    }
                }

                View.Refresh();
                (View as ListView).EditView?.Refresh();
            }
            else
            {
                options = Message.GetMessageOptions("Please select more 2 rows for exchange", "Error",
                    InformationType.Error, null, 5000);
                Application.ShowViewStrategy.ShowMessage(options);
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
