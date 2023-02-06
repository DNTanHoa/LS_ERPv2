using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class DownloadDocument_ViewController : ObjectViewController<ListView, InvoiceDocument>
    {
        public DownloadDocument_ViewController()
        {
            InitializeComponent();
            SimpleAction downLoadDocument = new SimpleAction(this, "DownloadInvoiceDocument", PredefinedCategory.Unspecified);
            downLoadDocument.ImageName = "Update";
            downLoadDocument.Caption = "Download Document";
            //updateInvoice.TargetObjectType = typeof(Invoice);
            //updateInvoice.TargetViewType = ViewType.DetailView;
            downLoadDocument.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            //exportSalesContract.Shortcut = "CtrlShiftP";
            downLoadDocument.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;

            downLoadDocument.Execute += DownloadDocument_Execute;
        }

        public virtual void DownloadDocument_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

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
