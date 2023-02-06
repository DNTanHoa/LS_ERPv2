using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ResetRemainQuantity_ViewController : ObjectViewController<ListView, PackingOverQuantity>
    {
        public ResetRemainQuantity_ViewController()
        {
            InitializeComponent();

            SimpleAction resetRemainQuantityAction = new SimpleAction(this, "ResetRemainQuantityAction", PredefinedCategory.Unspecified);
            resetRemainQuantityAction.ImageName = "Reset";
            resetRemainQuantityAction.Caption = "Reset Quantity";
            resetRemainQuantityAction.TargetObjectType = typeof(PackingOverQuantity);
            resetRemainQuantityAction.TargetViewType = ViewType.ListView;
            resetRemainQuantityAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            resetRemainQuantityAction.Shortcut = "CtrlR";
            resetRemainQuantityAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;

            resetRemainQuantityAction.Execute += ResetRemainQuantityAction_Execute;
        }

        private void ResetRemainQuantityAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var objectSpace = Application.CreateObjectSpace(typeof(PackingOverQuantity));
            PropertyCollectionSource collectionSource = (View as ListView).CollectionSource as PropertyCollectionSource;
            var itemStyle = collectionSource.MasterObject as ItemStyle;
            var failSheetID= ObjectSpace.GetObjects<PackingSheetName>()
                    .FirstOrDefault(x => x.SheetName.ToUpper().Contains("FAIL")).ID;

            /// Check delete all of packing list before
            var packingLists = new List<PackingList>();
            var tableName = "";
            var joinTableName = "";
            var filter = "";

            if (string.IsNullOrEmpty(tableName))
                tableName = typeof(PackingList).Name;

            if (string.IsNullOrEmpty(joinTableName))
                joinTableName = typeof(PackingLine).Name;

            var connectString = Application.ConnectionString ?? string.Empty;
            using (var db = new QueryFactory(
                new SqlConnection(connectString), new SqlServerCompiler()))
            {
                    filter = "[ID] IN ( SELECT [PACKINGLISTID] FROM " + joinTableName + " WHERE [LSSTYLE] IN ('" + itemStyle?.LSStyle + "'))";

                    packingLists = db.Query(tableName)
                            .WhereRaw(filter).Get<PackingList>().ToList();

            } 


            //var criteria = CriteriaOperator.Parse("ID IN ([LSStyle] IN ('" + itemStyle.LSStyle + "'))");
            //packingLists = ObjectSpace.GetObjects<PackingList>(criteria).ToList();
            if(packingLists.Where(x => (x.SheetNameID ?? 0) != failSheetID).Count() > 0)
            {
                var error = Message.GetMessageOptions("Please delete or update fail all of packing list before", "Error",
                    InformationType.Error, null, 5000);
                Application.ShowViewStrategy.ShowMessage(error);
                return;
            }

            var criteria = CriteriaOperator.Parse("[ItemStyleNumber] IN ('" + itemStyle.Number + "')");
            var packingOverQuantities = objectSpace.GetObjects<PackingOverQuantity>(criteria).ToList();
            foreach (var orderDetail in itemStyle.OrderDetails)
            {
                var packingOverQuantity = packingOverQuantities.First(x => x.Size == orderDetail.Size);
                packingOverQuantity.Quantity = orderDetail.ShipQuantity;
                packingOverQuantity.SetUpdateAudit(SecuritySystem.CurrentUserName);

                var currentPackingOverQuantity = itemStyle.PackingOverQuantities.FirstOrDefault(x => x.Size == orderDetail.Size);
                currentPackingOverQuantity.Quantity = orderDetail.ShipQuantity;
            }

            objectSpace.CommitChanges();
            //ObjectSpace.Refresh();
            View.Refresh(true);
            
        }
        protected override void OnActivated()
        {
            base.OnActivated();
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
