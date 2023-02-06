using AutoMapper;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class FabricDataInfoAction_ViewController 
        : ObjectViewController<ListView, FabricPurchaseOrderInforData>
    {
        public FabricDataInfoAction_ViewController()
        {
            InitializeComponent();
            PopupWindowShowAction popupFabricData = new PopupWindowShowAction(this, "PopupCloneFabricData", PredefinedCategory.Unspecified);
            popupFabricData.ImageName = "Header";
            popupFabricData.Caption = "Clone (Ctrl + Shift + C)";
            popupFabricData.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            popupFabricData.Shortcut = "CtrlShiftC";
            popupFabricData.TargetObjectType = typeof(FabricPurchaseOrderInforData);
            popupFabricData.TargetViewType = ViewType.ListView;

            popupFabricData.CustomizePopupWindowParams += PopupCloneFabricData_CustomizePopupWindowParams;
            popupFabricData.Execute += PopupCloneFabricData_Execute;
        }

        private void PopupCloneFabricData_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var viewObject = e.PopupWindowView.CurrentObject as CloneFabricPopupModel;
            var property = ((View as ListView).CollectionSource) as PropertyCollectionSource;
            var fbPopupModel = property.MasterObject as FabricPopupModel;


            if (fbPopupModel.FabricPurchaseInfor != null && fbPopupModel.FabricPurchaseInfor.Count > 0)
            {
                var itemFB = fbPopupModel.FabricPurchaseInfor.FirstOrDefault();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<FabricPurchaseOrderInforData, FabricPurchaseOrderInforData>()
                        .ForMember(x => x.LotNumber, y => y.Ignore())
                        .ForMember(x => x.DyeNumber, y => y.Ignore())
                        .ForMember(x => x.EntryQuantity, y => y.Ignore());
                });

                var mapper = config.CreateMapper();

                for (int i = 0; i < viewObject.NumberRow; i++)
                {
                    if (fbPopupModel.FabricPurchaseInfor == null)
                    {
                        fbPopupModel.FabricPurchaseInfor = new List<FabricPurchaseOrderInforData>();
                    }

                    var fb = mapper.Map<FabricPurchaseOrderInforData>(itemFB);

                    fbPopupModel.FabricPurchaseInfor.Add(fb);
                }

                ObjectSpace.CommitChanges();
                property.Reload();
                View.Refresh();
            }
        }

        private void PopupCloneFabricData_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new CloneFabricPopupModel();
            model.NumberRow = 1;
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
