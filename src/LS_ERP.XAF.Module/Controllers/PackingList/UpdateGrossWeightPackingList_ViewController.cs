using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using System;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class UpdateGrossWeightPackingList_ViewController : ObjectViewController<ListView, PackingList>
    {
        public UpdateGrossWeightPackingList_ViewController()
        {
            InitializeComponent();

            SimpleAction updateGrossWeightPackingList = new SimpleAction(this, "UpdateGrossWeightPackingList", PredefinedCategory.Unspecified);
            updateGrossWeightPackingList.ImageName = "BO_Security_Permission_Type";
            updateGrossWeightPackingList.Caption = "Update Gross Weight (Ctrl + W)";
            updateGrossWeightPackingList.TargetObjectType = typeof(PackingList);
            updateGrossWeightPackingList.TargetViewType = ViewType.ListView;
            updateGrossWeightPackingList.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            updateGrossWeightPackingList.Shortcut = "CtrlW";

            updateGrossWeightPackingList.Execute += UpdateGrossWeightPackingList_Execute;
        }
        private void UpdateGrossWeightPackingList_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var objectSpace = Application.CreateObjectSpace(typeof(PackingList));
            var packingLists = ((ListView)View).SelectedObjects.Cast<PackingList>()
                                            .Where(x => x.CustomerID == "DE").ToList();

            if (packingLists.Any())
            {
                try
                {
                    foreach (var data in packingLists)
                    {
                        var creatia = CriteriaOperator.Parse("[CustomerStyle] = ? && [CustomerID] = ?",
                                        data?.ItemStyles?.FirstOrDefault()?.CustomerStyle, data?.CustomerID);
                        var styleNetWeights = ObjectSpace.GetObjects<StyleNetWeight>(creatia);
                        var packingList = objectSpace.GetObjectByKey<PackingList>(data.ID);

                        foreach(var netWeight in styleNetWeights)
                        {
                            if((netWeight?.BoxWeight ?? 0) > 0)
                            {
                                var packingLines = packingList.PackingLines
                                    .Where(x => x?.Size == netWeight?.Size &&
                                                x?.BoxDimensionCode?.Trim()?.ToLower().Replace("  ", "").Replace(" ", "").Replace("*", "").Replace("x", "")
                                                == netWeight?.BoxDimensionCode?.Trim()?.ToLower().Replace("  ", "").Replace(" ", "").Replace("*", "").Replace("x", "")).ToList();
                                packingLines.ForEach(x =>
                                {
                                    x.GrossWeight = x.NetWeight + x.TotalCarton * netWeight.BoxWeight;
                                    x.SetUpdateAudit(SecuritySystem.CurrentUserName);
                                });
                            }
                        }
                        packingList.SetUpdateAudit(SecuritySystem.CurrentUserName);
                    }

                    objectSpace.CommitChanges();

                    var message = Message.GetMessageOptions("Update gross weight sucessful", "Success", InformationType.Success, null, 5000);
                    Application.ShowViewStrategy.ShowMessage(message);
                }
                catch (Exception ex)
                {
                    var message = Message.GetMessageOptions("Update gross weight failed", "Error", InformationType.Error, null, 5000);
                    Application.ShowViewStrategy.ShowMessage(message);
                }

            }

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
