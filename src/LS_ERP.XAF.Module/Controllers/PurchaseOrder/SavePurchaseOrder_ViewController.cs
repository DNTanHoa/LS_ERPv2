using AutoMapper;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Dtos;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class SavePurchaseOrder_ViewController : ViewController
    {
        public SavePurchaseOrder_ViewController()
        {
            InitializeComponent();

            SimpleAction savePurchaseOrderAction = new SimpleAction(this, "SavePurchaseOrder", PredefinedCategory.Unspecified);
            savePurchaseOrderAction.ImageName = "Save";
            savePurchaseOrderAction.Caption = "Save";
            savePurchaseOrderAction.TargetObjectType = typeof(PurchaseOrder);
            savePurchaseOrderAction.TargetViewType = ViewType.DetailView;
            savePurchaseOrderAction.PaintStyle = ActionItemPaintStyle.Default;
            savePurchaseOrderAction.Category = "Save";
            savePurchaseOrderAction.Shortcut = "CtrlS";

            savePurchaseOrderAction.Execute += SavePurchaseOrderAction_Execute;
        }

        private void SavePurchaseOrderAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var purchaseOrder = View.CurrentObject as PurchaseOrder;
            MessageOptions message = null;
            var service = new PurchaseOrderService();

            foreach(var purchaseOrderLine in purchaseOrder?.PurchaseOrderGroupLines?
                .SelectMany(x => x.PurchaseOrderLines))
            {
                purchaseOrderLine.PurchaseOrderID = purchaseOrder.ID;
            }

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PurchaseOrder, CreatePurchaseOrderRequest>();
                cfg.CreateMap<PurchaseOrder, UpdatePurchaseOrderRequest>();
                cfg.CreateMap<ProductionBOM, ProductionBOMDto>();
                cfg.CreateMap<ItemStyle, ItemStyleDto>();
                cfg.CreateMap<PurchaseOrderLine, PurchaseOrderLineDto>();
                cfg.CreateMap<PurchaseOrderGroupLine, PurchaseOrderGroupLineDto>();
                cfg.CreateMap<ReservationEntry, ReservationEntryDto>();
                cfg.CreateMap<ReservationForecastEntry, ReservationForecastEntryDto>();
            });
            
            var mapper = config.CreateMapper();
            var objectSpace = Application.CreateObjectSpace(typeof(PurchaseOrder));

            var existPurchaseOrder = objectSpace.GetObjectByKey<PurchaseOrder>(purchaseOrder.ID);

            if(existPurchaseOrder != null)
            {
                var request = mapper.Map<UpdatePurchaseOrderRequest>(purchaseOrder);
                request.UserName = SecuritySystem.CurrentUserName;

                var response = service.UpdatePurchaseOrder(request).Result;

                if (response != null)
                {
                    if (response.Result.Code == "000")
                    {
                        message = Message.GetMessageOptions("Action successfully", "Success", InformationType.Success,
                            null, 5000);
                    }
                    else
                    {
                        message = Message.GetMessageOptions(response.Result.Message, "Error", InformationType.Error,
                            null, 5000);
                    }
                }
                else
                {
                    message = Message.GetMessageOptions("Unknown error. Contact your admin", "Error", InformationType.Error,
                            null, 5000);
                }

                if (message != null)
                    Application.ShowViewStrategy.ShowMessage(message);
                
                View.Refresh(false);
                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();
            }
            else
            {
                var request = mapper.Map<CreatePurchaseOrderRequest>(purchaseOrder);
                request.UserName = SecuritySystem.CurrentUserName;

                var response = service.CreatePurchaseOrder(request).Result;

                if (response != null)
                {
                    if (response.Result.Code == "000")
                    {
                        message = Message.GetMessageOptions("Action successfully", "Success", InformationType.Success,
                            null, 5000);
                    }
                    else
                    {
                        message = Message.GetMessageOptions(response.Result.Message, "Error", InformationType.Error,
                            null, 5000);
                    }
                }
                else
                {
                    message = Message.GetMessageOptions("Unknown error. Contact your admin", "Error", InformationType.Error,
                            null, 5000);
                }

                if (message != null)
                    Application.ShowViewStrategy.ShowMessage(message);

                var newPurchaseOrder = objectSpace
                    .GetObjects<PurchaseOrder>(CriteriaOperator.Parse("[Number] = ?", purchaseOrder.Number))
                    .FirstOrDefault();

                if (newPurchaseOrder != null)
                {
                    purchaseOrder = newPurchaseOrder;
                    purchaseOrder.PurchaseOrderGroupLines = newPurchaseOrder.PurchaseOrderGroupLines;
                    purchaseOrder.PurchaseOrderLines = newPurchaseOrder.PurchaseOrderLines;
                }

                View.Refresh();
                var newDetailView = Application.CreateDetailView(objectSpace, newPurchaseOrder, false);
                Frame.SetView(newDetailView);
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
