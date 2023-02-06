using DevExpress.ExpressApp;
using LS_ERP.XAF.Module.DomainComponent;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class LoadingPlanSearchParam_WindowController : WindowController
    {
        public LoadingPlanSearchParam_WindowController()
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
            if (e.ObjectType.IsAssignableFrom(typeof(LoadingPlanSearchParam)))
            {
                if (((int)e.Key) == 0)
                {
                    LoadingPlanSearchParam defaultObject = new LoadingPlanSearchParam();
                    defaultObject.ID = 0;
                    defaultObject.OrderNumber = string.Empty;
                    defaultObject.ContainerNumber = string.Empty;
                    defaultObject.LoadingPlans = new List<EntityFrameworkCore.Entities.LoadingPlan>();
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
