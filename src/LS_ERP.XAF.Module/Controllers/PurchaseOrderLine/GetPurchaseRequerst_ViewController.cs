using AutoMapper;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Dtos;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class GetPurchaseRequerst_ViewController : ViewController
    {
        public GetPurchaseRequerst_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction getPurchaseRequestAction = new PopupWindowShowAction(this, "GetPurchaseRequestForPurchaseOrderLine",
              PredefinedCategory.Unspecified);

            getPurchaseRequestAction.ImageName = "ShowAllFieldCodes";
            getPurchaseRequestAction.Caption = "Purchase Request";
            getPurchaseRequestAction.TargetObjectType = typeof(PurchaseOrderLine);
            getPurchaseRequestAction.TargetViewType = ViewType.ListView;
            getPurchaseRequestAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            getPurchaseRequestAction.Shortcut = "CtrlShiftM";

            getPurchaseRequestAction.CustomizePopupWindowParams += GetPurchaseRequestAction_CustomizePopupWindowParams;
            getPurchaseRequestAction.Execute += GetPurchaseRequestAction_Execute;

        }

        private void GetPurchaseRequestAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var purchaseOrder = (((ListView)View).CollectionSource as PropertyCollectionSource).MasterObject
                 as PurchaseOrder;

            ListPropertyEditor listPropertyEditor = ((DetailView)e.PopupWindowView)
                .FindItem("RequestMaterials") as ListPropertyEditor;
            MessageOptions message = null;

            var config = new MapperConfiguration(c =>
            {
                c.CreateMap<PurchaseRequestLine, PurchaseRequestLineDto>()
                    .ForMember(x => x.GarmentColorName, y => y.MapFrom(s => s.GarmentColorName))
                    .ForMember(x => x.PurchaseRequestID, y => y.MapFrom(s => s.PurchaseRequest.ID))
                    .ForMember(x => x.PurchaseRequestGroupLineID, y => y.MapFrom(s => s.PurchaseRequestGroupLineID))
                    .ForMember(x => x.PurchaseRequestID, y => y.MapFrom(s => s.ID))
                    .ForMember(x => x.GarmentColorCode, y => y.MapFrom(s => s.GarmentColorCode));

                c.CreateMap<PurchaseOrderLine, PurchaseOrderLineDto>();
                c.CreateMap<PurchaseOrderGroupLine, PurchaseOrderGroupLineDto>();
                c.CreateMap<ReservationEntry, ReservationEntryDto>();
            });

            var mapper = config.CreateMapper();

            var request = new GroupPurchaseRequestLineToPurchaseOrderLineRequest()
            {
                PurchaseOrderLines = purchaseOrder.PurchaseOrderLines?
                                        .Where(x => x.ID > 0)
                                        .Select(x => mapper.Map<PurchaseOrderLineDto>(x)).ToList(),
                PurchaseOrderGroupLines = purchaseOrder.PurchaseOrderGroupLines?
                                        .Where(x => x.ID > 0)
                                        .Select(x => mapper.Map<PurchaseOrderGroupLineDto>(x)).ToList(),
                PurchaseRequestLines = listPropertyEditor.ListView
                                        .SelectedObjects.Cast<PurchaseRequestLine>()?
                                        .ToList()
                                        .Select(x => mapper.Map<PurchaseRequestLineDto>(x)).ToList()
            };

            var service = new PurchaseRequestLineService();

            var response = service.GroupToPurchaseOrderLine(request).Result;

            if (response != null)
            {
                List<PurchaseOrderGroupLine> groupLines = null;
                List<PurchaseOrderLine> lines = null;

                if (purchaseOrder.PurchaseOrderGroupLines == null)
                {
                    purchaseOrder.PurchaseOrderGroupLines = new List<PurchaseOrderGroupLine>();
                    groupLines = new List<PurchaseOrderGroupLine>();
                }
                else
                {
                    groupLines = purchaseOrder.PurchaseOrderGroupLines.ToList();
                }

                if (purchaseOrder.PurchaseOrderLines == null)
                {
                    purchaseOrder.PurchaseOrderLines = new List<PurchaseOrderLine>();
                    lines = new List<PurchaseOrderLine>();
                }
                else
                {
                    lines = purchaseOrder.PurchaseOrderLines.ToList();
                }

                /// Add new group line
                groupLines.AddRange(response.Data?.PurchaseOrderGroupLines.Where(x => x.ID == 0));

                /// Update old group line
                foreach (var currentGroupLine in purchaseOrder.PurchaseOrderGroupLines)
                {
                    if (currentGroupLine.ID > 0)
                    {
                        var groupLine = response.Data?.PurchaseOrderGroupLines
                            .FirstOrDefault(x => x.ID == currentGroupLine.ID);

                        if (groupLine != null)
                        {
                            currentGroupLine.Quantity = groupLine.Quantity;
                        }
                    }
                }

                lines.AddRange(response.Data?.PurchaseOrderLines.Where(x => x.ID == 0).ToList());

                purchaseOrder.PurchaseOrderGroupLines
                        .AddRange(response.Data?.PurchaseOrderGroupLines.Where(x => x.ID == 0));
                purchaseOrder.PurchaseOrderLines
                    = purchaseOrder.PurchaseOrderGroupLines.SelectMany(x => x.PurchaseOrderLines).ToList();
            }
            else
            {
                message = Message.GetMessageOptions("Unknown error. Contact your admin", "Error", InformationType.Error,
                        null, 5000);
            }

            if (message != null)
                Application.ShowViewStrategy.ShowMessage(message);

            if (ObjectSpace is NonPersistentObjectSpace)
                ObjectSpace.Rollback(false);

            View.Refresh();
        }

        private void GetPurchaseRequestAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var purchaseOrder = (((ListView)View).CollectionSource as PropertyCollectionSource).MasterObject as PurchaseOrder;
            var objectSpace = this.ObjectSpace;
            var model = new PurchaseRequestMaterial()
            {
                ToDate = DateTime.Today,
                FromDate = DateTime.Today.AddMonths(-12),
                Vendor = purchaseOrder.Vendor,
            };
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            e.View = Application.CreateDetailView(objectSpace, model, false);
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
