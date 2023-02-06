using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class LoadProductionForBalance_ViewController 
        : ObjectViewController<DetailView, BalanceQuantityParam>
    {
        public LoadProductionForBalance_ViewController()
        {
            InitializeComponent();

            SimpleAction loadProductionForBalance = new SimpleAction(this, "LoadProductionForBalance", PredefinedCategory.Unspecified);
            loadProductionForBalance.ImageName = "RotateCounterclockwise";
            loadProductionForBalance.TargetObjectType = typeof(BalanceQuantityParam);
            loadProductionForBalance.TargetViewType = ViewType.DetailView;
            loadProductionForBalance.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadProductionForBalance.Caption = "Load BOM (Ctrl + L)";
            loadProductionForBalance.Shortcut = "CtrlL";

            loadProductionForBalance.Execute += LoadProductionForBalance_Execute;

            SimpleAction autoCalculateBalanceQuantity = new SimpleAction(this, "AutoCalculateBalanceQuantity", PredefinedCategory.Unspecified);
            autoCalculateBalanceQuantity.ImageName = "CalcDefault";
            autoCalculateBalanceQuantity.Caption = "Auto Balance";
            autoCalculateBalanceQuantity.TargetObjectType = typeof(BalanceQuantityParam);
            autoCalculateBalanceQuantity.TargetViewType = ViewType.DetailView;
            autoCalculateBalanceQuantity.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            autoCalculateBalanceQuantity.Execute += AutoCalculateBalanceQuantity_Execute;

            SimpleAction loadPurchaseOrderForBalance = new SimpleAction(this, "LoadPurchaseOrderForBalance", PredefinedCategory.Unspecified);
            loadPurchaseOrderForBalance.ImageName = "RotateCounterclockwise";
            loadPurchaseOrderForBalance.TargetObjectType = typeof(BalanceQuantityParam);
            loadPurchaseOrderForBalance.TargetViewType = ViewType.DetailView;
            loadPurchaseOrderForBalance.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadPurchaseOrderForBalance.Caption = "Load PO (Shift + P)";
            loadPurchaseOrderForBalance.Shortcut = "ShftP";

            loadPurchaseOrderForBalance.Execute += LoadPurchaseOrderForBalance_Execute;

            SimpleAction calculateBalanceQuantity = new SimpleAction(this, "CalculateBalanceQuantity", PredefinedCategory.Unspecified);
            calculateBalanceQuantity.ImageName = "CalcDefault";
            calculateBalanceQuantity.Caption = "Calculate Balance";
            calculateBalanceQuantity.TargetObjectType = typeof(BalanceQuantityParam);
            calculateBalanceQuantity.TargetViewType = ViewType.DetailView;
            calculateBalanceQuantity.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            calculateBalanceQuantity.Execute += CalculateBalanceQuantity_Execute;
        }

        private void AutoCalculateBalanceQuantity_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as BalanceQuantityParam;

            if(viewObject != null)
            {
                foreach (var productionBOM in viewObject.Materials)
                {
                    var purchaseOrderLine = viewObject.PurchaseForecast
                        .FirstOrDefault(x => x.ItemMasterID == productionBOM.ItemMasterID &&
                                             (x.Quantity - (x.ConsumedForecastQuantity ?? 0) > 0));

                    if(purchaseOrderLine != null)
                    {
                        var reservationEntry = ObjectSpace.CreateObject<ReservationEntry>();

                        var canBalanceQuantity = purchaseOrderLine.Quantity 
                            - (purchaseOrderLine.ConsumedForecastQuantity ?? 0);

                        if (purchaseOrderLine.ConsumedForecastQuantity == null)
                            purchaseOrderLine.ConsumedForecastQuantity = 0;

                        if (canBalanceQuantity < productionBOM.RemainQuantity)
                        {
                            productionBOM.RemainQuantity -= canBalanceQuantity ?? 0;
                            productionBOM.ReservedQuantity += canBalanceQuantity ?? 0;
                            productionBOM.Status = "Remain";

                            purchaseOrderLine.ConsumedForecastQuantity += canBalanceQuantity;
                        }
                        else
                        {
                            productionBOM.ReservedQuantity += productionBOM.RemainQuantity;
                            purchaseOrderLine.ConsumedForecastQuantity += productionBOM.RemainQuantity;

                            productionBOM.RemainQuantity = 0;
                            reservationEntry.ReservedQuantity = productionBOM.BalanceQuantity;

                            productionBOM.Status = "Full";
                        }

                        reservationEntry.ProductionBOMID = productionBOM.ProductionBOMID;
                        reservationEntry.PurchaseOrderLineID = purchaseOrderLine.ID;
                        
                        if (viewObject.ReservationEntries == null)
                            viewObject.ReservationEntries = new List<ReservationEntry>();

                        viewObject.ReservationEntries.Add(reservationEntry);
                    }
                    else
                    {
                        productionBOM.Status = "Not Found";
                    }
                }
            }
        }

        private void LoadPurchaseOrderForBalance_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as BalanceQuantityParam;

            if(viewObject != null)
            {
                var criteria = CriteriaOperator.Parse("(([PurchaseOrder.CustomerID] = ?) AND (LSStyle IS NULL OR LSStyle = ''))",
                    viewObject.Customer?.ID);
                var purchaseOrderLines = ObjectSpace.GetObjects<PurchaseOrderLine>(criteria);
                viewObject.PurchaseForecast = purchaseOrderLines.ToList();
                View.Refresh();
            }
        }

        private void CalculateBalanceQuantity_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as BalanceQuantityParam;

            if(viewObject != null)
            {
                ListPropertyEditor listPropertyEditor = 
                    ((DetailView)View).FindItem("PurchaseForecast") as ListPropertyEditor;
                var selectPurchaseOrderLines = listPropertyEditor
                    .ListView.SelectedObjects.Cast<PurchaseOrderLine>();

                foreach (var productionBOM in viewObject.Materials)
                {
                    var purchaseOrderLine = selectPurchaseOrderLines
                        .FirstOrDefault(x => x.ItemMasterID == productionBOM.ItemMasterID &&
                                             (x.Quantity - (x.ConsumedForecastQuantity ?? 0) > 0));

                    if (purchaseOrderLine != null)
                    {
                        var canBalanceQuantity = purchaseOrderLine.Quantity
                            - (purchaseOrderLine.ConsumedForecastQuantity ?? 0);
                        var reservationEntry = ObjectSpace.CreateObject<ReservationEntry>();

                        if (purchaseOrderLine.ConsumedForecastQuantity == null)
                            purchaseOrderLine.ConsumedForecastQuantity = 0;

                        if (canBalanceQuantity < productionBOM.RemainQuantity)
                        {
                            productionBOM.RemainQuantity -= canBalanceQuantity ?? 0;
                            productionBOM.ReservedQuantity += canBalanceQuantity ?? 0;
                            reservationEntry.ReservedQuantity = canBalanceQuantity;

                            productionBOM.Status = "Remain";

                            purchaseOrderLine.ConsumedForecastQuantity += canBalanceQuantity;
                        }
                        else
                        {
                            productionBOM.ReservedQuantity += productionBOM.RemainQuantity;
                            purchaseOrderLine.ConsumedForecastQuantity += productionBOM.RemainQuantity;
                            reservationEntry.ReservedQuantity = productionBOM.RemainQuantity;

                            productionBOM.RemainQuantity = 0;

                            productionBOM.Status = "Full";
                        }


                        reservationEntry.ProductionBOMID = productionBOM.ProductionBOMID;
                        reservationEntry.PurchaseOrderLineID = purchaseOrderLine.ID;

                        if(viewObject.ReservationEntries == null)
                            viewObject.ReservationEntries = new List<ReservationEntry>();

                        viewObject.ReservationEntries.Add(reservationEntry);
                    }
                    else
                    {
                        productionBOM.Status = "Not Found";
                    }
                }
            }
        }

        private void LoadProductionForBalance_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as BalanceQuantityParam;
            
            if(viewObject != null)
            {
                var productionBOMs = viewObject.ItemStyles
                    .SelectMany(x => x.ProductionBOMs);

                viewObject.Materials = productionBOMs
                    .Select(x => new MaterialBalanceQuantity()
                    {
                        ProductionBOMID = x.ID,
                        ItemID = x.ItemID,
                        ItemColorCode = x.ItemColorCode,
                        ItemColorName = x.ItemColorName,
                        CustomerStyle = x.ItemStyle?.CustomerStyle,
                        LSStyle = x.ItemStyle?.LSStyle,
                        ItemName = x.ItemName,
                        Position = x.Position,
                        Specify = x.Specify,
                        Season = x.ItemStyle?.Season,
                        GarmentColorCode = x.ItemStyle?.ColorCode,
                        GarmentColorName = x.ItemStyle?.ColorName,
                        GarmentSize = x.GarmentSize,
                        UnitID = x.PriceUnitID,
                        RequiredQuantity = x.RequiredQuantity ?? 0,
                        ReservedQuantity = x.ReservedQuantity ?? 0,
                        RemainQuantity = x.RemainQuantity ?? 0,
                        BalanceQuantity = 0,
                        Status = string.Empty,
                        VendorID = x.VendorID,
                        ItemMasterID = x.ItemMasterID,
                    }).ToList();

                View.Refresh();
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
        }
        
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
        }
        
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
