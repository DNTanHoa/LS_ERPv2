using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class DeleteNotifyParty_ViewController : ObjectViewController<ListView, Consignee>
    {
        public DeleteNotifyParty_ViewController()
        {
            InitializeComponent();

            SimpleAction deleteConsigneeInvoice = new SimpleAction(this, "DeleteConsigneeInvoice", PredefinedCategory.Unspecified);
            deleteConsigneeInvoice.ImageName = "Delete";
            deleteConsigneeInvoice.Caption = "Delete Consignee of Invoice";
            //deleteConsigneeInvoice.TargetObjectType = typeof(InvoiceDocumentParam);
            //deleteConsigneeInvoice.TargetViewType = ViewType.DetailView;
            deleteConsigneeInvoice.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            deleteConsigneeInvoice.Execute += BrowserDeleteConsigneeInvoice_Execute;
        }

        public virtual void BrowserDeleteConsigneeInvoice_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var proObj = (View.CollectionSource) as PropertyCollectionSource;
            var invoice = proObj.MasterObject as Invoice;

            var notifyRemove = new List<Consignee>();

            foreach (Consignee item in e.SelectedObjects)
            {
                foreach (Consignee notify in invoice.NotifyParties)
                {
                    if (notify.ID == item.ID)
                    {
                        notifyRemove.Add(notify);
                    }
                }
            }

            foreach (Consignee notify in notifyRemove)
            {
                invoice.NotifyParties.Remove(notify);
            }

            ObjectSpace.CommitChanges();
            proObj.Reload();
            View.Refresh();
        }


        protected override void OnActivated()
        {
            base.OnActivated();

            if (View.Id != "Invoice_NotifyParties_ListView")
            {
                Frame.GetController<DeleteNotifyParty_ViewController>().Active["1"] = false;
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
