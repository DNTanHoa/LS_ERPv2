using DevExpress.ExpressApp;
using System;

namespace LS_ERP.XAF.Module.Controllers.DomainComponet
{
    public partial class PurchaserRequestSearchParam_WindowController : WindowController
    {
        public PurchaserRequestSearchParam_WindowController()
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
            if (e.ObjectType.IsAssignableFrom(c: typeof(PurchaseRequestSearchParam)))
            {
                if (((int)e.Key) == 0)
                {
                    PurchaseRequestSearchParam defaultObject = new PurchaseRequestSearchParam();
                    defaultObject.ID = 0;
                    defaultObject.FromDate = DateTime.Today.AddDays(1).AddYears(-1);
                    defaultObject.ToDate = DateTime.Today.AddDays(1);
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
