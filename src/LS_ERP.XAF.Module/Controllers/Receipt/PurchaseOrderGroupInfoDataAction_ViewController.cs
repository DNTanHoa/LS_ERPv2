using AutoMapper;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class PurchaseOrderGroupInfoDataAction_ViewController : ObjectViewController<ListView, PurchaseOrderGroupInforData>
    {
        public PurchaseOrderGroupInfoDataAction_ViewController()
        {
            InitializeComponent();
            PopupWindowShowAction popupPurchaseGroupInfoData = new PopupWindowShowAction(this, "PopupClonePurchaseGroupInfoData", PredefinedCategory.Unspecified);
            popupPurchaseGroupInfoData.ImageName = "Header";
            popupPurchaseGroupInfoData.Caption = "Clone (Ctrl + Shift + C)";
            popupPurchaseGroupInfoData.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            popupPurchaseGroupInfoData.Shortcut = "CtrlShiftC";
            //popupPurchaseGroupInfoData.TargetObjectType = typeof(FabricPurchaseOrderInforData);
            //popupPurchaseGroupInfoData.TargetViewType = ViewType.ListView;

            popupPurchaseGroupInfoData.CustomizePopupWindowParams += PopupClonePurchaseGroupInfoData_CustomizePopupWindowParams;
            popupPurchaseGroupInfoData.Execute += PopupClonePurchaseGroupInfoData_Execute;
        }

        private void PopupClonePurchaseGroupInfoData_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var viewObject = e.PopupWindowView.CurrentObject as CloneFabricPopupModel;
            var property = ((View as ListView).CollectionSource) as PropertyCollectionSource;
            var poPopupModel = property.MasterObject as PurchasePopupModel;


            if (poPopupModel.PurchaseGroup != null && poPopupModel.PurchaseGroup.Count > 0)
            {
                var itemPOs = e.SelectedObjects.Cast<PurchaseOrderGroupInforData>();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<PurchaseOrderGroupInforData, PurchaseOrderGroupInforData>()
                        .ForMember(x => x.LotNumber, y => y.Ignore())
                        .ForMember(x => x.DyeLotNumber, y => y.Ignore())
                        .ForMember(x => x.EntryQuantity, y => y.Ignore());
                });

                var mapper = config.CreateMapper();

                if (poPopupModel.PurchaseGroup == null)
                {
                    poPopupModel.PurchaseGroup = new List<PurchaseOrderGroupInforData>();
                }

                for (int i = 0; i < viewObject.NumberRow; i++)
                {
                    foreach (var item in itemPOs)
                    {
                        var po = mapper.Map<PurchaseOrderGroupInforData>(item);
                        poPopupModel.PurchaseGroup.Add(po);
                    }
                }

                ObjectSpace.CommitChanges();
                property.Reload();
                View.Refresh();
            }
        }

        private void PopupClonePurchaseGroupInfoData_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
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
