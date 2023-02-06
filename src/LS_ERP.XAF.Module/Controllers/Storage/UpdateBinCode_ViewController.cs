using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class UpdateBinCode_ViewController : ViewController
    {
        public UpdateBinCode_ViewController()
        {
            PopupWindowShowAction updateStorageBinCode = new PopupWindowShowAction(this, "UpdateStorageBinCode", PredefinedCategory.Unspecified);
            updateStorageBinCode.ImageName = "AlignmentBottomLeft";
            updateStorageBinCode.Caption = "Update Bin Code";
            updateStorageBinCode.TargetObjectType = typeof(StorageDetail);
            updateStorageBinCode.TargetViewType = ViewType.Any;
            updateStorageBinCode.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            updateStorageBinCode.CustomizePopupWindowParams += UpdateStorageBinCode_CustomizePopupWindowParams;
            updateStorageBinCode.Execute += UpdateStorageBinCode_Execute;

            PopupWindowShowAction updateLocation = new PopupWindowShowAction(this, "UpdateFinishGoodLocation", PredefinedCategory.Unspecified);
            updateLocation.ImageName = "AlignmentBottomLeft";
            updateLocation.Caption = "Update Location";
            updateLocation.TargetObjectType = typeof(Storage);
            updateLocation.TargetViewType = ViewType.Any;
            updateLocation.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            updateLocation.CustomizePopupWindowParams += UpdateLocation_CustomizePopupWindowParams;
            updateLocation.Execute += UpdateLocation_Execute;

            InitializeComponent();
        }

        private void UpdateLocation_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {

        }

        private void UpdateLocation_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            
        }

        private void UpdateStorageBinCode_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var selectedStorageDetails = View.SelectedObjects.Cast<StorageDetail>();
            var param = e.PopupWindowViewCurrentObject as UpdateBinCodeParam;

            if(selectedStorageDetails != null &&
               selectedStorageDetails.Any())
            {
                foreach(var storageDetail in selectedStorageDetails)
                {
                    storageDetail.StorageBinCode = param.BinCode;
                }

                ObjectSpace.CommitChanges();
                View.Refresh();
            }
        }

        private void UpdateStorageBinCode_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new UpdateBinCodeParam();
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
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
