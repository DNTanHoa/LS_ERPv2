using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Ultils.Extensions;
using Ultils.Helpers;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ExportOnhandTransactionByLocation_ViewController : ObjectViewController<DetailView, StorageDetailReportParam>
    {
        public ExportOnhandTransactionByLocation_ViewController()
        {
            InitializeComponent();            

            PopupWindowShowAction loacdPackingList = new PopupWindowShowAction(this,
                "ExportOnhandByLocation", PredefinedCategory.Unspecified);
            loacdPackingList.ImageName = "Export";
            loacdPackingList.Caption = "Export Onhand By Location";
            loacdPackingList.TargetObjectType = typeof(StorageDetailReportParam);
            loacdPackingList.TargetViewType = ViewType.DetailView;
            loacdPackingList.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
           

            loacdPackingList.CustomizePopupWindowParams += ExportOnhandByLocation_CustomizePopupWindowParams;
            loacdPackingList.Execute += ExportOnhandByLocation_Execute;
        }

        public virtual void ExportOnhandByLocation_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            
        }

        private void ExportOnhandByLocation_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var viewObject = View.CurrentObject as StorageDetailReportParam;
           
            var model = new ExportOnhandTransactionByLocationParam();
            if(viewObject != null)
            {               
                //if(viewObject?.Customer != null && viewObject?.Storage != null)
                //{
                    var connectionString = Application.ConnectionString ?? string.Empty;
                    SqlParameter[] parameters =
                    {
                                new SqlParameter("@CustomerID",viewObject?.Customer?.ID ?? string.Empty),
                                new SqlParameter("@StorageCode",viewObject?.Storage?.Code ?? string.Empty)
                            };
                    DataTable table = SqlHelper.FillByReader(connectionString, "sp_LoadStorageBinCode", parameters);
                    var locations = table.AsListObject<StorageBinCodeModel>();
                    model.StorageBinCodes = locations.OrderBy(o=>o.StorageBinCode).ToList();
                    var view = Application.CreateDetailView(objectSpace, model, false);
                    e.DialogController.SaveOnAccept = false;
                    e.Maximized = true;
                    e.View = view;
                //}
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
