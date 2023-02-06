using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Service.Request;
using System.Configuration;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class ImportPackingList_ViewController : ObjectViewController<DetailView, PackingListSearchParam>
    {
        public ImportPackingList_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction importPackingList = new PopupWindowShowAction(this, "Import", PredefinedCategory.Unspecified);
            importPackingList.ImageName = "Import";
            importPackingList.Caption = "Import (Ctrl + I)";
            importPackingList.TargetObjectType = typeof(PackingListSearchParam);
            importPackingList.TargetViewType = ViewType.DetailView;
            importPackingList.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            importPackingList.Shortcut = "CtrlI";

            importPackingList.CustomizePopupWindowParams += ImportPackingList_CustomizePopupWindowParams;
            importPackingList.Execute += ImportPackingList_Execute;
        }
        private void ImportPackingList_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new PackingListImportParam()
            {
                Customer = objectSpace.GetObjects<Customer>().FirstOrDefault(),
            };
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.View = view;
        }

        private void ImportPackingList_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var packingListObjectSpace = Application.CreateObjectSpace(typeof(PackingList));
            var viewObject = View.CurrentObject as PackingListSearchParam;
            var importParam = e.PopupWindowView.CurrentObject as PackingListImportParam;

            if (importParam != null)
            {
                var service = new PackingListService();
                var request = new ImportPackingListRequest()
                {
                    CustomerID = importParam.Customer?.ID,
                    FilePath = importParam.ImportFilePath,
                    UserName = SecuritySystem.CurrentUserName
                };
                //if(importParam.Customer?.ID == "HA")
                //{
                    var response = service.ImportPackingList(request).Result;
                    if (response.Result.IsSuccess && response.Data.Any())
                    {
                        response.Data.ForEach(x =>
                        {
                            var packingList = packingListObjectSpace.GetObjectByKey<PackingList>(x.ID);
                            if(packingList.CustomerID == "HA")
                            {
                                packingList.PackingListImageThumbnails.Add(new PackingListImageThumbnail
                                {
                                    ImageUrl = ConfigurationManager.AppSettings.Get("DontShortShip").ToString(),
                                    SortIndex = 1
                                });
                            }
                            else if (packingList.CustomerID == "DE")
                            {
                                packingList.PackingListImageThumbnails.Add(new PackingListImageThumbnail
                                {
                                    ImageUrl = ConfigurationManager.AppSettings.Get("LogoPackingListDE").ToString(),
                                    SortIndex = 1
                                });
                            }

                            viewObject.PackingLists.Add(packingList);
                        });

                        packingListObjectSpace.CommitChanges();

                        var message = Message.GetMessageOptions("Import successfully. ", "Successs",
                                   InformationType.Success, null, 5000);
                        Application.ShowViewStrategy.ShowMessage(message);

                        View.Refresh(true);
                    }
                    else
                    {
                        var message = Message.GetMessageOptions("Import failed.", "Error",
                                   InformationType.Error, null, 5000);
                        Application.ShowViewStrategy.ShowMessage(message);
                        //viewObject.PackingLists.Add(response.Result);
                        View.Refresh();
                    }
                //}
                //else
                //{
                //    var message = Message.GetMessageOptions("Please select customer", "Error",
                //                   InformationType.Error, null, 5000);
                //    Application.ShowViewStrategy.ShowMessage(message);
                //    //viewObject.PackingLists.Add(response.Result);
                //    View.Refresh();
                //}
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
