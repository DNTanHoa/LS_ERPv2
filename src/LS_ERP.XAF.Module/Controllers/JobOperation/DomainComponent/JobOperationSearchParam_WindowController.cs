using DevExpress.ExpressApp;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class JobOperationSearchParam_WindowController : WindowController
    {
        public JobOperationSearchParam_WindowController()
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
            if (e.ObjectType.IsAssignableFrom(typeof(JobOperationSearchParam)))
            {
                if (((int)e.Key) == 0)
                {
                    var objectSpace = Application.CreateObjectSpace(typeof(JobOperation));
                    JobOperationSearchParam defaultObject = new JobOperationSearchParam();
                    defaultObject.ID = 0;
                    defaultObject.Operation = objectSpace.GetObjects<Operation>()
                       .OrderBy(x => x.Index).FirstOrDefault();
                    defaultObject.FromDate = DateTime.Today.AddMonths(-2);
                    defaultObject.ToDate = DateTime.Today.AddMonths(2);
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
