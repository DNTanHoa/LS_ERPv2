using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using System;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class JobOutputEntryAction_ViewController : ViewController
    {
        public JobOutputEntryAction_ViewController()
        {
            InitializeComponent();

            SimpleAction loadJobOperationToEntry = new SimpleAction(this, "LoadJobOperationToEntry", PredefinedCategory.Unspecified);
            loadJobOperationToEntry.ImageName = "GridResetColumnWidths";
            loadJobOperationToEntry.Caption = "Load Operation (Ctrl + L)";
            loadJobOperationToEntry.TargetObjectType = typeof(JobOutPutEntryParam);
            loadJobOperationToEntry.TargetViewType = ViewType.DetailView;
            loadJobOperationToEntry.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadJobOperationToEntry.Shortcut = "CtrlL";

            loadJobOperationToEntry.Execute += LoadJobOperationToEntry_Execute;

            SimpleAction copyJobOperationToInput = new SimpleAction(this, "CopyJobOperationToInput", PredefinedCategory.Unspecified);
            copyJobOperationToInput.ImageName = "BO_Transition";
            copyJobOperationToInput.Caption = "Entry (Ctrl + E)";
            copyJobOperationToInput.TargetObjectType = typeof(JobOutPutEntryParam);
            copyJobOperationToInput.TargetViewType = ViewType.DetailView;
            copyJobOperationToInput.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            copyJobOperationToInput.Shortcut = "CtrlE";

            copyJobOperationToInput.Execute += CopyJobOperationToInput_Execute;

            SimpleAction submitJobOutput = new SimpleAction(this, "SubmitJobOutput", PredefinedCategory.Unspecified);
            submitJobOutput.ImageName = "Actions_Send";
            submitJobOutput.Caption = "Submit (Ctrl + I)";
            submitJobOutput.TargetObjectType = typeof(JobOutPutEntryParam);
            submitJobOutput.TargetViewType = ViewType.DetailView;
            submitJobOutput.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            submitJobOutput.Shortcut = "CtrlI";

            submitJobOutput.Execute += SubmitJobOutput_Execute;
        }

        private void SubmitJobOutput_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CopyJobOperationToInput_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void LoadJobOperationToEntry_Execute(object sender, SimpleActionExecuteEventArgs e)
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
