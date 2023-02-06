using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Controls;
using LS_ERP.EntityFrameworkCore.Entities;
using System;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class TriggerEdit_ViewController : ObjectViewController<DetailView, PurchaseOrder>
    {
        public TriggerEdit_ViewController()
        {
            InitializeComponent();

        }
        protected override void OnActivated()
        {
            base.OnActivated();
            View.CustomizeViewItemControl(this, SetTrigger, nameof(PurchaseOrder.Vendor));
        }

        private void SetTrigger(ViewItem viewItem)
        {
            try
            {
                var lookupEdit = viewItem.Control as LookupEdit;
                lookupEdit.EditValueChanged += LookupEdit_EditValueChanged;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        private void LookupEdit_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if(View == null)
                {
                    return;
                }    
                var purchaseOrder = View.CurrentObject as PurchaseOrder;
                var evn = e as ChangingEventArgs;
                if (evn != null)
                {
                    var vendor = evn.NewValue as Vendor;
                    if (vendor != null &&
                        purchaseOrder.Currency == null)
                    {
                        purchaseOrder.Currency = vendor.Currency;
                        if (vendor.CurrencyID == "VND")
                        {
                            purchaseOrder.Tax = ObjectSpace.GetObjectByKey<Tax>(vendor.TaxCode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
