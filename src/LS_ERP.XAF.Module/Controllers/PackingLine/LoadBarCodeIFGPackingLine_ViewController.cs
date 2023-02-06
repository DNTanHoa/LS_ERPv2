using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class LoadBarCodeIFGPackingLine_ViewController : ObjectViewController<ListView, PackingLine>
    {
       
        public LoadBarCodeIFGPackingLine_ViewController()
        {
            InitializeComponent();

            SimpleAction loadBarCodePackingLine = new SimpleAction(this, "LoadBarCodeIFGPackingLine", PredefinedCategory.Unspecified);
            loadBarCodePackingLine.ImageName = "Barcode_32x32";
            loadBarCodePackingLine.Caption = "Load BarCode (Ctrl + I)";
            loadBarCodePackingLine.TargetObjectType = typeof(PackingLine);
            loadBarCodePackingLine.TargetViewType = ViewType.ListView;
            loadBarCodePackingLine.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadBarCodePackingLine.Shortcut = "CtrlI";

            loadBarCodePackingLine.Execute += LoadBarCodeIFGPackingLine_Execute;
        }
        private void LoadBarCodeIFGPackingLine_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var objectSpaceBoxGroup = Application.CreateObjectSpace(typeof(BoxGroup));
            
            var packingList = ((DetailView)View.ObjectSpace.Owner).CurrentObject as PackingList;
            var errorMessage = "";

            if (packingList?.CustomerID == "IFG")
            {
                //var packingLines = (View as ListView).CollectionSource.List
                //    .Cast<PackingLine>().ToList().OrderBy(x => x.SequenceNo);

                var sheetName = "Total";
                if ((packingList.SheetNameID ?? 0) != 0)
                    sheetName = ObjectSpace.GetObjectByKey<PackingSheetName>(packingList.SheetNameID).SheetName;

                var criteria = CriteriaOperator.Parse("[Style] " +
                    "IN (" + string.Join(",", packingList?.ItemStyles.Select(x => "'" + x.CustomerStyle + "'")) + ")");
                var itemModels = ObjectSpace.GetObjects<ItemModel>(criteria).ToList();

                criteria = CriteriaOperator.Parse("[ItemStyleNumber] " +
                    "IN (" + string.Join(",", packingList?.ItemStyles.Select(x => "'" + x.Number + "'")) + ")");
                var barCodes = ObjectSpace.GetObjects<ItemStyleBarCode>(criteria).ToList();

                var newBoxGroups = IFG_CreateBoxGroupProcess
                        .CreateScanBarcode(ref packingList, objectSpaceBoxGroup, itemModels, barCodes, sheetName, ref errorMessage);

                if (newBoxGroups == null)
                {
                    var error = Message.GetMessageOptions(errorMessage, "Error", InformationType.Error, null, 5000);
                    Application.ShowViewStrategy.ShowMessage(error);
                }
                else if(newBoxGroups.Any())
                {
                    /// Set box group -> IsPulled = true when create barcode on packing list
                    var updateBoxGroups = objectSpaceBoxGroup.GetObjects<BoxGroup>()
                        .Where(x => x.PackingListCode == packingList.PackingListCode &&
                               x.IsPulled != true && x.CustomerID == packingList.CustomerID).ToList();
                    if (updateBoxGroups.Any())
                    {
                        updateBoxGroups.ForEach(x =>
                        {
                            x.IsPulled = true;
                        });
                    }

                    packingList.BarCodeCompleted = true;
                    objectSpaceBoxGroup.CommitChanges();
                    objectSpace.CommitChanges();

                    var message = Message.GetMessageOptions("Load barcode successful", "Success", InformationType.Success, null, 5000);
                    Application.ShowViewStrategy.ShowMessage(message);
                }
            }

            ((DetailView)View.ObjectSpace.Owner).Refresh(true);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
