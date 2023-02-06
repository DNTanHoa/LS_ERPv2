using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Service;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class LoadCuttingOutputFromSearchParam_ViewController : ViewController
    {
        public LoadCuttingOutputFromSearchParam_ViewController()
        {
            InitializeComponent();

            SimpleAction searchCuttingOutputAction = new SimpleAction(this, "SearchCuttingOutput", PredefinedCategory.Unspecified);
            searchCuttingOutputAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchCuttingOutputAction.Caption = "Search (Ctrl + L)";
            searchCuttingOutputAction.TargetObjectType = typeof(CuttingOutputSearchParam);
            searchCuttingOutputAction.TargetViewType = ViewType.DetailView;
            searchCuttingOutputAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchCuttingOutputAction.Execute += SearchCuttingOutputAction_Execute;
        }

        private void SearchCuttingOutputAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var searchParam = View.CurrentObject as CuttingOutputSearchParam;

            if (searchParam != null)
            {
                var cuttingOutputService = new CuttingOutputService();

                var cuttingOutputDetails = cuttingOutputService.GetCuttingOutputReport(searchParam.Customer?.ID, searchParam.LSStyle, searchParam.FromDate, searchParam.ToDate);
                searchParam.CuttingOutputReports = cuttingOutputDetails.Result.Data.ToList();

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
}
