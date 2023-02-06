using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Controls;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class TriggerChooseFabric_ViewController : ObjectViewController<DetailView, FabricPopupModel>
    {
        public TriggerChooseFabric_ViewController()
        {
            InitializeComponent();
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            View.CustomizeViewItemControl(this, SetTrigger, nameof(FabricPopupModel.FabricPurchaseOrder));
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
                if (View == null)
                {
                    return;
                }
                var popup = View.CurrentObject as FabricPopupModel;
                var evn = e as ChangingEventArgs;
                if (evn != null)
                {
                    var fabric = evn.NewValue as FabricPurchaseOrder;
                    if (fabric != null)
                    {
                        popup.CustomerStyle = fabric.CustomerStyles;
                        popup.Season = fabric.Seasons;
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
