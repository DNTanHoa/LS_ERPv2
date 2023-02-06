using DevExpress.ExpressApp;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class InvoiceInit_ViewController : ObjectViewController<DetailView, Invoice>
    {
        public InvoiceInit_ViewController()
        {
            InitializeComponent();
        }

        protected override void OnActivated()
        {
            base.OnActivated();

        }
        protected override void OnViewControlsCreated()
        {
            var invoice = View.CurrentObject as Invoice;

            if (invoice != null)
            {
                if (invoice.Company == null || invoice.InvoiceType == null)
                {
                    invoice.Company = ObjectSpace.GetObjects<Company>().FirstOrDefault(x => x.Code == "LS");
                    invoice.InvoiceType = ObjectSpace.FirstOrDefault<InvoiceType>(x => x.ID == 2);
                    invoice.SetCreateAudit(SecuritySystem.CurrentUserName);
                }
            }

            base.OnViewControlsCreated();
        }
        protected override void OnDeactivated()
        {

            base.OnDeactivated();
        }
    }
}
