using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using System;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ForecastReportAction_ViewController 
        : ObjectViewController<DetailView, ForecastReportParam>
    {
        public ForecastReportAction_ViewController()
        {
            InitializeComponent();

            SimpleAction loadForecastReport = new SimpleAction(this, "LoadForecastReport", PredefinedCategory.Unspecified);
            loadForecastReport.ImageName = "Action_Search_Object_FindObjectByID";
            loadForecastReport.TargetObjectType = typeof(ForecastReportParam);
            loadForecastReport.TargetViewType = ViewType.DetailView;
            loadForecastReport.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadForecastReport.Caption = "Load (Ctrl + L)";
            loadForecastReport.Shortcut = "CtrlL";

            loadForecastReport.Execute += LoadForecastReport_Execute;
        }

        private void LoadForecastReport_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            throw new NotImplementedException();
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
