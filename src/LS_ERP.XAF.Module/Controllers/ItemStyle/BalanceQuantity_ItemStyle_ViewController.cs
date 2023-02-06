using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class BalanceQuantity_ItemStyle_ViewController : ViewController
    {
        public BalanceQuantity_ItemStyle_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction balanceQuantityItemStyle = new PopupWindowShowAction(this, "BalanceQuantityItemStyle", PredefinedCategory.Unspecified);
            balanceQuantityItemStyle.ImageName = "GlobalColors";
            balanceQuantityItemStyle.Caption = "Balance Quantity";
            balanceQuantityItemStyle.TargetObjectType = typeof(ItemStyle);
            balanceQuantityItemStyle.TargetViewType = ViewType.ListView;
            balanceQuantityItemStyle.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            balanceQuantityItemStyle.CustomizePopupWindowParams += BalanceQuantityItemStyle_CustomizePopupWindowParams;
            balanceQuantityItemStyle.Execute += BalanceQuantityItemStyle_Execute;
        }

        private void BalanceQuantityItemStyle_CustomizePopupWindowParams(object sender, 
            CustomizePopupWindowParamsEventArgs e)
        {
            var itemStyles = View.SelectedObjects.Cast<ItemStyle>();

            var objectSpace = this.ObjectSpace;
            e.DialogController.SaveOnAccept = false;
            var model = new BalanceQuantityParam()
            {
                ItemStyles = itemStyles.ToList(),
                Customer = objectSpace.GetObjects<Customer>().FirstOrDefault()
            };
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.Maximized = true;
            e.View = view;
        }

        private void BalanceQuantityItemStyle_Execute(object sender, 
            PopupWindowShowActionExecuteEventArgs e)
        {
            var balanceParam = e.PopupWindowViewCurrentObject as BalanceQuantityParam;
            
            if(balanceParam != null)
            {
                ///Update purchase order forecast
                var itemStyle = View.SelectedObjects.Cast<ItemStyle>();
                
                if(itemStyle != null)
                {
                    var productionBOMs = itemStyle.SelectMany(x => x.ProductionBOMs)
                        .ToList();

                    if (productionBOMs != null)
                    {
                        foreach(var material in balanceParam.Materials)
                        {
                            var productionBOM = 
                                productionBOMs.FirstOrDefault(x => x.ID == material.ProductionBOMID);

                            if(productionBOM != null)
                            {
                                productionBOM.RemainQuantity = material.RemainQuantity;
                                productionBOM.ReservedQuantity = material.ReservedQuantity;
                            }
                        }
                    }
                }

                ObjectSpace.CommitChanges();
                View.Refresh();
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
