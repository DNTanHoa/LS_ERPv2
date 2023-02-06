using AutoMapper;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.Service.Mail;
using LS_ERP.XAF.Module.Controllers;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailMessage = LS_ERP.Service.Common.Message;
using Message = LS_ERP.XAF.Module.Helpers.Message;

namespace LS_ERP.XAF.Module.Win.Controllers
{
    public class SubmitBom_Win_ViewController
        : SubmitBom_ViewController
    {
        public override void SubmitBomAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            base.SubmitBomAction_Execute(sender, e);

            var viewObject = e.PopupWindowViewCurrentObject as SubmitBomParam;
            var partRevision = viewObject.PartMaterials?.FirstOrDefault()?.PartRevision;

            MessageOptions options = null;

            if (viewObject != null)
            {
                ///Get mail template
                var masterTemplate = ObjectSpace.GetObjectByKey<MessageTemplate>("SubmitMailTemplate");
                var contentTemplate = ObjectSpace.GetObjectByKey<MessageTemplate>("SubmitBOMTemplate");

                ///Create mail content
                var mailContent = masterTemplate.Template?.Replace("[FullName]", SecuritySystem.CurrentUserName);
                mailContent = mailContent.Replace("[PartNum]", partRevision.PartNumber);
                mailContent = mailContent.Replace("[RevNum]", partRevision.RevisionNumber);
                mailContent = mailContent.Replace("[Season]", partRevision.Season);
                mailContent = mailContent.Replace("[EffectDate]", partRevision.EffectDate?.ToString("MM/dd/yyyy HH:mm:ss"));

                var tableContent = string.Empty;

                foreach (var item in viewObject.PartMaterials)
                {
                    var rowContent = contentTemplate.Template;

                    rowContent = rowContent.Replace("[ExCode]", item.ExternalCode);
                    rowContent = rowContent.Replace("[ItemID]", item.ItemID);
                    rowContent = rowContent.Replace("[ItemName]", item.ItemName);
                    rowContent = rowContent.Replace("[Color]", item.ItemColorCode);
                    rowContent = rowContent.Replace("[GridValue]", item.ItemColorName);
                    rowContent = rowContent.Replace("[GarmentSize]", item.GarmentSize);
                    rowContent = rowContent.Replace("[QtyPerUnit]", item.QuantityPerUnit?.ToString("G29"));
                    rowContent = rowContent.Replace("[PerUnit]", item.PerUnitID);
                    rowContent = rowContent.Replace("[PriceUnit]", item.PriceUnitID);
                    rowContent = rowContent.Replace("[Currency]", item.CurrencyID);
                    rowContent = rowContent.Replace("[VendId]", item.VendorID);
                    rowContent = rowContent.Replace("[MaterialClass]", item.MaterialTypeCode);

                    tableContent += rowContent;
                }

                mailContent = mailContent.Replace("[TableContent]", tableContent);

                ///Create mail service
                var mailService = new MailService();

                ///Make message
                var destinations = new List<string>();
                destinations.Add(viewObject.Email);

                var message = new MailMessage()
                {
                    Subject = "Request confirmination for master BOM - " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"),
                    Destinations = destinations,
                    Body = mailContent,
                };

                ///Send submit mail
                var sendMailResult = mailService.SenViaOutLook(message);

                if (sendMailResult)
                {
                    var config = new MapperConfiguration(x => x.CreateMap<PartMaterial, PartRevisionLogDetail>()
                                                                .ForMember(x => x.ID, y => y.Ignore()));
                    var mapper = config.CreateMapper();

                    var log = ObjectSpace.CreateObject<PartRevisionLog>();
                    log.Code = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    log.Description = "Submit Bom via email";
                    log.ActivityDate = DateTime.Now;
                    log.PartRevisionID = partRevision.ID;
                    log.Note = viewObject.Remarks;
                    log.PartRevisionLogDetails = new List<PartRevisionLogDetail>();

                    foreach (var partMaterial in viewObject.PartMaterials)
                    {
                        partMaterial.PartMaterialStatusCode = "1";
                        var partRevisonLogDetail = ObjectSpace.CreateObject<PartRevisionLogDetail>();
                        mapper.Map(partMaterial, partRevisonLogDetail);
                        partRevisonLogDetail.PartRevisionLogCode = log.Code;
                        partRevisonLogDetail.PartMaterialID = partMaterial.ID;
                    }

                    log.SetCreateAudit(SecuritySystem.CurrentUserName);

                    ObjectSpace.CommitChanges();
                    View.Refresh();

                    options = Message.GetMessageOptions("Submit successfully.", "Success",
                        InformationType.Success, null, 5000);
                }
                else
                {
                    options = Message.GetMessageOptions("Send mail error check your outlook mail", "Error",
                    InformationType.Error, null, 5000);
                }
            }
            else
            {
                options = Message.GetMessageOptions("Null object error. Please contact admin for support", "Error",
                    InformationType.Error, null, 5000);
            }

            Application.ShowViewStrategy.ShowMessage(options);
            View.Refresh();
        }
    }
}
