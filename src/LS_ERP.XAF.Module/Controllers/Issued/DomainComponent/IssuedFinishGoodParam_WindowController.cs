using DevExpress.ExpressApp;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class IssuedFinishGoodParam_WindowController : WindowController
    {
        public IssuedFinishGoodParam_WindowController()
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
            if (e.ObjectType.IsAssignableFrom(typeof(IssuedFinishGoodParam)))
            {
                if (((int)e.Key) == 0)
                {
                    var objectSpace = Application.CreateObjectSpace(typeof(IssuedFinishGoodParam));

                    IssuedFinishGoodParam defaultObject = new IssuedFinishGoodParam();
                    defaultObject.ID = 0;
                    defaultObject.Storage = objectSpace.GetObjects<Storage>().FirstOrDefault();
                    defaultObject.Customer = objectSpace.GetObjects<Customer>().FirstOrDefault();
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
