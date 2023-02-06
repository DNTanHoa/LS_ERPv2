using DevExpress.ExpressApp;
using LS_ERP.XAF.Module.DomainComponent;
using System;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class CuttingOutputSearchParam_WindowController : WindowController
    {
        public CuttingOutputSearchParam_WindowController()
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
            if (e.ObjectType.IsAssignableFrom(typeof(CuttingOutputSearchParam)))
            {
                if (((int)e.Key) == 0)
                {
                    CuttingOutputSearchParam defaultObject = new CuttingOutputSearchParam();
                    defaultObject.ID = 0;
                    defaultObject.FromDate = DateTime.Today.AddDays(-1);
                    defaultObject.ToDate = DateTime.Today.AddMonths(1);
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
