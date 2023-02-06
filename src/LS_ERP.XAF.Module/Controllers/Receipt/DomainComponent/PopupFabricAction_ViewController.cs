using AutoMapper;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class PopupFabricAction_ViewController : ObjectViewController<DetailView, FabricPopupModel>
    {
        public PopupFabricAction_ViewController()
        {
            InitializeComponent();

            SimpleAction loadFabricPurchaseInforData = new SimpleAction(this, "LoadFabricPurchaseInforData", PredefinedCategory.Unspecified);
            loadFabricPurchaseInforData.ImageName = "Actions_Reload";
            loadFabricPurchaseInforData.Caption = "Load Order";
            loadFabricPurchaseInforData.TargetObjectType = typeof(FabricPopupModel);
            loadFabricPurchaseInforData.TargetViewType = ViewType.DetailView;
            loadFabricPurchaseInforData.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            loadFabricPurchaseInforData.Execute += LoadFabricPurchaseData_Execute;
        }

        private void LoadFabricPurchaseData_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as FabricPopupModel;
            var criteria = CriteriaOperator.Parse("ID = ? ", viewObject.FabricPurchaseOrder?.ID);
            var fabricPurchaseGroupLines = ObjectSpace.GetObjects<FabricPurchaseOrder>(criteria);
            var unit = ObjectSpace.GetObjectByKey<Unit>(viewObject.FabricPurchaseOrder?.UnitID);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FabricPurchaseOrder, FabricPurchaseOrderInforData>()
                    .ForMember(x => x.PurchaseUnitID, y => y.MapFrom(s => s.UnitID))
                    .ForMember(x => x.PurchaseQuantity, y => y.MapFrom(s => s.OrderedQuantity))
                    .ForMember(x => x.EntryQuantity, y => y.MapFrom(s => s.ShippedQuantity))
                    .ForMember(x => x.FabricPurchaseOrderLineNumber, y => y.MapFrom(s => s.Number))
                    .ForMember(x => x.CustomerStyle, y => y.MapFrom(s => s.CustomerStyles))
                    .ForMember(x => x.Season, y => y.MapFrom(s => s.Seasons))
                    .ForMember(x => x.GarmentColorCodes, y => y.MapFrom(s => s.GarmentColorCodes))
                    .ForMember(x => x.FabricSupplier, y => y.MapFrom(s => s.FabricSupplier))
                    .ForMember(x => x.FabricPurchaseOrderLineID, y => y.MapFrom(s => s.ID));
            });

            var mapper = config.CreateMapper();
            var fabricPurchaseInfors = new List<FabricPurchaseOrderInforData>();

            if (viewObject.FabricPurchaseInfor == null)
            {
                viewObject.FabricPurchaseInfor = new List<FabricPurchaseOrderInforData>();
            }

            var hbFabricNumber = viewObject.FabricPurchaseOrder.Number;
            var checkHB = false;

            if (!string.IsNullOrEmpty(viewObject.InvoiceNumber) && viewObject.InvoiceNumber.ToUpper().Contains("HB")
                && !hbFabricNumber.ToUpper().Contains("HB"))
            {
                hbFabricNumber = "HB-" + hbFabricNumber;

                var criteriaFB = CriteriaOperator.Parse("[Number] = ?", hbFabricNumber);
                var exitsFBPO = ObjectSpace.GetObjects<FabricPurchaseOrder>(criteriaFB).FirstOrDefault();

                if (exitsFBPO != null)
                {
                    viewObject.FabricPurchaseOrder.ID = exitsFBPO.ID;
                    viewObject.FabricPurchaseOrder.Number = exitsFBPO.Number;
                    checkHB = false;
                }
                else
                {
                    checkHB = true;
                }
            }



            foreach (var itemFB in fabricPurchaseGroupLines)
            {
                var fabric = mapper.Map<FabricPurchaseOrderInforData>(itemFB);

                if (fabric != null)
                {
                    fabric.LotNumber = viewObject.LotNumber;
                    fabric.DyeNumber = viewObject.DyeLotNumber;
                    fabric.BinCode = viewObject.BinCode;
                    fabric.Season = viewObject.Season;
                    fabric.CustomerStyle = viewObject.CustomerStyle;
                    fabric.EntryUnitID = viewObject.Unit.ID;
                    fabric.Offset = viewObject.Offset;

                    if (checkHB)
                    {
                        fabric.FabricPurchaseOrderLineNumber = hbFabricNumber;
                        fabric.FabricPurchaseOrderLineID = 0;
                    }

                    if (viewObject.Unit != null)
                    {
                        if (viewObject.Unit.ID.ToUpper().Trim() == "M")
                        {
                            fabric.FactorUnit = unit.Factor;
                            fabric.RoundingUnit = unit.Rouding;
                        }
                    }

                    fabricPurchaseInfors.Add(fabric);
                }
            }

            viewObject.FabricPurchaseInfor = fabricPurchaseInfors;
            View.Refresh();

            if (!fabricPurchaseInfors.Any())
            {
                var infor = Message.GetMessageOptions("Fabric purchase has no data to entry", "infor",
                    InformationType.Info, null, 2000);
                Application.ShowViewStrategy.ShowMessage(infor);
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
