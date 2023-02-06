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
using PurchaseOrderInforData = LS_ERP.XAF.Module.DomainComponent.PurchaseOrderInforData;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class PopupPurchase_ViewController : ViewController
    {
        public PopupPurchase_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction popupPurchaseFromReceipt = new PopupWindowShowAction(this, "PopupPurchaseOrderFromReceipt", PredefinedCategory.Unspecified);
            popupPurchaseFromReceipt.ImageName = "Header";
            popupPurchaseFromReceipt.Caption = "Purchase (Ctrl + Shift + O)";
            popupPurchaseFromReceipt.TargetObjectType = typeof(ReceiptSearchParam);
            popupPurchaseFromReceipt.TargetViewType = ViewType.DetailView;
            popupPurchaseFromReceipt.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            popupPurchaseFromReceipt.Shortcut = "CtrlShiftO";

            popupPurchaseFromReceipt.CustomizePopupWindowParams += PopupPurchaseFromReceipt_CustomizePopupWindowParams; ;
            popupPurchaseFromReceipt.Execute += PopupPurchaseFromReceipt_Execute;
        }

        private void PopupPurchaseFromReceipt_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var viewObject = e.PopupWindowView.CurrentObject as PurchasePopupModel;
            var config = new MapperConfiguration(x =>
            {
                x.CreateMap<PurchaseOrderInforData, ReceiptGroupLineRequest>()
                    .ForMember(x => x.ReceiptQuantity, y => y.MapFrom(s => s.EntryQuantity))
                    .ForMember(x => x.UnitID, y => y.MapFrom(s => s.EntryUnitID))
                    .ForMember(x => x.PurchaseOrderGroupLineID, y => y.MapFrom(s => s.PurchaseOrderGroupLineID))
                    .ForMember(x => x.PurchaseQuantity, y => y.MapFrom(s => s.PurchaseQuantity))
                    .ForMember(x => x.StorageBinCode, y => y.MapFrom(s => s.BinCode))
                    .ForMember(x => x.Remark, y => y.MapFrom(s => s.Remark))
                    .ForMember(x => x.VendorDeliveriedQuantity, y => y.MapFrom(s => s.EntryQuantity));
                x.CreateMap<PurchaseOrderGroupInforData, ReceiptGroupLineRequest>()
                    .ForMember(x => x.ReceiptQuantity, y => y.MapFrom(s => s.EntryQuantity))
                    .ForMember(x => x.UnitID, y => y.MapFrom(s => s.EntryUnitID))
                    .ForMember(x => x.PurchaseQuantity, y => y.MapFrom(s => s.PurchaseQuantity))
                    .ForMember(x => x.StorageBinCode, y => y.MapFrom(s => s.BinCode))
                    .ForMember(x => x.Remark, y => y.MapFrom(s => s.Remark))
                    .ForMember(x => x.VendorDeliveriedQuantity, y => y.MapFrom(s => s.EntryQuantity));
            });

            var mapper = config.CreateMapper();

            var lines = new List<ReceiptGroupLineRequest>();

            if (viewObject.GetGroup)
            {
                lines = viewObject.PurchaseGroup.Where(x => x.EntryQuantity > 0)
                    .Select(x => mapper.Map<ReceiptGroupLineRequest>(x)).ToList();
            }
            else
            {
                lines = viewObject.PurchaseInfor.Where(x => x.EntryQuantity > 0)
                    .Select(x => mapper.Map<ReceiptGroupLineRequest>(x)).ToList();
            }

            var request = new CreateReceiptRequest()
            {
                InvoiceNumber = viewObject.InvoiceNumber,
                InvoiceNumberNoTotal = viewObject.InvoiceNumberNoTotal,
                PurchaseOrderID = viewObject.PurchaseOrder?.ID,
                PurchaseOrderNumber = viewObject.PurchaseOrder?.Number,
                ArrivedDate = viewObject.ArrivedDate,
                ReceiptDate = viewObject.ReceiptDate,
                EntriedDate = viewObject.ReceiptDate,
                DocumentReferenceNumber = viewObject.DocumentReferenceNumber,
                StorageCode = viewObject.Storage?.Code,
                VendorID = viewObject.PurchaseOrder?.VendorID,
                VendorAddress = viewObject.PurchaseOrder?.Vendor?.Address,
                VendorName = viewObject.PurchaseOrder?.Vendor?.Name,
                CustomerID = viewObject.PurchaseOrder?.CustomerID,
                ReceiptBy = SecuritySystem.CurrentUserName,
                EntriedBy = SecuritySystem.CurrentUserName,
                Username = SecuritySystem.CurrentUserName,
                ReceiptGroupLines = lines,
            };

            var service = new ReceiptService();
            var respone = service.CreateReceipt(request).Result;
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

        private void PopupPurchaseFromReceipt_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new PurchasePopupModel()
            {
                Storage = ObjectSpace.GetObjects<Storage>().FirstOrDefault(),
                ArrivedDate = DateTime.Today,
                ReceiptDate = DateTime.Today
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
