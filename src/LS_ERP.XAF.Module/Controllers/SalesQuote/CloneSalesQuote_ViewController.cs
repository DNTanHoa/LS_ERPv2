using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class CloneSalesQuote_ViewController : ViewController
    {
        public CloneSalesQuote_ViewController()
        {
            InitializeComponent();

            SimpleAction cloneSalesQuote = new SimpleAction(this,
                "CloneSalesQuote",
                PredefinedCategory.Unspecified);
            cloneSalesQuote.ImageName = "Action_CloneMerge_Clone_Object";
            cloneSalesQuote.Caption = "Clone To Local";
            cloneSalesQuote.TargetObjectType = typeof(SalesQuote);
            cloneSalesQuote.TargetViewType = ViewType.Any;
            cloneSalesQuote.PaintStyle =
                ActionItemPaintStyle.CaptionAndImage;
            cloneSalesQuote.SelectionDependencyType =
                SelectionDependencyType.RequireSingleObject;

            cloneSalesQuote.Execute += CloneSalesQuote_Execute;
        }

        private void CloneSalesQuote_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var salesQuote = View.CurrentObject as SalesQuote;
            salesQuote.Level = 1;
            ObjectSpace.CommitChanges();
            View.Refresh();
            var messageOptions = Message.GetMessageOptions("Clone successfully", "Success",
                InformationType.Success, null, 5000);
            Application.ShowViewStrategy.ShowMessage(messageOptions);
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
