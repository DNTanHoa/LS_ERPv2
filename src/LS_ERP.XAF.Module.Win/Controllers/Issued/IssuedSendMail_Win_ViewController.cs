using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.Service.Mail;
using LS_ERP.XAF.Module.BusinessObjects;
using LS_ERP.XAF.Module.Controllers;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Dtos;
using LS_ERP.XAF.Module.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailMessage = LS_ERP.Service.Common.Message;
using Message = LS_ERP.XAF.Module.Helpers.Message;

namespace LS_ERP.XAF.Module.Win.Controllers
{
    public class IssuedSendMail_Win_ViewController : SendMail_ViewController
    {
        public override void SendMailIssued_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            base.SendMailIssued_Execute(sender, e);
            var issued = View.CurrentObject as Issued;
            var popupObject = e.PopupWindowViewCurrentObject as IssuedSendMailParam;

            MessageOptions options = null;

            if (issued != null && issued.FabricRequestID != null && issued.FabricRequestID > 0)
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
                var fabricRequest = ObjectSpace.GetObjectByKey<FabricRequest>(issued.FabricRequestID);
                var contentTemplate = ObjectSpace.GetObjectByKey<MessageTemplate>("IssuedMailTemplate");
                var contentTemplateDetail = ObjectSpace.GetObjectByKey<MessageTemplate>("IssuedDetailMailTemplate");
                var contentTemplateTotalDetail = ObjectSpace.GetObjectByKey<MessageTemplate>("IssuedTotalDetailMailTemplate");
                var contentTemplateFabricRequest = ObjectSpace.GetObjectByKey<MessageTemplate>("IssuedFabricRequestMailTemplate");
                var contentTemplateFabricRequestDetail = ObjectSpace.GetObjectByKey<MessageTemplate>("ApprovedFabricRequestDetailMailTemplate");

                var content = contentTemplate.Template;

                /// set data lot
                var tableContentIssuedDetail = string.Empty;
                var listStorageID = new List<long>();
                var dicGroupLines = new Dictionary<string, List<IssuedGroupLine>>();

                foreach (var issuedGroupLine in issued.IssuedGroupLines)
                {
                    string key = issuedGroupLine.ItemID + issuedGroupLine.ItemColorName?.Trim();
                    List<IssuedGroupLine> groupLines = new List<IssuedGroupLine>();

                    if (dicGroupLines.TryGetValue(key, out List<IssuedGroupLine> rsGroupLines))
                    {
                        groupLines = rsGroupLines;
                    }

                    groupLines.Add(issuedGroupLine);
                    dicGroupLines[key] = groupLines;

                    if (issuedGroupLine.StorageDetailID > 0)
                    {
                        listStorageID.Add(issuedGroupLine.StorageDetailID);
                    }

                }

                // get supplier
                var dicSupplierInfo = new Dictionary<long, IssuedSupplierInfoDto>();
                var service = new IssuedService();
                var messageOptions = new MessageOptions();

                var request = new GetIssuedSupplierRequest()
                {
                    StorageDetailIDs = listStorageID
                };

                var responseSupplier = service.GetIssuedSupplier(request).Result;

                if (responseSupplier != null)
                {
                    if (responseSupplier.Result.Code == "000")
                    {
                        //messageOptions = Message.GetMessageOptions(compare.Result.Message, "Success",
                        //    InformationType.Success, null, 5000);
                        if (responseSupplier.Data != null)
                        {
                            dicSupplierInfo = responseSupplier.Data.Details.ToDictionary(x => x.StorageDetailID);
                        }
                    }
                    else
                    {
                        messageOptions = Message.GetMessageOptions(responseSupplier.Result.Message, "Error",
                            InformationType.Error, null, 5000);
                    }
                }

                foreach (var itemGroupLine in dicGroupLines)
                {
                    decimal totalQty = 0;

                    int count = itemGroupLine.Value.Count();
                    int start = 1;
                    foreach (var issuedGroupLine in itemGroupLine.Value)
                    {
                        var rowContent = contentTemplateDetail.Template;

                        rowContent = rowContent.Replace("[COLORSIZE]", issuedGroupLine.ItemColorName);
                        rowContent = rowContent.Replace("[DYELOT]", issuedGroupLine.DyeLotNumber);
                        rowContent = rowContent.Replace("[OUTPUTROLL]", issuedGroupLine.Roll?.ToString("#,##0.######"));
                        rowContent = rowContent.Replace("[OUTPUTQTY]", issuedGroupLine.IssuedQuantity?.ToString("#,##0.######"));
                        rowContent = rowContent.Replace("[OUTPUTORDER]", fabricRequest.OrderNumber);
                        rowContent = rowContent.Replace("[MODELCODE]", issuedGroupLine.ItemID);

                        if (dicSupplierInfo.TryGetValue(issuedGroupLine.StorageDetailID, out IssuedSupplierInfoDto rs))
                        {
                            rowContent = rowContent.Replace("[SUPPLIER]", rs.SupplierName);
                        }
                        else
                        {
                            rowContent = rowContent.Replace("[SUPPLIER]", "");
                        }


                        tableContentIssuedDetail += rowContent;

                        if (count == start)
                        {
                            totalQty += (decimal)issuedGroupLine.IssuedQuantity;
                            var rowContentTotal = contentTemplateTotalDetail.Template;
                            rowContentTotal = rowContentTotal.Replace("[TOTAL_OUTPUTQTY]", totalQty.ToString("#,##0.######"));
                            tableContentIssuedDetail += rowContentTotal;
                        }
                        else
                        {
                            //tableContentIssuedDetail = tableContentIssuedDetail.Replace("[TableContentIssuedTotalDetail]", "");
                            totalQty += (decimal)issuedGroupLine.IssuedQuantity;
                        }

                        start++;
                    }
                }

                content = content.Replace("[TableContentIssuedDetail]", tableContentIssuedDetail);

                content = content.Replace("[ContentIssuedFabricRequest]", contentTemplateFabricRequest.Template);
                content = content.Replace("[RequestDate]", fabricRequest.RequestDate.ToString("MM/dd/yyyy"));
                content = content.Replace("[Company]", fabricRequest.Company?.Code);

                var tableContentFabricRequestDetail = string.Empty;
                int NoNum = 1;
                var shortDetails = fabricRequest.Details.OrderBy(x => x.ItemColorCode).OrderBy(x => x.FabricColor);

                foreach (var item in shortDetails)
                {
                    var rowContent = contentTemplateFabricRequestDetail.Template;

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

                    tableContentFabricRequestDetail += rowContent;
                }

                content = content.Replace("[TableContentFabricRequest]", tableContentFabricRequestDetail);

                content = content.Replace("[Remark]", popupObject.Remark);

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

                var destinations = new List<string>();
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
                    Subject = "Issued for Fabric request - " + fabricRequest.CustomerStyleNumber + " " + fabricRequest.OrderNumber,
                    Destinations = destinations,
                    Body = content,
                    CCs = CCs
                };

                ///Send issued mail
                var sendMailResult = mailService.SenViaOutLook(message);

                if (sendMailResult)
                {
                    options = Message.GetMessageOptions("Submit successfully.", "Success",
                       InformationType.Success, null, 5000);
                }
                else
                {
                    options = Message.GetMessageOptions("Send mail error check your outlook mail", "Error",
                        InformationType.Error, null, 5000);
                }
            }
        }
    }
}

