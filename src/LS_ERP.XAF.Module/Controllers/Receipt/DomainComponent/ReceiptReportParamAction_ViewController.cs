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

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class ReceiptReportParamAction_ViewController : ViewController
    {
        public ReceiptReportParamAction_ViewController()
        {
            InitializeComponent();

            SimpleAction loadReceiptReportAction = new SimpleAction(this, "LoadReceiptReport", PredefinedCategory.Unspecified);
            loadReceiptReportAction.ImageName = "Action_Search_Object_FindObjectByID";
            loadReceiptReportAction.Caption = "Search (Ctrl + L)";
            loadReceiptReportAction.TargetObjectType = typeof(ReceiptReportParam);
            loadReceiptReportAction.TargetViewType = ViewType.DetailView;
            loadReceiptReportAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadReceiptReportAction.Shortcut = "CtrlL";

            loadReceiptReportAction.Execute += LoadReceiptReportAction_Execute;
        }

        private void LoadReceiptReportAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as ReceiptReportParam;
            var service = new ReceiptService();
            var response = service.GetReport(viewObject.ReceiptNumber ?? string.Empty, viewObject.Storage?.Code,
                viewObject.FromDate, viewObject.ToDate).Result;

            if (response.IsSuccess)
            {
                var mapperConfig = new MapperConfiguration(c =>
                {
                    c.CreateMap<ReceiptReportResponseData, ReceiptReportDetail>();
                });

                var mapper = mapperConfig.CreateMapper();

                viewObject.Details = response.Data
                    .Select(x => mapper.Map<ReceiptReportDetail>(x)).ToList();
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
