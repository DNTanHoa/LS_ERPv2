using DevExpress.ExpressApp;
using LS_ERP.XAF.Module.DomainComponent;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class PartMasterSearchParam_WindowController : WindowController
    {
        public PartMasterSearchParam_WindowController()
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
            if (e.ObjectType.IsAssignableFrom(typeof(PartMasterSearchParam)))
            {
                if (((int)e.Key) == 0)
                {
                    PartMasterSearchParam defaultObject = new PartMasterSearchParam();
                    defaultObject.ID = 0;
                    defaultObject.Search = string.Empty;
                    defaultObject.PartMasters = new List<EntityFrameworkCore.Entities.PartMaster>();
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
