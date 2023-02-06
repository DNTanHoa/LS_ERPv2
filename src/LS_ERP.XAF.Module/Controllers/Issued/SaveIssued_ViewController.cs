using AutoMapper;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Dtos;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class SaveIssued_ViewController : ViewController
    {
        public SaveIssued_ViewController()
        {
            InitializeComponent();

            SimpleAction saveIssuedAction = new SimpleAction(this, "SaveIssued", PredefinedCategory.Unspecified);
            saveIssuedAction.ImageName = "Save";
            saveIssuedAction.Caption = "Save";
            saveIssuedAction.TargetObjectType = typeof(Issued);
            saveIssuedAction.TargetViewType = ViewType.DetailView;
            saveIssuedAction.PaintStyle = ActionItemPaintStyle.Default;
            saveIssuedAction.Category = "Save";
            saveIssuedAction.Shortcut = "CtrlS";

            saveIssuedAction.Execute += SaveIssuedAction_Execute;
        }

        private void SaveIssuedAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var issued = View.CurrentObject as Issued;
            MessageOptions message = null;
            var service = new IssuedService();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Issued, CreateIssuedRequest>();
                cfg.CreateMap<Issued, UpdateIssuedRequest>()
                                      .ForMember(x => x.GroupLines, y => y.MapFrom(s => s.IssuedGroupLines))
                                      .ForMember(x => x.Lines, y => y.MapFrom(s => s.IssuedLines));
                cfg.CreateMap<IssuedGroupLine, IssuedGroupLineDto>();
                cfg.CreateMap<IssuedLine, IssuedLineDto>();
            });

            var mapper = config.CreateMapper();
            var objectSpace = Application.CreateObjectSpace(typeof(Issued));

            var existIssued = objectSpace.GetObjectByKey<Issued>(issued.Number);

            if (existIssued != null)
            {
                var checkQty = true;

                var dicItemLine = new Dictionary<long, IssuedLine>();

                if (existIssued.IssuedLines != null)
                {
                    foreach (var item in existIssued.IssuedLines)
                    {
                        dicItemLine[item.ID] = item;
                    }
                }

                if (issued.IssuedLines != null)
                {
                    foreach (var item in issued.IssuedLines)
                    {
                        if (dicItemLine.TryGetValue(item.ID, out IssuedLine rsLine))
                        {
                            if (rsLine.IssuedQuantity < item.IssuedQuantity || rsLine.Roll < item.Roll)
                            {
                                checkQty = false;
                            }
                        }
                    }
                }

                if (checkQty)
                {
                    var request = mapper.Map<UpdateIssuedRequest>(issued);
                    request.UserName = SecuritySystem.CurrentUserName;
                    var response = service.UpdateIssued(request).Result;

                    if (response != null)
                    {
                        if (response.Result.Code == "000")
                        {
                            message = Message.GetMessageOptions("Action successfully", "Success", InformationType.Success,
                                null, 5000);
                            ObjectSpace.CommitChanges();
                        }
                        else
                        {
                            message = Message.GetMessageOptions(response.Result.Message, "Error", InformationType.Error,
                                null, 5000);
                        }
                    }
                    else
                    {
                        message = Message.GetMessageOptions("Unknown error. Contact your admin", "Error", InformationType.Error,
                                null, 5000);
                    }
                }
                else
                {
                    message = Message.GetMessageOptions("Quantity incorrect", "Error", InformationType.Error,
                                null, 5000);
                }

                if (message != null)
                    Application.ShowViewStrategy.ShowMessage(message);
            }
            else
            {
                var request = mapper.Map<CreateIssuedRequest>(issued);
                request.UserName = SecuritySystem.CurrentUserName;
                var response = service.CreateIssued(request).Result;

                if (response != null)
                {
                    if (response.Result.Code == "000")
                    {
                        message = Message.GetMessageOptions("Action successfully", "Success", InformationType.Success,
                            null, 5000);
                    }
                    else
                    {
                        message = Message.GetMessageOptions(response.Result.Message, "Error", InformationType.Error,
                            null, 5000);
                    }
                }
                else
                {
                    message = Message.GetMessageOptions("Unknown error. Contact your admin", "Error", InformationType.Error,
                            null, 5000);
                }

                if (message != null)
                    Application.ShowViewStrategy.ShowMessage(message);
            }

            View.Refresh();

            ModificationsController modificationsController = Frame.GetController<ModificationsController>();
            modificationsController.ModificationsHandlingMode = ModificationsHandlingMode.AutoRollback;
            ObjectSpace.Refresh();
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
