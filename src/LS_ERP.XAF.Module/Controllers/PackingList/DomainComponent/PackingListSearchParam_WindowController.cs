using DevExpress.ExpressApp;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class PackingListSearchParam_WindowController : WindowController
    {
        public PackingListSearchParam_WindowController()
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
            if (e.ObjectType.IsAssignableFrom(typeof(PackingListSearchParam)))
            {
                if (((int)e.Key) == 0)
                {
                    PackingListSearchParam defaultObject = new PackingListSearchParam();
                    defaultObject.ID = 0;
                    defaultObject.PackingFromDate = DateTime.Today.AddDays(1).AddMonths(-3);
                    defaultObject.PackingToDate = DateTime.Today.AddDays(1);
                    defaultObject.PackingLists = new List<EntityFrameworkCore.Entities.PackingList>();
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
