using DevExpress.ExpressApp;
using LS_ERP.XAF.Module.DomainComponent;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class BoxInfoSearchParam_WindowController : WindowController
    {
        public BoxInfoSearchParam_WindowController()
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
            if (e.ObjectType.IsAssignableFrom(typeof(BoxInfoSearchParam)))
            {
                if (((int)e.Key) == 0)
                {
                    BoxInfoSearchParam defaultObject = new BoxInfoSearchParam();
                    defaultObject.ID = 0;
                    defaultObject.Search = string.Empty;
                    defaultObject.BoxInfos = new List<EntityFrameworkCore.Entities.BoxInfo>();
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
