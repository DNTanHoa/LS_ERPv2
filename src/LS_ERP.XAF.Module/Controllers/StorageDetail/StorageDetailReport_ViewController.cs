using AutoMapper;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Service.Response;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class StorageDetailReport_ViewController : ObjectViewController<DetailView, StorageDetailReportParam>
    {
        public StorageDetailReport_ViewController()
        {
            InitializeComponent();

            SimpleAction loadReceiptReportAction = new SimpleAction(this, "LoadStorageDetailReport", PredefinedCategory.Unspecified);
            loadReceiptReportAction.ImageName = "Action_Search_Object_FindObjectByID";
            loadReceiptReportAction.Caption = "Search (Ctrl + L)";
            //loadReceiptReportAction.TargetObjectType = typeof(ReceiptReportParam);
            //loadReceiptReportAction.TargetViewType = ViewType.DetailView;
            loadReceiptReportAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadReceiptReportAction.Shortcut = "CtrlL";

            loadReceiptReportAction.Execute += LoadStorageDetailReportAction_Execute;
        }

        private void LoadStorageDetailReportAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as StorageDetailReportParam;
            var service = new StorageDetailService();

            var onHandQuantity = 0;

            if (!viewObject.OnHandQuantityZero)
            {
                onHandQuantity = -1;
            }

            var response = service.GetReport(viewObject.Customer?.ID ?? string.Empty, viewObject.Storage?.Code,
                viewObject.FromDate, viewObject.ToDate, viewObject.ProductionMethodCode?.Code, onHandQuantity).Result;

            if (response.IsSuccess)
            {
                var mapperConfig = new MapperConfiguration(c =>
                {
                    c.CreateMap<StorageDetailReportResponseData, StorageDetailsReport>();
                });

                var mapper = mapperConfig.CreateMapper();

                viewObject.Details = response.Data
                    .Select(x => mapper.Map<StorageDetailsReport>(x)).ToList();
                View.Refresh();
            }
            else
            {
                var message = Message.GetMessageOptions(response.ErrorMessage, "Error",
                    InformationType.Error, null, 5000);
                Application.ShowViewStrategy.ShowMessage(message);
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
