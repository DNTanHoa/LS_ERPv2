using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent.Report;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class InventoryReportDetailAction_ViewController 
        : ObjectViewController<ListView, InventoryReportDetail>
    {
        public InventoryReportDetailAction_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction updateInventoryBinCodeAction = new PopupWindowShowAction(this, 
                "UpdateInventoryBinCodeAction", PredefinedCategory.Unspecified);
            updateInventoryBinCodeAction.ImageName = "AlignmentBottomLeft";
            updateInventoryBinCodeAction.Caption = "Update Bin Code (Ctrl + B)";
            updateInventoryBinCodeAction.TargetObjectType = typeof(InventoryReportDetail);
            updateInventoryBinCodeAction.TargetViewType = ViewType.ListView;
            updateInventoryBinCodeAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            updateInventoryBinCodeAction.Shortcut = "CtrlB";

            updateInventoryBinCodeAction.CustomizePopupWindowParams += UpdateInventoryBinCodeAction_CustomizePopupWindowParams;
            updateInventoryBinCodeAction.Execute += UpdateInventoryBinCodeAction_Execute;
        }

        private void UpdateInventoryBinCodeAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new UpdateInventoryBinCodeParam();
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.View = view;
        }

        private void UpdateInventoryBinCodeAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var viewObjects = View.SelectedObjects.Cast<InventoryReportDetail>();
            var param = e.PopupWindowViewCurrentObject as UpdateInventoryBinCodeParam;
            
            if(viewObjects != null &&
               viewObjects.Any())
            {
                var objectSpace = Application.CreateObjectSpace(typeof(StorageDetail));
                var criteriaString = "ID IN (" + string.Join(",", viewObjects.Select(x => x.StorageDetailID)) + ")";
                var storageDetails = objectSpace.GetObjects<StorageDetail>(
                    CriteriaOperator.Parse(criteriaString)).ToList();

                foreach (var storageDetail in storageDetails)
                {
                    storageDetail.StorageBinCode = param.BinCode;
                }

                foreach (var viewObject in viewObjects)
                {
                    viewObject.BinCode = param.BinCode;
                }

                objectSpace.CommitChanges();
                View.Refresh();
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

    [DomainComponent]
    public class UpdateInventoryBinCodeParam
    {
        public string BinCode { get; set; }
    }
}
