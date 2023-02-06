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
    public partial class PopupItemStyle_ViewController : ViewController
    {
        public PopupItemStyle_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction popupItemStyleFromReceip = new PopupWindowShowAction(this, "PopupItemStyleFromReceipt", PredefinedCategory.Unspecified);
            popupItemStyleFromReceip.ImageName = "Header";
            popupItemStyleFromReceip.Caption = "Entry FG(Ctrl + Shift + O)";
            popupItemStyleFromReceip.TargetObjectType = typeof(ReceiptSearchParam);
            popupItemStyleFromReceip.TargetViewType = ViewType.DetailView;
            popupItemStyleFromReceip.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            popupItemStyleFromReceip.Shortcut = "CtrlShiftO";

            popupItemStyleFromReceip.CustomizePopupWindowParams += PopupItemStyleFromReceip_CustomizePopupWindowParams;
            popupItemStyleFromReceip.Execute += PopupItemStyleFromReceip_Execute;
        }

        private void PopupItemStyleFromReceip_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var model = new ItemStylePopupModel()
            {
                Storage = ObjectSpace.GetObjects<Storage>().FirstOrDefault(),
                Customer = ObjectSpace.GetObjects<Customer>().FirstOrDefault(),
                ReceiptDate = DateTime.Now,
            };
            var view = Application.CreateDetailView(ObjectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            e.View = view;
        }

        private void PopupItemStyleFromReceip_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var viewObject = e.PopupWindowViewCurrentObject as ItemStylePopupModel;

            var config = new MapperConfiguration(x =>
            {
                x.CreateMap<OrderDetailReceipt, ReceiptGroupLineRequest>()
                    .ForMember(x => x.ReceiptQuantity, y => y.MapFrom(s => s.ReceiptQuantity))
                    .ForMember(x => x.UnitID, y => y.MapFrom(s => s.UnitID))
                    .ForMember(x => x.StorageBinCode, y => y.MapFrom(s => s.Location))
                    .ForMember(x => x.ItemName, y => y.MapFrom(s => s.ProductDescription))
                    .ForMember(x => x.Remark, y => y.MapFrom(s => s.Remark))
                    .ForMember(x => x.VendorDeliveriedQuantity, y => y.MapFrom(s => s.ReceiptQuantity));
            });
            var mapper = config.CreateMapper();

            var lines = new List<ReceiptGroupLineRequest>();
            lines = viewObject.OrderDetailReceipt.Where(x => string.IsNullOrEmpty(x.ReceiptQuantity))
                    .Select(x => mapper.Map<ReceiptGroupLineRequest>(x)).ToList();

            if (viewObject != null)
            {
                var service = new ReceiptService();

                var request = new CreateReceiptRequest()
                {
                    ArrivedDate = viewObject.ReceiptDate,
                    ReceiptDate = viewObject.ReceiptDate,
                    EntriedDate = viewObject.ReceiptDate,
                    DocumentReferenceNumber = viewObject.DocumentReferenceNumber,
                    StorageCode = viewObject.Storage?.Code,
                    CustomerID = viewObject.Customer?.ID,
                    ReceiptBy = SecuritySystem.CurrentUserName,
                    EntriedBy = viewObject.EntryBy,
                    Username = SecuritySystem.CurrentUserName,
                    ReceiptGroupLines = lines,
                };

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
