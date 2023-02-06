using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class FabricStorageParamAction_ViewController : ObjectViewController<ListView, IssuedFabric>
    {
        public FabricStorageParamAction_ViewController()
        {
            InitializeComponent();
            PopupWindowShowAction calculateFabricPurchaseInforData = new PopupWindowShowAction(this, "CalculateIssuedFabricStorage", PredefinedCategory.Unspecified);
            calculateFabricPurchaseInforData.ImageName = "Actions_Reload";
            calculateFabricPurchaseInforData.Caption = "Calculate Quantity";
            calculateFabricPurchaseInforData.TargetObjectType = typeof(IssuedFabric);
            calculateFabricPurchaseInforData.TargetViewType = ViewType.ListView;
            calculateFabricPurchaseInforData.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            calculateFabricPurchaseInforData.CustomizePopupWindowParams += CalculateFabricPurchaseInforData_CustomizePopupWindowParams;
            calculateFabricPurchaseInforData.Execute += CalculateFabricPurchaseInforData_Execute;
        }

        private void CalculateFabricPurchaseInforData_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            //var viewObject = View.CurrentObject as FabricPopupModel;
            var viewObject = e.PopupWindowView.CurrentObject as CalculateFabricPopupModel;
            var selected = e.CurrentObject as IssuedFabric;
            var property = ((View as ListView).CollectionSource) as PropertyCollectionSource;
            var fbPopupModel = property.MasterObject as IssuedFabricParam;

            decimal? remainQty = 0;
            decimal? remainRoll = 0;
            foreach (var itemFB in fbPopupModel.Fabrics)
            {
                if (itemFB.StorageDetailID != selected.StorageDetailID)
                {
                    if (itemFB.InStockQuantity != null && itemFB.InStockQuantity > 0)
                    {
                        itemFB.IssuedQuantity = itemFB.InStockQuantity;
                        remainQty += itemFB.IssuedQuantity;
                    }

                    if (itemFB.RollInStock != null && itemFB.RollInStock > 0)
                    {
                        itemFB.Roll = itemFB.RollInStock;
                        remainRoll += itemFB.Roll;
                    }
                }

            }

            if (selected != null)
            {
                remainQty = viewObject.RequiredQuantity - remainQty;
                remainRoll = viewObject.RequiredRoll - remainRoll;

                if (remainQty > 0 && selected.InStockQuantity != null && selected.InStockQuantity > 0)
                {
                    selected.IssuedQuantity = remainQty;

                }

                if (remainRoll > 0 && selected.RollInStock != null && selected.RollInStock > 0)
                {
                    selected.Roll = remainRoll;
                }
            }

            ObjectSpace.CommitChanges();
            property.Reload();
            View.Refresh();
        }

        private void CalculateFabricPurchaseInforData_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new CalculateFabricPopupModel();
            model.RequiredQuantity = 0;
            model.RequiredRoll = 0;
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.Maximized = false;
            e.View = view;
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
