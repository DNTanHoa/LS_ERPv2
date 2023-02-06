using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Process;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class DeletePackingList_ViewController : ObjectViewController<ListView, PackingList>
    {
        public DeletePackingList_ViewController()
        {
            InitializeComponent();

            SimpleAction deletePKInvoice = new SimpleAction(this, "DeletePKInvoice", PredefinedCategory.Unspecified);
            deletePKInvoice.ImageName = "Delete";
            deletePKInvoice.Caption = "Delete PL of Invoice";
            //deletePKInvoice.TargetObjectType = typeof(InvoiceDocumentParam);
            //deletePKInvoice.TargetViewType = ViewType.DetailView;
            deletePKInvoice.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            deletePKInvoice.Execute += BrowserDeletePKInvoice_Execute;

        }

        public virtual void BrowserDeletePKInvoice_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var proObj = (View.CollectionSource) as PropertyCollectionSource;
            var invoice = proObj.MasterObject as Invoice;

            List<PackingList> deletePackingList = new List<PackingList>();

            foreach (PackingList item in e.SelectedObjects)
            {
                invoice.PackingList.Remove(item);
                deletePackingList.Add(item);
                item.SetUpdateAudit(SecuritySystem.CurrentUserName);
            }

            InvoiceProcessor.DeleteInvoiceDetail(invoice, deletePackingList, SecuritySystem.CurrentUserName, out string errorMessage);

            invoice.SetUpdateAudit(SecuritySystem.CurrentUserName);
            objectSpace.CommitChanges();
            objectSpace.Refresh();
            objectSpace.ReloadObject(invoice);
            //proObj.Reload();
            View.Refresh();
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            if (View.Id != "Invoice_PackingList_ListView")
            {
                Frame.GetController<DeletePackingList_ViewController>().Active["1"] = false;
            }

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
