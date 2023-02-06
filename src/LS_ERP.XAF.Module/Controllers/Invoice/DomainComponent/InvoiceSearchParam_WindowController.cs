using DevExpress.ExpressApp;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class InvoiceSearchParam_WindowController : WindowController
    {
        public InvoiceSearchParam_WindowController()
        {
            InitializeComponent();
            TargetWindowType = WindowType.Main;
        }

        private void Application_ObjectSpaceCreated(object sender, ObjectSpaceCreatedEventArgs e)
        {
            NonPersistentObjectSpace nonPersistentObjectSpace = e.ObjectSpace as NonPersistentObjectSpace;
            if (nonPersistentObjectSpace != null)
            {
                nonPersistentObjectSpace.ObjectByKeyGetting += nonPersistentObjectSpace_ObjectByKeyGetting;
            }
        }

        private void nonPersistentObjectSpace_ObjectByKeyGetting(object sender, ObjectByKeyGettingEventArgs e)
        {
            if (e.ObjectType.IsAssignableFrom(typeof(InvoiceSearchParam)))
            {
                if (((int)e.Key) == 0)
                {
                    InvoiceSearchParam defaultObject = new InvoiceSearchParam();
                    defaultObject.ID = 0;
                    defaultObject.OrderFromDate = DateTime.Today.AddDays(1).AddYears(-1);
                    defaultObject.OrderToDate = DateTime.Today.AddDays(1);
                    defaultObject.Invoices = new List<EntityFrameworkCore.Entities.Invoice>();
                    e.Object = defaultObject;
                }
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            Application.ObjectSpaceCreated += Application_ObjectSpaceCreated;
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            Application.ObjectSpaceCreated -= Application_ObjectSpaceCreated;
        }
    }
}
