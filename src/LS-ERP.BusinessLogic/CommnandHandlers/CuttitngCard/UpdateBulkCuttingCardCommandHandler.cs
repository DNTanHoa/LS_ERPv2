using AutoMapper;
using Common.Model;
using Hangfire;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.BusinessLogic.Shared.Process.Jobs;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class UpdateBulkCuttingCardCommandHandler
        : IRequestHandler<UpdateBulkCuttingCardCommand, CommonCommandResultHasData<CuttingCard>>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<CreateCuttingCardCommandHandler> logger;
        private readonly IMapper mapper;

        public UpdateBulkCuttingCardCommandHandler(SqlServerAppDbContext context,
            ILogger<CreateCuttingCardCommandHandler> logger,
            IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<CuttingCard>> Handle(UpdateBulkCuttingCardCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<CuttingCard>();
            if(string.IsNullOrEmpty(request.CurrentOperation))
            {
                logger.LogInformation("{@time} - Exceute update cutting card command with request {@request}",
               DateTime.Now.ToString(), request.ToString());
                LogHelper.Instance.Information("{@time} - Exceute update cutting card command with request {@request}",
                    DateTime.Now.ToString(), request.ToString());
            }    
            else if(request.CurrentOperation == "SUPPERMARKET")
            {
                logger.LogInformation("{@time} - Exceute update cutting card location location command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
                LogHelper.Instance.Information("{@time} - Exceute update cutting card location command with request {@request}",
                    DateTime.Now.ToString(), request.ToString());
                try
                {
                    foreach (var ID in request.Ids)
                    {
                        var existCuttingCard = context.CuttingCard.FirstOrDefault(x => x.ID == ID);

                        if (existCuttingCard != null)
                        {
                            existCuttingCard.CurrentOperation = request.CurrentOperation;
                            existCuttingCard.Location = request.StorageBinEntry.BinCode;
                            existCuttingCard.StorageBinEntryID = request.StorageBinEntry.ID;

                            existCuttingCard.SetUpdateAudit(request.UserName);
                            context.CuttingCard.Update(existCuttingCard);
                            context.SaveChanges();
                            result.Success = true;
                        }
                        else
                        {
                            result.Success = false;
                            result.Message = "Can't not find to update";
                        }
                    }

                }
                catch (DbUpdateException ex)
                {
                    result.Message = ex.Message;
                    logger.LogInformation("{@time} - Exceute update cutting card location command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                    LogHelper.Instance.Information("{@time} - Exceute update cutting card location command with request {@request} has error {@error}",
                        DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                }
            } 
            else if (request.CurrentOperation == "COMPLING")
            {
                logger.LogInformation("{@time} - Exceute update cutting card compling command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
                LogHelper.Instance.Information("{@time} - Exceute update cutting card compling command with request {@request}",
                    DateTime.Now.ToString(), request.ToString());
                try
                {
                    foreach (var ID in request.Ids)
                    {
                        var existCuttingCard = context.CuttingCard.FirstOrDefault(x => x.ID == ID);

                        if (existCuttingCard != null)
                        {
                            //existCuttingCard.CurrentOperation = request.CurrentOperation;
                            existCuttingCard.IsCompling = request.IsCompling;

                            existCuttingCard.SetUpdateAudit(request.UserName);
                            context.CuttingCard.Update(existCuttingCard);
                            context.SaveChanges();
                            result.Success = true;
                        }
                        else
                        {
                            result.Success = false;
                            result.Message = "Can't not find to update";
                        }
                    }

                }
                catch (DbUpdateException ex)
                {
                    result.Message = ex.Message;
                    logger.LogInformation("{@time} - Exceute update cutting card compling command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                    LogHelper.Instance.Information("{@time} - Exceute update cutting card compling command with request {@request} has error {@error}",
                        DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                }
            } 
            else if (request.CurrentOperation == "EXTERNALPRINT")
            {
                logger.LogInformation("{@time} - Exceute add cutting card to delivery note command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
                LogHelper.Instance.Information("{@time} - Exceute add cutting card to delivery note command with request {@request}",
                    DateTime.Now.ToString(), request.ToString());
                try
                {
                    foreach (var ID in request.Ids)
                    {
                        var existDeliveryNoteDetail = context.DeliveryNoteDetail.FirstOrDefault(x => x.CuttingCardID == ID && x.IsDeleted == false);
                        var existDeliveryNote = context.DeliveryNote.Where(x => x.IsSend == false
                                                                            && x.ID == request.DeliveryNote.ID
                                                                            && x.IsDeleted == false).FirstOrDefault();
                        if (existDeliveryNoteDetail == null && existDeliveryNote != null)
                        {
                            var deliveryNoteDetail = new DeliveryNoteDetail();
                            var cuttingCard = context.CuttingCard.FirstOrDefault(x => x.ID == ID);
                            mapper.Map(cuttingCard, deliveryNoteDetail);
                            deliveryNoteDetail.SetCreateAudit(request.UserName);
                            deliveryNoteDetail.DeliveryNoteID = request.DeliveryNote.ID;
                            deliveryNoteDetail.CuttingCardID = ID;
                            deliveryNoteDetail.CompanyID = request.DeliveryNote.CompanyID;
                            deliveryNoteDetail.Status = "NEW";
                            context.DeliveryNoteDetail.Add(deliveryNoteDetail);
                            context.SaveChanges();
                            result.Success = true;
                        }
                        else
                        {
                            result.Success = false;
                            result.Message = "Can't not find to add";
                        }
                    }
                }
                catch (DbUpdateException ex)
                {
                    result.Message = ex.Message;
                    logger.LogInformation("{@time} - Exceute  add cutting card to delivery note command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                    LogHelper.Instance.Information("{@time} - Exceute  add cutting card to delivery note command with request {@request} has error {@error}",
                        DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                }
            }
            else if (request.CurrentOperation == "RECEIVEEXTERNALPRINT")
            {
                logger.LogInformation("{@time} - Exceute receive from external print command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
                LogHelper.Instance.Information("{@time} - Exceute receive from external print command with request {@request}",
                    DateTime.Now.ToString(), request.ToString());
                try
                {
                    foreach (var ID in request.Ids)
                    {
                        var existDeliveryNoteDetail = context.DeliveryNoteDetail.FirstOrDefault(x => x.CuttingCardID == ID);

                        if (existDeliveryNoteDetail != null)
                        {
                            existDeliveryNoteDetail.IsReceived = true;
                            existDeliveryNoteDetail.ReceivedDate = DateTime.Now;
                            existDeliveryNoteDetail.SetUpdateAudit(request.UserName);
                            context.DeliveryNoteDetail.Update(existDeliveryNoteDetail);
                            context.SaveChanges();
                            result.Success = true;
                        }
                        else
                        {
                           
                        }
                    }
                }
                catch (DbUpdateException ex)
                {
                    result.Message = ex.Message;
                    logger.LogInformation("{@time} - Exceute  receive from external print command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                    LogHelper.Instance.Information("{@time} - Exceute  receive from external print command with request {@request} has error {@error}",
                        DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                }
            }
            return Task.FromResult(result);
        }
        
    }
}
