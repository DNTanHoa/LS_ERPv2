using AutoMapper;
using Hangfire;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ultils.Helpers;

namespace LS_ERP.BusinessLogic.Shared.Process.Jobs
{
    public class CreateCuttingCardOperationDetailJob
    {
        private readonly ILogger<CreateCuttingCardOperationDetailJob> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;
        

        public CreateCuttingCardOperationDetailJob(ILogger<CreateCuttingCardOperationDetailJob> logger,
            SqlServerAppDbContext context, IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
            
        }

        [JobDisplayName("Create, Update when update Master CuttingCard")]
        [AutomaticRetry(Attempts = 3)]
        public Task Execute(CuttingCard cuttingCard,string userName)
        {           
            try
            {
                if(cuttingCard.IsPrint == false)
                {
                    CreateOperationDetailCuttingCard(cuttingCard, userName);
                }
                else
                {
                    if(cuttingCard.IsAllSize)
                    {
                        //
                        var cuttingOutput = context.CuttingOutput.Where(x => x.ID == cuttingCard.CuttingOutputID).FirstOrDefault();
                        var listSize = (from d in context.DailyTarget.Where(x => x.Operation == "CUTTING"
                                                            && x.MergeBlockLSStyle == cuttingOutput.MergeBlockLSStyle
                                                            && (x.MergeLSStyle == cuttingOutput.MergeLSStyle || string.IsNullOrEmpty(cuttingOutput.MergeLSStyle)))
                                        from o in context.AllocDailyOutput.Where(a => a.LSStyle == d.LSStyle && a.TargetID == d.ID)
                                        select o.Size).Distinct().ToList();
                        //
                     
                        foreach(var size in listSize)
                        {
                            var tempCuttingCard = new CuttingCard();
                            mapper.Map(cuttingCard, tempCuttingCard);
                            tempCuttingCard.ID = cuttingCard.ID;
                            tempCuttingCard.Size = size;
                            CreateOperationDetailCuttingCard(tempCuttingCard,userName);
                        }   
                    }
                    else
                    {
                        CreateOperationDetailCuttingCard(cuttingCard, userName);
                    }    
                }  
            }
            catch (DbUpdateException ex)
            {
                logger.LogError("Create, update cuttingCard operation detail event handler has error with message {@message}",
                    ex.InnerException?.Message);
                LogHelper.Instance.Error("Create, update cuttingCard operation detail  event handler has error with message {@message}",
                    ex.InnerException?.Message);
            }

            return Task.CompletedTask;  
        }
        public void CreateOperationDetailCuttingCard(CuttingCard cuttingCard, string userName)
        {
            var operationDetails = context.OperationDetail.Where(x => x.MergeBlockLSStyle.Trim().Equals(cuttingCard.MergeBlockLSStyle)
                                                                     && x.Set == cuttingCard.Set
                                                                     && x.IsPercentPrint == cuttingCard.IsPrint
                                                                     && x.FabricContrastName == cuttingCard.FabricContrastName
                                                                    ).ToList();
            foreach (var operationDetail in operationDetails)
            {
                var existCuttingCard = context.CuttingCard.Where(c => c.MergeBlockLSStyle == operationDetail.MergeBlockLSStyle
                                                            && c.ParentID == cuttingCard.ID
                                                            && c.Set == operationDetail.Set
                                                            && c.IsPrint == operationDetail.IsPercentPrint
                                                            && c.Size == cuttingCard.Size
                                                            && c.FabricContrastName == operationDetail.FabricContrastName
                                                            && c.CardType == operationDetail.OperationName
                                                        ).FirstOrDefault();
                if (existCuttingCard == null) // Create CuttingCard with operation detail
                {
                    var operation = context.Operation.Where(o => o.ID == operationDetail.OperationID).FirstOrDefault();
                    var newCuttingCard = new CuttingCard();
                    newCuttingCard = mapper.Map<CuttingCard>(cuttingCard);
                    //newCuttingCard.CardType = operationDetail.OperationName;
                    newCuttingCard.CardType = operation.Name;
                    newCuttingCard.CardTypeID = operationDetail.OperationID;
                    newCuttingCard.ParentID = cuttingCard.ID;
                    newCuttingCard.SetCreateAudit(userName);
                    context.CuttingCard.Add(newCuttingCard);
                    context.SaveChanges();
                }
                else // Update totalPackage 
                {
                    existCuttingCard.MergeBlockLSStyle = cuttingCard.MergeBlockLSStyle;
                    existCuttingCard.MergeLSStyle = cuttingCard.MergeLSStyle;
                    existCuttingCard.Set = cuttingCard.Set;
                    existCuttingCard.Size = cuttingCard.Size;
                    existCuttingCard.Lot = cuttingCard.Lot;
                    existCuttingCard.Quantity = cuttingCard.Quantity;
                    existCuttingCard.WorkCenterName = cuttingCard.WorkCenterName;
                    existCuttingCard.AllocQuantity = cuttingCard.AllocQuantity;
                    existCuttingCard.TableNO = cuttingCard.TableNO;
                    existCuttingCard.IsPrint = cuttingCard.IsPrint;
                    existCuttingCard.FabricContrastName = cuttingCard.FabricContrastName;
                    existCuttingCard.FabricContrastColor = cuttingCard.FabricContrastColor;
                    existCuttingCard.FabricContrastDescription = cuttingCard.FabricContrastDescription;
                    existCuttingCard.TotalPackage = cuttingCard.TotalPackage;
                    existCuttingCard.Remark = cuttingCard.Remark;
                    existCuttingCard.SetUpdateAudit(userName);
                    context.CuttingCard.Update(existCuttingCard);
                    context.SaveChanges();
                }
            }
        }
    }
}
