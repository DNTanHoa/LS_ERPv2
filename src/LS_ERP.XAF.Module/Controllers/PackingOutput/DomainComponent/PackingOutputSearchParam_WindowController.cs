using DevExpress.ExpressApp;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class PackingOutputSearchParam_WindowController : WindowController
    {
        public PackingOutputSearchParam_WindowController()
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
            if (e.ObjectType.IsAssignableFrom(typeof(PackingOutputSearchParam)))
            {
                var objectSpace = Application.CreateObjectSpace(typeof(PackingOutput));
                if (((int)e.Key) == 0)
                {
                    PackingOutputSearchParam defaultObject = 
                        objectSpace.CreateObject<PackingOutputSearchParam>();
                    defaultObject.ID = 0;
                    defaultObject.Customer = objectSpace.GetObjects<Customer>().FirstOrDefault();
                    defaultObject.FromDate = DateTime.Today.AddDays(1).AddYears(-1);
                    defaultObject.ToDate = DateTime.Today.AddDays(1);
                    e.Object = defaultObject;
                }
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
