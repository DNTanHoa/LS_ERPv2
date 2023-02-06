using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ExchangePackingLine_ViewController 
        : ObjectViewController<ListView, PackingLine>
    {
        public ExchangePackingLine_ViewController()
        {
            InitializeComponent();

            SimpleAction exchangePackingLineAction = new SimpleAction(this, "ExchangePackingLineAction", PredefinedCategory.Unspecified);
            exchangePackingLineAction.ImageName = "Action_Search_Object_FindObjectByID";
            exchangePackingLineAction.Caption = "Exchange (Ctrl + E)";
            exchangePackingLineAction.TargetObjectType = typeof(PackingLine);
            exchangePackingLineAction.TargetViewType = ViewType.ListView;
            exchangePackingLineAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            exchangePackingLineAction.Shortcut = "CtrlE";

            exchangePackingLineAction.Execute += ExchangePackingLineAction_Execute;
        }

        private void ExchangePackingLineAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var selectedRows = View.SelectedObjects.Cast<PackingLine>();            
            MessageOptions options = null;
            var packingList = ((DetailView)View.ObjectSpace.Owner).CurrentObject as PackingList;

            if(selectedRows.Count() == 2)
            {
                var firstRow = selectedRows.First();
                var secondRow = selectedRows.Last();

                var tempSequenceNumber = firstRow.SequenceNo;
                var tempFromNo = firstRow.FromNo;
                var tempToNo = firstRow.ToNo;

                firstRow.SequenceNo = secondRow.SequenceNo;
                firstRow.FromNo = secondRow.FromNo;
                firstRow.ToNo = secondRow.ToNo;

                secondRow.SequenceNo = tempSequenceNumber;
                secondRow.FromNo = tempFromNo;
                secondRow.ToNo = tempToNo;

                int cartonNo = 1;

                View.Refresh();
                (View as ListView).EditView?.Refresh();

                var packingLines = (View as ListView).CollectionSource.List
                    .Cast<PackingLine>().ToList();

                foreach (PackingLine line in packingLines.OrderBy(x => x.SequenceNo))
                {
                    line.FromNo = cartonNo;
                    line.ToNo = cartonNo + (int)line.TotalCarton - 1;
                    cartonNo += (int)line.TotalCarton;
                }

                packingList.PackingLines = packingLines.OrderBy(x => x.SequenceNo).ToList();
                View.Refresh(true);
            }
            else
            {
                options = Message.GetMessageOptions("Please select 2 rows for exchange", "Error",
                    InformationType.Error, null, 5000);
                Application.ShowViewStrategy.ShowMessage(options);
            }
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
