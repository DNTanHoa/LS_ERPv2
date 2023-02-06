using AutoMapper;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Service.Request;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class PopupFabricPurchaseOrder_ViewController : ObjectViewController<DetailView, FabricInvoicePopupModel>
    {
        public PopupFabricPurchaseOrder_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction popupFabricPurchaseFromReceipt = new PopupWindowShowAction(this, "PopupFabricPurchaseOrderFromReceipt", PredefinedCategory.Unspecified);
            popupFabricPurchaseFromReceipt.ImageName = "Header";
            popupFabricPurchaseFromReceipt.Caption = "Fabric";
            popupFabricPurchaseFromReceipt.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            popupFabricPurchaseFromReceipt.Shortcut = "CtrlShiftF";

            popupFabricPurchaseFromReceipt.CustomizePopupWindowParams += PopupFabricPurchaseFromReceipt_CustomizePopupWindowParams; ;
            popupFabricPurchaseFromReceipt.Execute += PopupFabricPurchaseFromReceipt_Execute;
        }

        private void PopupFabricPurchaseFromReceipt_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var viewObject = e.PopupWindowView.CurrentObject as FabricPopupModel;
            var config = new MapperConfiguration(x =>
            {
                x.CreateMap<FabricPurchaseOrderInforData, ReceiptGroupLineRequest>()
                    .ForMember(x => x.ReceiptQuantity, y => y.MapFrom(s => s.EntryQuantity))
                    .ForMember(x => x.UnitID, y => y.MapFrom(s => s.PurchaseUnitID))
                    .ForMember(x => x.FabricPurchaseOrderGroupLineID, y => y.MapFrom(s => s.FabricPurchaseOrderLineID))
                    .ForMember(x => x.FabricPurchaseOrderNumber, y => y.MapFrom(s => s.FabricPurchaseOrderLineNumber))
                    .ForMember(x => x.PurchaseQuantity, y => y.MapFrom(s => s.PurchaseQuantity))
                    .ForMember(x => x.StorageBinCode, y => y.MapFrom(s => s.BinCode))
                    .ForMember(x => x.Remark, y => y.MapFrom(s => s.Remark))
                    .ForMember(x => x.DyeLotNumber, y => y.MapFrom(s => s.DyeNumber))
                    .ForMember(x => x.GarmentColorCode, y => y.MapFrom(s => s.GarmentColorCodes))
                    .ForMember(x => x.VendorDeliveriedQuantity, y => y.MapFrom(s => s.EntryQuantity));
            });

            var mapper = config.CreateMapper();

            var lines = new List<ReceiptGroupLineRequest>();

            foreach (var itemFB in viewObject.FabricPurchaseInfor)
            {
                if (itemFB.EntryUnitID != itemFB.PurchaseUnitID)
                {
                    if (viewObject.Unit.ID.ToUpper().Trim() == "M")
                    {
                        //fabric.PurchaseQuantity = Math.Round((decimal)fabric.PurchaseQuantity / (decimal)unit.Factor, (int)unit.Rouding);
                        itemFB.EntryQuantity = Math.Round((decimal)itemFB.EntryQuantity / (decimal)itemFB.FactorUnit, 2);
                    }
                }
            }

            lines = viewObject.FabricPurchaseInfor.Where(x => x.PurchaseQuantity > 0)
                .Select(x => mapper.Map<ReceiptGroupLineRequest>(x)).ToList();

            var request = new CreateReceiptRequest()
            {
                InvoiceNumber = viewObject.InvoiceNumber,
                InvoiceNumberNoTotal = viewObject.InvoiceNumberNoTotal,
                ProductionMethodCode = viewObject.ProductionMethods?.Code,
                FabricPurchaseOrderID = viewObject.FabricPurchaseOrder?.ID,
                FabricPurchaseOrderNumber = viewObject.FabricPurchaseOrder?.Number,
                CustomerStyle = viewObject.CustomerStyle,
                ArrivedDate = viewObject.ArrivedDate,
                ReceiptDate = viewObject.ReceiptDate,
                EntriedDate = viewObject.ReceiptDate,
                DocumentReferenceNumber = viewObject.DocumentReferenceNumber,
                StorageCode = viewObject.Storage?.Code,
                Offset = viewObject.Offset,
                CustomerID = viewObject.FabricPurchaseOrder?.CustomerID,
                ReceiptBy = SecuritySystem.CurrentUserName,
                EntriedBy = SecuritySystem.CurrentUserName,
                Username = SecuritySystem.CurrentUserName,
                ReceiptGroupLines = lines,
                VendorName = viewObject.FabricPurchaseOrder?.FabricSupplier
            };

            var service = new ReceiptService();
            var respone = service.CreateReceiptFabric(request).Result;
            var messageOptions = new MessageOptions();

            if (respone != null)
            {
                if (respone.Result.Code == "100")
                {
                    messageOptions = Message.GetMessageOptions("Create successfully", "Success",
                        InformationType.Success, null, 5000);
                }
                else
                {
                    messageOptions = Message.GetMessageOptions(respone.Result.Message, "Error",
                        InformationType.Error, null, 5000);
                }
            }
            else
            {
                messageOptions = Message.GetMessageOptions("Unexpected error", "Error", InformationType.Error, null, 5000);
            }

            View.Refresh();
            Application.ShowViewStrategy.ShowMessage(messageOptions);
        }

        private void PopupFabricPurchaseFromReceipt_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var viewObject = View.CurrentObject as FabricInvoicePopupModel;

            var model = new FabricPopupModel()
            {
                Storage = ObjectSpace.GetObjects<Storage>().FirstOrDefault(x => x.Code == "FB"),
                Unit = ObjectSpace.GetObjects<Unit>().FirstOrDefault(x => x.ID == "YDS"),
                ProductionMethods = ObjectSpace.GetObjects<PriceTerm>().FirstOrDefault(x => x.Code == "CMT"),
                ArrivedDate = viewObject.ArrivedDate,
                ReceiptDate = DateTime.Today,
                InvoiceNumber = viewObject.InvoiceNumber,
                InvoiceNumberNoTotal = viewObject.InvoiceNumberNoTotal,
                DocumentReferenceNumber = viewObject.DocumentReferenceNumber

            };
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
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
