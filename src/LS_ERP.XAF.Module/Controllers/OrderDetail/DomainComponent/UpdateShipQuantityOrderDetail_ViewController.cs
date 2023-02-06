using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class UpdateShipQuantityOrderDetail_ViewController : ObjectViewController<ListView, OrderDetailForPacking>
    {
        public UpdateShipQuantityOrderDetail_ViewController()
        {
            InitializeComponent();

            SimpleAction updateShipQuantityOrderDetailAction = new SimpleAction(this, "UpdateShipQuantityOrderDetailAction", PredefinedCategory.Unspecified);
            updateShipQuantityOrderDetailAction.ImageName = "Update";
            updateShipQuantityOrderDetailAction.Caption = "Update Ship Quantity";
            updateShipQuantityOrderDetailAction.TargetObjectType = typeof(OrderDetailForPacking);
            updateShipQuantityOrderDetailAction.TargetViewType = ViewType.ListView;
            updateShipQuantityOrderDetailAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            updateShipQuantityOrderDetailAction.Shortcut = "CtrlU";
            updateShipQuantityOrderDetailAction.Execute += UpdateShipQuantityOrderDetailAction_Execute;
        }
        private void UpdateShipQuantityOrderDetailAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var orderDetails = ((ListView)View).CollectionSource.List
                                .Cast<OrderDetailForPacking>().ToList();
            var masterObject = ((PropertyCollectionSource)((ListView)View).CollectionSource)
                                .MasterObject as PackingLineCreateParam;
            var errorMessage = "";

            var ratios = masterObject.PackingRatios.ToList();
            if(ratios.Find(x => x.TotalQuantity > 0) != null)
            {
                ratios.ForEach(x =>
                {
                    orderDetails.ForEach(y =>
                    {
                        if (y.Size == x.Size)
                        {
                            y.ShipQuantity = x.TotalQuantity;
                        }
                    });
                    //x.TotalQuantity = 0;
                });
            }
            else
            {
                if(masterObject.PackingType == PackingType.AssortedSizeSolidColor)
                {
                    if(masterObject.QuantityPackagePerCarton == 0)
                    {
                        errorMessage = "Please key quantity package per carton";
                    }
                    else
                    {
                        if (ratios.FirstOrDefault(x => x.TotalCarton > 0) == null)
                        {
                            errorMessage = "Please key total carton";
                        }
                        else
                        {
                            var totalCarton = ratios.FirstOrDefault(x => x.TotalCarton > 0).TotalCarton;
                            ratios.ForEach(x =>
                            {
                                orderDetails.ForEach(y =>
                                {
                                    if (y.Size == x.Size && y.ShipQuantity > totalCarton * masterObject.QuantityPackagePerCarton * x.Ratio)
                                    {
                                        y.ShipQuantity = totalCarton * masterObject.QuantityPackagePerCarton * x.Ratio;
                                    }
                                });
                            });
                        }
                    }
                }
                else
                {
                    if(masterObject.TotalQuantity > 0)
                    {
                        ratios.ForEach(x =>
                        {
                            orderDetails.ForEach(y =>
                            {
                                if (y.Size == x.Size && y.ShipQuantity > x.TotalCarton * masterObject.TotalQuantity)
                                {
                                    y.ShipQuantity = x.TotalCarton * masterObject.TotalQuantity;
                                }
                            });
                        });
                    }
                    else
                    {
                        ratios.ForEach(x =>
                        {
                            orderDetails.ForEach(y =>
                            {
                                if (y.Size == x.Size && y.ShipQuantity > x.TotalCarton * x.Ratio)
                                {
                                    y.ShipQuantity = x.TotalCarton * x.Ratio;
                                }
                            });
                        });
                    }
                }
            }

            if(!string.IsNullOrEmpty(errorMessage))
            {
                var error = Message.GetMessageOptions(errorMessage, "Error",
                            InformationType.Error, null, 5000);
                Application.ShowViewStrategy.ShowMessage(error);
            }
            else
            {
                View.Refresh(true);
                (View as ListView).EditView?.Refresh();
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
