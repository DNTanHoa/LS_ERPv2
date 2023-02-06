using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Service.Request;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class IssuedItemStyle_ViewController : ViewController
    {
        public IssuedItemStyle_ViewController()
        {
            InitializeComponent();

            SimpleAction issuedItemStyleAction = new SimpleAction(this, "IssuedItemStyle", PredefinedCategory.Unspecified);
            issuedItemStyleAction.ImageName = "ModelEditor_ModelMerge";
            issuedItemStyleAction.Caption = "Issued PPC";
            issuedItemStyleAction.TargetObjectType = typeof(ItemStyle);
            issuedItemStyleAction.TargetViewType = ViewType.ListView;
            issuedItemStyleAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            issuedItemStyleAction.Execute += IssuedItemStyleAction_Execute;
        }

        private void IssuedItemStyleAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var selectedItemStyles = View.SelectedObjects.Cast<ItemStyle>();
            MessageOptions message = null;
            
            if(selectedItemStyles != null &&
               selectedItemStyles.Any())
            {
                var service = new ItemStyleService();
                var request = new BulkIssueItemStyleRequest()
                {
                    UserName = SecuritySystem.CurrentUserName,
                    ItemStyleNumbers = selectedItemStyles.Select(x => x.Number).ToList(),
                };

                var response = service.BulkIssue(request).Result;

                if (response != null)
                {
                    if (response.Result.Code == "000")
                    {
                        message = Message.GetMessageOptions("Issue successfully", "Success",
                            InformationType.Success, null, 5000);
                    }
                    else
                    {
                        message = Message.GetMessageOptions(response.Result.Message, "Error",
                            InformationType.Error, null, 5000);
                    }
                }

                message = Message.GetMessageOptions("Action successfully", "Success",
                    InformationType.Success, null, 5000);
            }
            else
            {
                message = Message.GetMessageOptions("Unknown error. Contact your admin", "Error",
                    InformationType.Error, null, 5000);
            }

            if (message != null)
                Application.ShowViewStrategy.ShowMessage(message);
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
