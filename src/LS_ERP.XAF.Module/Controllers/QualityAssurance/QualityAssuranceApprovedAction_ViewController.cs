using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Ultils.Helpers;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class QualityAssuranceApprovedAction_ViewController : ObjectViewController<ListView, QualityAssurance>
    {
        public QualityAssuranceApprovedAction_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction popupQualityAssuranceApprovedAction = new PopupWindowShowAction(this, "QualityAssuranceApprovedAction", PredefinedCategory.Unspecified);
            popupQualityAssuranceApprovedAction.ImageName = "ApplyChanges";
            popupQualityAssuranceApprovedAction.Caption = "Approve";
            popupQualityAssuranceApprovedAction.TargetObjectType = typeof(QualityAssurance);
            popupQualityAssuranceApprovedAction.TargetViewType = ViewType.ListView;
            popupQualityAssuranceApprovedAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            popupQualityAssuranceApprovedAction.Shortcut = "CtrlA";

            popupQualityAssuranceApprovedAction.CustomizePopupWindowParams +=
                PopupQualityAssuranceApprovedAction_CustomizePopupWindowParams;
            popupQualityAssuranceApprovedAction.Execute += PopupQualityAssuranceApprovedAction_Execute;
        }
        private void PopupQualityAssuranceApprovedAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = Application.CreateObjectSpace();
            var viewObject = View.CurrentObject as QualityAssurance;

            var model = objectSpace.CreateObject<QualityAssuranceApprovedPopupModel>();
            model.ID = viewObject.ID;
            model.PurchaseOrderNumber = viewObject.PurchaseOrderNumber;
            model.ItemStyleNumber = viewObject.ItemStyleNumber;
            model.GarmentSize=viewObject.GarmentSize;

            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.View = view;
        }
        private void PopupQualityAssuranceApprovedAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as QualityAssurance;
            var param = e.PopupWindowViewCurrentObject as QualityAssuranceApprovedPopupModel;
            var qualityStatus = ObjectSpace.GetObjects<QualityStatus>();

            if (param != null)
            {
                var connectionString = Application.ConnectionString ?? string.Empty;
                SqlParameter[] parameters =
                {
                    new SqlParameter("@ID",(long)(viewObject?.ID ?? 0)),
                    new SqlParameter("@PurchaseOrdernumber",viewObject?.PurchaseOrderNumber ?? string.Empty),
                    new SqlParameter("@ItemStyleNumber",viewObject?.ItemStyleNumber ?? string.Empty),
                    new SqlParameter("@GarmentSize",viewObject?.GarmentSize ?? string.Empty),
                    new SqlParameter("@StatusID",param?.Status?.ID ?? string.Empty),
                    new SqlParameter("@Percent",viewObject?.Percent ?? 0),
                    new SqlParameter("@Remark",param?.Remark ?? string.Empty),
                    new SqlParameter("@CustomerID",viewObject?.CustomerID ?? string.Empty),
                    new SqlParameter("@UserName", SecuritySystem.CurrentUserName),
                };

                DataTable table = SqlHelper.FillByReader_ItemMasterJob(connectionString, "sp_CreateUpdateQualityAssurance", parameters);
                foreach (DataRow dr in table.Rows)
                {
                    viewObject.ID = (long)dr["ID"];
                    viewObject.Remark = param?.Remark ?? string.Empty;
                    viewObject.QualityStatus = qualityStatus.FirstOrDefault(x => x.ID == param?.Status?.ID);
                }
            }
            
            View.Refresh(true);
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


