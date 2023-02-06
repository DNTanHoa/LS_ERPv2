using DevExpress.ExpressApp;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class IssuedReportParam_WindowController : WindowController
    {
        public IssuedReportParam_WindowController()
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
            if (e.ObjectType.IsAssignableFrom(typeof(IssuedReportParam)))
            {
                if (((int)e.Key) == 0)
                {
                    var objectSpace = Application.CreateObjectSpace(typeof(Storage));

                    IssuedReportParam defaultObject = new IssuedReportParam();
                    defaultObject.ID = 0;
                    //defaultObject.Storage = objectSpace.GetObjects<Storage>().FirstOrDefault();
                    defaultObject.FromDate = DateTime.Today.AddDays(1).AddMonths(-6);
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
