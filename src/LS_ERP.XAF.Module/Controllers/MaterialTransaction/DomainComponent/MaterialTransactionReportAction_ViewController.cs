using AutoMapper;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Service.Response;
using System;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class MaterialTransactionReportAction_ViewController : ViewController
    {
        public MaterialTransactionReportAction_ViewController()
        {
            InitializeComponent();

            SimpleAction searchMaterialTransactionReportAction = new SimpleAction(this, "SearchMaterialTransaction", PredefinedCategory.Unspecified);
            searchMaterialTransactionReportAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchMaterialTransactionReportAction.Caption = "Search (Ctrl + L)";
            searchMaterialTransactionReportAction.TargetObjectType = typeof(MaterialTransactionReportParam);
            searchMaterialTransactionReportAction.TargetViewType = ViewType.DetailView;
            searchMaterialTransactionReportAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchMaterialTransactionReportAction.Shortcut = "CtrlL";

            searchMaterialTransactionReportAction.Execute += SearchMaterialTransactionReportAction_Execute;
        }

        private void SearchMaterialTransactionReportAction_Execute(object sender,
            SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as MaterialTransactionReportParam;
            var service = new MaterialTransactionService();
            var response = service.GetSummaryReport(viewObject.Storage?.Code,
                viewObject.FromDate ?? DateTime.Today, viewObject.ToDate ?? DateTime.Now).Result;

            if(response.IsSuccess)
            {
                var mapperConfig = new MapperConfiguration(c => 
                {
                    c.CreateMap<MaterialTransactionReportData, MaterialTransactionReportDetail>();
                });

                var mapper = mapperConfig.CreateMapper();

                viewObject.Details = response.Data
                    .Select(x => mapper.Map<MaterialTransactionReportDetail>(x)).ToList();
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
