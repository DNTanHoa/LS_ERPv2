using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class DailyFinishGoodReceiptParam_WindowController : WindowController
    {
        public DailyFinishGoodReceiptParam_WindowController()
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
            if (e.ObjectType.IsAssignableFrom(typeof(DailyFinishGoodReceiptParam)))
            {
                if (((int)e.Key) == 0)
                {
                    var objectSpace = Application.CreateObjectSpace(typeof(Storage));
                    var criteria = CriteriaOperator.Parse("[ReceiptDate] <= ? AND [ReceiptDate] >= ? AND ReceiptTypeId = 'RFG'", 
                        DateTime.Now, DateTime.Today);

                    DailyFinishGoodReceiptParam defaultObject = objectSpace
                        .CreateObject<DailyFinishGoodReceiptParam>();
                    defaultObject.ID = 0;
                    defaultObject.Storage = objectSpace.GetObjects<Storage>().FirstOrDefault();
                    defaultObject.Customer = objectSpace.GetObjects<Customer>().FirstOrDefault();
                    defaultObject.ReceiptDate = DateTime.Today;
                    defaultObject.Receipts = objectSpace.GetObjects<Receipt>(criteria).ToList();

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
