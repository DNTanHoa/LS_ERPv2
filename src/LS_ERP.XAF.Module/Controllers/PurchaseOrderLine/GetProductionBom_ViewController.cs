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
using LS_ERP.XAF.Module.Service.Request;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class GetProductionBom_ViewController : ViewController
    {
        public GetProductionBom_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction getProductionBomAction = new PopupWindowShowAction(this, "GetProductionBomForPurchaseOrderLine", 
                PredefinedCategory.Unspecified);

            getProductionBomAction.ImageName = "Header";
            getProductionBomAction.Caption = "Production Bom";
            getProductionBomAction.TargetObjectType = typeof(PurchaseOrderLine);
            getProductionBomAction.TargetViewType = ViewType.ListView;
            getProductionBomAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            getProductionBomAction.Shortcut = "CtrlShiftB";

            getProductionBomAction.CustomizePopupWindowParams += GetProductionBomAction_CustomizePopupWindowParams;
            getProductionBomAction.Execute += GetProductionBomAction_Execute;
        }

        private void GetProductionBomAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var param = e.PopupWindowViewCurrentObject as PurchaseProductionBOM;
            var purchaseOrder = (((ListView)View).CollectionSource as PropertyCollectionSource).MasterObject
                as PurchaseOrder;

            ListPropertyEditor listPropertyEditor = ((DetailView)e.PopupWindowView)
                .FindItem("ProductionBOMs") as ListPropertyEditor;
            MessageOptions message = null;

            if (listPropertyEditor != null)
            {
                var config = new MapperConfiguration(c =>
                {
                    c.CreateMap<ProductionBOM, ProductionBOMDto>();
                    c.CreateMap<ItemStyle, ItemStyleDto>();
                    c.CreateMap<PurchaseOrderLine, PurchaseOrderLineDto>();
                    c.CreateMap<PurchaseOrderGroupLine, PurchaseOrderGroupLineDto>();
                    c.CreateMap<ReservationEntry, ReservationEntryDto>();
                });

                var mapper = config.CreateMapper();

                var service = new ProductionBOMService();

                var productionBoms = listPropertyEditor.ListView.SelectedObjects.Cast<ProductionBOM>()?
                                            .Select(x => mapper.Map<ProductionBOMDto>(x)).ToList();

                productionBoms.ForEach(x =>
                {
                    var quantity = x.RequiredQuantity * param.Percent / 100;
                    var wareHouseQuantity = x.WareHouseQuantity * param.Percent / 100;

                    if (x.ReservedQuantity != null && x.RemainQuantity <= quantity)
                    {
                        x.RequiredQuantity = x.RemainQuantity;
                        x.WareHouseQuantity = x.WareHouseQuantity * param.Percent / 100 * x.RemainQuantity / x.RequiredQuantity;
                        x.ReservedQuantity = x.ReservedQuantity * param.Percent / 100 * x.RemainQuantity / x.RequiredQuantity;
                    }
                    else
                    {
                        x.RequiredQuantity = x.RequiredQuantity * param.Percent / 100;
                        x.WareHouseQuantity = x.WareHouseQuantity * param.Percent / 100;
                    }

                });

                var request = new GroupToPurchaseOrderLineRequest()
                {
                    PurchaseOrderLines = purchaseOrder.PurchaseOrderLines?
                                            .Where(x => x.ID > 0)
                                            .Select(x => mapper.Map<PurchaseOrderLineDto>(x)).ToList(),
                    PurchaseOrderGroupLines = purchaseOrder.PurchaseOrderGroupLines?
                                            .Where(x => x.ID > 0)
                                            .Select(x => mapper.Map<PurchaseOrderGroupLineDto>(x)).ToList(),
                    ProductionBOMs = productionBoms,
                };

                var response = service.GroupToPurchaseOrderLine(request).Result;

                if (response != null)
                {
                    if (response.Result.Code == "000")
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

                if (ObjectSpace is NonPersistentObjectSpace)
                    ObjectSpace.Rollback(false);
            }

            if (message != null)
                Application.ShowViewStrategy.ShowMessage(message);

            View.Refresh();
        }
        
        private void GetProductionBomAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var purchaseOrder = (((ListView)View).CollectionSource as PropertyCollectionSource).MasterObject as PurchaseOrder;
            var objectSpace = this.ObjectSpace;
            var model = new PurchaseProductionBOM()
            {
                VendorID = purchaseOrder.Vendor?.ID,
                ReservationEntries = purchaseOrder.PurchaseOrderLines
                    .SelectMany(x => x.ReservationEntries).ToList(),
                Percent = 100
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
