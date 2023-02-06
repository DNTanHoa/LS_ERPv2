using DevExpress.ExpressApp;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.Report.DomainComponent
{
    public partial class InventoryReportParam_WindowController : WindowController
    {
        public InventoryReportParam_WindowController()
        {
            InitializeComponent();
        }

        private void Application_ObjectSpaceCreated(object sender, ObjectSpaceCreatedEventArgs e)
        {
            NonPersistentObjectSpace nonPersistentObjectSpace = e.ObjectSpace 
                as NonPersistentObjectSpace;
            if (nonPersistentObjectSpace != null)
            {
                nonPersistentObjectSpace.ObjectByKeyGetting += 
                    nonPersistentObjectSpace_ObjectByKeyGetting;
            }
        }

        private void nonPersistentObjectSpace_ObjectByKeyGetting(object sender, 
            ObjectByKeyGettingEventArgs e)
        {
            if (e.ObjectType.IsAssignableFrom(typeof(InventoryReportParam)))
            {
                if (((int)e.Key) == 0)
                {
                    var objectSpace = Application.CreateObjectSpace(typeof(Storage));
                    InventoryReportParam defaultObject = objectSpace
                        .CreateObject<InventoryReportParam>();
                    defaultObject.ID = 0;
                    defaultObject.Customer = objectSpace.GetObjects<Customer>().First();
                    defaultObject.Storage = objectSpace.GetObjects<Storage>().First();
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
