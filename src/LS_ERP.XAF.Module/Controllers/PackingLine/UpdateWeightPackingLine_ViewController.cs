using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class UpdateWeightPackingLine_ViewController : ViewController
    {
        private IObjectSpace objectSpaceNetWeight;
        public UpdateWeightPackingLine_ViewController()
        {
            InitializeComponent();

            SimpleAction updateWeightPackingLineAction = new SimpleAction(this, "UpdateWeightPackingLineAction", PredefinedCategory.Unspecified);
            updateWeightPackingLineAction.ImageName = "CalculateNow";
            updateWeightPackingLineAction.Caption = "Update Weight (Ctrl + M)";
            updateWeightPackingLineAction.TargetObjectType = typeof(PackingLine);
            updateWeightPackingLineAction.TargetViewType = ViewType.ListView;
            updateWeightPackingLineAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            updateWeightPackingLineAction.Shortcut = "CtrlM";

            updateWeightPackingLineAction.Execute += UpdateWeightPackingLineAction_Execute;
        }
        private void UpdateWeightPackingLineAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var packingList = ((DetailView)View.ObjectSpace.Owner).CurrentObject as PackingList;
            var packingLines = (View as ListView).CollectionSource.List
                    .Cast<PackingLine>().ToList().OrderBy(x => x.SequenceNo);
            objectSpaceNetWeight = Application.CreateObjectSpace(typeof(StyleNetWeight));

            if(packingList.CustomerID == "IFG")
            {
                var netWeights = objectSpaceNetWeight.GetObjects<StyleNetWeight>()
                                .Where(x => x.CustomerStyle == packingList.ItemStyles.First()?.CustomerStyle).ToList();

                if (packingLines.ToList().FirstOrDefault().PrePack.Trim() == "Assorted Size - Solid Color")
                {
                    decimal totalNetWeight = 0;
                    var newPackingLines = new List<PackingLine>();
                    var assortedPackingLines = packingLines.OrderBy(x => x.SequenceNo).OrderByDescending(x => x.TotalQuantity).ToList();
                    var totalQuantity = assortedPackingLines[0].TotalQuantity;
                    var sequenceNo = assortedPackingLines[0].SequenceNo;
                    foreach (var packingLine in assortedPackingLines)
                    {
                        if (totalQuantity != packingLine.TotalQuantity)
                        {
                            newPackingLines.ForEach(x =>
                            {
                                x.NetWeight = totalNetWeight;
                                x.GrossWeight = totalNetWeight + x.BoxDimension?.Weight * x.TotalCarton +
                                                (x.InnerBoxDimension?.Weight * x.PackagesPerBox * x.TotalCarton ?? 0);

                                sequenceNo = x.SequenceNo;
                            });

                            totalNetWeight = 0;
                        }
                        var newNetWeight = netWeights.FirstOrDefault(y => y.Size == packingLine.Size).NetWeight ?? 0;
                        totalNetWeight += (decimal)(newNetWeight * packingLine.TotalCarton * packingLine.QuantitySize);
                        newPackingLines.Add(packingLine);

                        totalQuantity = packingLine.TotalQuantity;
                    }
                    newPackingLines.ForEach(x =>
                    {
                        if (string.Compare(x.SequenceNo, sequenceNo) > 0 || sequenceNo == "000")
                        {
                            x.NetWeight = totalNetWeight;
                            x.GrossWeight = totalNetWeight + x.BoxDimension?.Weight * x.TotalCarton +
                                            (x.InnerBoxDimension?.Weight * x.PackagesPerBox * x.TotalCarton ?? 0);
                        }
                    });
                    packingList.PackingLines = newPackingLines;
                }
                else
                {
                    packingLines.ToList().ForEach(x => {
                        var newNetWeight = netWeights.FirstOrDefault(y => y.Size == x.Size).NetWeight ?? 0;
                        x.NetWeight = (decimal)(x.TotalQuantity * newNetWeight);
                        x.GrossWeight = (decimal)(x?.BoxDimension?.Weight ?? 0 * x.TotalCarton + x.TotalQuantity * newNetWeight);
                    });
                }
            }
            else if (packingList.CustomerID == "GA")
            {
                var criteria = CriteriaOperator.Parse("[CustomerStyle] " +
                                "IN (" + string.Join(",", packingList.ItemStyles.Select(x => "'" + x.CustomerStyle + "'")) + ")");
                var netWeights = objectSpaceNetWeight.GetObjects<StyleNetWeight>(criteria).ToList();
                
                if (packingLines.ToList().FirstOrDefault().PrePack.Trim() == "Assorted Size - Solid Color")
                {
                    
                    var newPackingLines = new List<PackingLine>();

                    foreach (var data in packingList.ItemStyles)
                    {
                        var assortedPackingLines = packingLines.Where(x => x.LSStyle == data.LSStyle).OrderBy(x => x.SequenceNo).ToList();
                        decimal netWeight = 0;
                        foreach (var packingLine in assortedPackingLines)
                        {
                            var newNetWeight = netWeights.FirstOrDefault(y => y.Size == packingLine.Size &&
                                                                         y.CustomerStyle == data.CustomerStyle).NetWeight ?? 0;
                            netWeight += (decimal)(newNetWeight * packingLine.TotalCarton * packingLine.QuantitySize);


                        }
                        foreach (var packingLine in assortedPackingLines)
                        {
                            packingLine.NetWeight = netWeight;
                            packingLine.GrossWeight = netWeight + (packingLine?.BoxDimension?.Weight ?? 0) * packingLine.TotalCarton;
                        }
                    }
                }
                else
                {
                    foreach(var data in packingList.ItemStyles)
                    {
                        packingLines.Where(x => x.LSStyle == data.LSStyle).ToList().ForEach(x => {
                            var newNetWeight = netWeights.FirstOrDefault(y => y.Size == x.Size &&
                                                                         y.CustomerStyle == data.CustomerStyle).NetWeight ?? 0;
                            x.NetWeight = (decimal)(x.TotalQuantity * newNetWeight);
                            x.GrossWeight = (decimal)(x?.BoxDimension?.Weight * x.TotalCarton + x.TotalQuantity * newNetWeight);
                        });
                    }
                }
            }
            else if(packingList.CustomerID == "DE")
            {
                var criteria = CriteriaOperator.Parse("[CustomerStyle] " +
                                "IN (" + string.Join(",", packingList.ItemStyles.Select(x => "'" + x.CustomerStyle + "'")) + ")");
                var netWeights = objectSpaceNetWeight.GetObjects<StyleNetWeight>(criteria).ToList();

                foreach (var data in packingList.ItemStyles)
                {
                    packingLines.Where(x => x.LSStyle == data.LSStyle).ToList().ForEach(x => {
                        var newNetWeight = netWeights.FirstOrDefault(y => y.Size == x.Size &&
                                                                     y.CustomerStyle == data.CustomerStyle);
                        
                        var boxWeight = newNetWeight?.BoxWeight ?? x?.BoxDimension?.Weight ?? 0;
                        x.NetWeight = (decimal)(x.TotalQuantity * newNetWeight?.NetWeight ?? 0);
                        x.GrossWeight = (decimal)(boxWeight * x.TotalCarton + x.TotalQuantity * newNetWeight?.NetWeight ?? 0);
                    });
                }
            }

            View.Refresh();
            ((DetailView)View.ObjectSpace.Owner).Refresh();
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



