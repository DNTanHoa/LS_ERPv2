using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ReceiptForReturnAction_ViewController 
        : ObjectViewController<DetailView, ReturnPopupModel>
    {
        public ReceiptForReturnAction_ViewController()
        {
            InitializeComponent();

            SimpleAction loadIssuedForReturn = new SimpleAction(this, "LoadIssuedForReturn", PredefinedCategory.Unspecified);
            loadIssuedForReturn.ImageName = "ConvertTo";
            loadIssuedForReturn.Caption = "Load Data";
            loadIssuedForReturn.TargetObjectType = typeof(ReturnPopupModel);
            loadIssuedForReturn.TargetViewType = ViewType.DetailView;
            loadIssuedForReturn.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            loadIssuedForReturn.Execute += LoadIssuedForReturn_Execute;
        }

        private void LoadIssuedForReturn_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as ReturnPopupModel;

            if(viewObject != null)
            {
                var issued = ObjectSpace.GetObjectByKey<Issued>(viewObject.Issued?.Number);

                if(issued != null)
                {
                    var data = issued.IssuedLines.ToList();
                    data.ForEach(item =>
                    {
                        item.ItemColorCode = item.GroupItemColor != true ? item.ItemColorCode : string.Empty;
                        item.ItemColorName = item.GroupItemColor != true ? item.ItemColorName : string.Empty;
                    });

                    var detailGroups = issued.IssuedLines.GroupBy(x => new
                    {
                        x.ItemMasterID,
                        x.ItemName,
                        x.ItemColorCode,
                        x.ItemColorName,
                        x.Specify,
                        x.GroupSize
                    });

                    var details = detailGroups.Select(detailGroup => new ReturnDetailModel()
                    {
                        ItemMaterID = detailGroup.Key.ItemMasterID,
                        ItemName = $"{detailGroup.Key.ItemName}-{detailGroup.Key.ItemColorCode}-{detailGroup.Key.ItemColorName}" +
                        $"-{detailGroup.Key.Specify}",
                        ItemColorCode = detailGroup.Key.ItemColorName,
                        ItemColorName = detailGroup.Key.ItemColorName,
                        CustomerStyle = detailGroup.First().CustomerStyle,
                        GarmentColorCode = detailGroup.First().GarmentColorCode,
                        GarmentColorName = detailGroup.First().GarmentColorName,
                        GarmentSize = detailGroup.First().GarmentSize,
                        Season = detailGroup.First().Season,
                        IssuedQuantity = detailGroup.Sum(x => x.IssuedQuantity) ?? 0,

                    }).ToList();

                    viewObject.Details = details;

                    View.Refresh();
                }
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
