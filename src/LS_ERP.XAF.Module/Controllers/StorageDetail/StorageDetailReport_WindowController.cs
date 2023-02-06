using DevExpress.ExpressApp;
using LS_ERP.XAF.Module.DomainComponent;
using System;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class StorageDetailReport_WindowController : WindowController
    {
        public StorageDetailReport_WindowController()
        {
            InitializeComponent();

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
            if (e.ObjectType.IsAssignableFrom(typeof(StorageDetailReportParam)))
            {
                if (((int)e.Key) == 0)
                {
                    //var objectSpace = Application.CreateObjectSpace(typeof(StorageDetail));

                    StorageDetailReportParam defaultObject = new StorageDetailReportParam();
                    defaultObject.ID = 0;
                    //defaultObject.Customer = objectSpace.GetObjects<Customer>().FirstOrDefault();
                    //defaultObject.Storage = objectSpace.GetObjects<Storage>().FirstOrDefault();
                    //defaultObject.FromDate = DateTime.Today.AddDays(1).AddMonths(-6);
                    defaultObject.FromDate = DateTime.Parse("01-01-2016");
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
