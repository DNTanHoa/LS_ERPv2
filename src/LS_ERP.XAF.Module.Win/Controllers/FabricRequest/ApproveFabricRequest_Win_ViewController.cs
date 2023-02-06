using AutoMapper;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.Service.Mail;
using LS_ERP.XAF.Module.BusinessObjects;
using LS_ERP.XAF.Module.Controllers;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Service.FabricRequest;
using LS_ERP.XAF.Module.Service.Request;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailMessage = LS_ERP.Service.Common.Message;
using Message = LS_ERP.XAF.Module.Helpers.Message;

namespace LS_ERP.XAF.Module.Win.Controllers
{
    public class ApproveFabricRequest_Win_ViewController : ApprovedFabricRequest_ViewController
    {
        public override void SubmitFabricRequest_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            base.SubmitFabricRequest_Execute(sender, e);

            var fabricRequest = View.CurrentObject as FabricRequest;
            var popupObject = e.PopupWindowViewCurrentObject as SubmitFabricRequestParam;

            MessageOptions options = null;

            if (fabricRequest != null)
            {
                ///// Set part material to approved status
                //foreach (var partRevisionLogDetail in partRevisionLog.PartRevisionLogDetails)
                //{
                //    partRevisionLogDetail.PartMaterial.PartMaterialStatusCode = "3";
                //}

                ///// Create log
                //var approvePartRevisionLog = ObjectSpace.CreateObject<PartRevisionLog>();
                //approvePartRevisionLog.Code = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                //approvePartRevisionLog.PartRevisionLogReferenceCode = partRevisionLog.Code;
                //approvePartRevisionLog.ActivityDate = DateTime.Now;
                //approvePartRevisionLog.PartRevisionID = partRevisionLog.PartRevisionID;
                //approvePartRevisionLog.Description = "Approve submit " + partRevisionLog.Code;
                //approvePartRevisionLog.SetCreateAudit(SecuritySystem.CurrentUserName);

                /// Send mail to submit persion
                ///Create mail service
                var mailService = new MailService();

                ///Make message
                var contentTemplate = ObjectSpace.GetObjectByKey<MessageTemplate>("ApprovedFabricRequestMailTemplate");
                var contentTemplateDetail = ObjectSpace.GetObjectByKey<MessageTemplate>("ApprovedFabricRequestDetailMailTemplate");
                var content = contentTemplate.Template;

                var approveUser = ObjectSpace.GetObjects<ApplicationUser>()
                    .FirstOrDefault(x => x.UserName == SecuritySystem.CurrentUserName);

                var groupMailCutting = ObjectSpace.GetObjects<GroupMail>()
                    .FirstOrDefault(x => x.CompanyCode == fabricRequest.CompanyCode &&
                                         x.CustomerID == fabricRequest.CustomerID &&
                                         x.DepartmentName.ToUpper().Contains("CUT"));

                var groupMailWarehouse = ObjectSpace.GetObjects<GroupMail>()
                    .FirstOrDefault(x => x.CompanyCode == fabricRequest.CompanyCode &&
                                         x.CustomerID == fabricRequest.CustomerID &&
                                         x.DepartmentName.ToUpper().Contains("WAREHOUSE"));

                content = content.Replace("[FullName]", approveUser.FullName);
                content = content.Replace("[CustomerStyleNumber]", fabricRequest.CustomerStyleNumber);
                content = content.Replace("[OrderNumber]", fabricRequest.OrderNumber);
                content = content.Replace("[OrderQuantity]", fabricRequest.OrderQuantity?.ToString("#,##0.######"));
                content = content.Replace("[RequestDate]", fabricRequest.RequestDate.ToString("yyyy-MM-dd"));
                content = content.Replace("[CustomerID]", fabricRequest.Customer?.Name);
                content = content.Replace("[Remark]", popupObject.Remarks);
                content = content.Replace("[Reason]", fabricRequest.Reason);

                var tableContent = string.Empty;
                var shortDetails = fabricRequest.Details.OrderBy(x => x.ItemColorCode).OrderBy(x => x.FabricColor);
                int NoNum = 1;
                foreach (var item in shortDetails)
                {
                    var rowContent = contentTemplateDetail.Template;

                    rowContent = rowContent.Replace("[No]", NoNum++.ToString());
                    rowContent = rowContent.Replace("[CustomerStyleNumber]", fabricRequest.CustomerStyleNumber);
                    rowContent = rowContent.Replace("[OrderNumber]", fabricRequest.OrderNumber);
                    rowContent = rowContent.Replace("[FabricColor]", item.FabricColor);
                    rowContent = rowContent.Replace("[ItemColorCode]", item.ItemColorCode);
                    rowContent = rowContent.Replace("[OrderQuantity]", item.OrderQuantity?.ToString("#,##0.######"));
                    rowContent = rowContent.Replace("[RequestQuantity]", item.RequestQuantity?.ToString("#,##0.######"));
                    rowContent = rowContent.Replace("[SemiFinishedProductQuantity]", item.SemiFinishedProductQuantity?.ToString("#,##0.######"));
                    rowContent = rowContent.Replace("[StreakRequestQuantity]", item.StreakRequestQuantity?.ToString("#,##0.######"));
                    rowContent = rowContent.Replace("[PercentPrintQuantity]", item.PercentPrintQuantity?.ToString("#,##0.######"));
                    rowContent = rowContent.Replace("[CustomerConsumption]", item.CustomerConsumption?.ToString("#,##0.######"));
                    rowContent = rowContent.Replace("[BalanceQuantity]", item.BalanceQuantity?.ToString("#,##0.######"));
                    rowContent = rowContent.Replace("[BreadthWidth]", item.BreadthWidth);

                    tableContent += rowContent;
                }

                content = content.Replace("[TableContent]", tableContent);


                var destinations = new List<string>();
                //var user = ObjectSpace.GetObjects<ApplicationUser>()
                //    .FirstOrDefault(x => x.UserName == fabricRequest.CreatedBy);

                destinations.Add(groupMailCutting.Mail);
                destinations.Add(groupMailWarehouse.Mail);

                var CCs = new List<string>();

                var listCCCutting = groupMailCutting.CCs?.Split(";");
                if (listCCCutting != null && listCCCutting.Count() > 0)
                {
                    foreach (var cc in listCCCutting)
                    {
                        if (!string.IsNullOrEmpty(cc))
                        {
                            CCs.Add(cc);
                        }

                    }
                }

                var listCCWarehouse = groupMailWarehouse.CCs?.Split(";");

                if (listCCWarehouse != null && listCCWarehouse.Count() > 0)
                {
                    foreach (var cc in listCCWarehouse)
                    {
                        if (!string.IsNullOrEmpty(cc))
                        {
                            CCs.Add(cc);
                        }
                    }
                }

                var message = new MailMessage()
                {
                    Subject = "Approved submit for Fabric request - " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"),
                    Destinations = destinations,
                    Body = content,
                    CCs = CCs
                };

                var service = new FabricRequestService();

                var request = new UpdateStatusFabricRequest()
                {
                    ID = fabricRequest.ID,
                    StatusID = "A",
                    UserName = SecuritySystem.CurrentUserName
                };

                var updateResponse = service.UpdateStatusFabricRequest(request).Result;

                if (updateResponse != null)
                {
                    if (updateResponse.Result.Code == "000")
                    {
                        var sendMailResult = mailService.SenViaOutLook(message);

                        if (sendMailResult)
                        {
                            var objectSpace = Application.CreateObjectSpace(typeof(FabricRequest));
                            ///Save fabric request to submitted status
                            var fabricUpdate = objectSpace.GetObjectByKey<FabricRequest>(fabricRequest.ID);

                            //fabricUpdate.Status = objectSpace.GetObjectByKey<Status>("A");
                            //fabricUpdate.StatusID = "A";

                            fabricRequest.Status = fabricUpdate.Status;
                            fabricRequest.StatusID = fabricUpdate.StatusID;

                            //objectSpace.CommitChanges();

                            options = Message.GetMessageOptions("Submit successfully", "Success",
                                InformationType.Success, null, 5000);

                            //var fabricRequestLog = objectSpace.CreateObject<FabricRequestLog>();
                            //fabricRequestLog = mapper.Map<FabricRequestLog>(fabricRequest);
                            //fabricRequestLog.StatusID = fabricRequest.StatusID;
                        }
                        else
                        {
                            options = Message.GetMessageOptions("Send mail error check your outlook mail or outlook is not open", "Error",
                                InformationType.Error, null, 5000);
                        }

                    }
                    else
                    {
                        options = Message.GetMessageOptions(updateResponse.Result.Message, "Error",
                            InformationType.Error, null, 5000);
                    }
                }
                else
                {
                    options = Message.GetMessageOptions("Unexpected error", "Error", InformationType.Error, null, 5000);
                }

                ObjectSpace.CommitChanges();
                Application.ShowViewStrategy.ShowMessage(options);
                View.Refresh();
            }
        }
    }
}
