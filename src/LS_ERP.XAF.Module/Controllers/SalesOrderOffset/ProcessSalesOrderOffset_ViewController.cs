using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using System;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ProcessSalesOrderOffset_ViewController : ViewController
    {
        public ProcessSalesOrderOffset_ViewController()
        {
            InitializeComponent();

            SimpleAction processSalesOrderOffset = new SimpleAction(this, "ProcessSalesOrderOffset", PredefinedCategory.Unspecified);
            processSalesOrderOffset.ImageName = "AlignmentBottomLeft";
            processSalesOrderOffset.Caption = "Process (Ctrl + Shift + P)";
            processSalesOrderOffset.TargetObjectType = typeof(SalesOrderOffset);
            processSalesOrderOffset.TargetViewType = ViewType.ListView;
            processSalesOrderOffset.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            processSalesOrderOffset.Shortcut = "CtrlShiftP";

            processSalesOrderOffset.Execute += ProcessSalesOrderOffset_Execute;
        }

        private void ProcessSalesOrderOffset_Execute(object sender, SimpleActionExecuteEventArgs e)
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
