using DevExpress.ExpressApp;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class FabricPurchaseOrderSearchParam_WindowController : WindowController
    {
        public FabricPurchaseOrderSearchParam_WindowController()
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
            if (e.ObjectType.IsAssignableFrom(typeof(FabricPurchaseOrderSearchParam)))
            {
                if (((int)e.Key) == 0)
                {
                    FabricPurchaseOrderSearchParam defaultObject = new FabricPurchaseOrderSearchParam();
                    defaultObject.ID = 0;
                    defaultObject.OrderFromDate = DateTime.Today.AddDays(1).AddYears(-1);
                    defaultObject.OrderToDate = DateTime.Today.AddDays(1);
                    defaultObject.FabricPurchaseOrders = new List<EntityFrameworkCore.Entities.FabricPurchaseOrder>();
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
