using DevExpress.ExpressApp;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class IssuedSearchParam_WindowController : WindowController
    {
        public IssuedSearchParam_WindowController()
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
            if (e.ObjectType.IsAssignableFrom(typeof(IssuedSearchParam)))
            {
                if (((int)e.Key) == 0)
                {
                    IssuedSearchParam defaultObject = new IssuedSearchParam();
                    defaultObject.ID = 0;
                    defaultObject.FromDate = DateTime.Today.AddDays(1).AddYears(-1);
                    defaultObject.ToDate = DateTime.Today.AddDays(1);
                    defaultObject.Issueds = new List<EntityFrameworkCore.Entities.Issued>();
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
