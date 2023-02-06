using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
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
    public class GetIssuedSupplierCommandHandler
        : IRequestHandler<GetIssuedSupplierInfoCommand, GetIssuedSupplierInfoResult>
    {
        private readonly ILogger<GetIssuedSupplierCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IStorageDetailRepository storageDetailRepository;
        private readonly IFabricPurchaseOrderRepository fabricPurchaseOrderRepository;
        private readonly IPurchaseOrderRepository purchaseOrderRepository;

        public GetIssuedSupplierCommandHandler(ILogger<GetIssuedSupplierCommandHandler> logger,
            SqlServerAppDbContext context,
            IStorageDetailRepository storageDetailRepository,
            IFabricPurchaseOrderRepository fabricPurchaseOrderRepository,
            IPurchaseOrderRepository purchaseOrderRepository)
        {
            this.logger = logger;
            this.context = context;
            this.storageDetailRepository = storageDetailRepository;
            this.fabricPurchaseOrderRepository = fabricPurchaseOrderRepository;
            this.purchaseOrderRepository = purchaseOrderRepository;
        }

        public async Task<GetIssuedSupplierInfoResult> Handle(GetIssuedSupplierInfoCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute get issued supplier command", DateTime.Now.ToString());
            var result = new GetIssuedSupplierInfoResult();

            var storageDetails = storageDetailRepository.GetOnlyStorageDetails(request.StorageDetailIDs);

            var fabricOrderNumber = storageDetails.Where(x => x.FabricPurchaseOrderNumber != null && x.FabricPurchaseOrderNumber != string.Empty).ToList();
            var purchaseOrderNumber = storageDetails.Where(x => x.PurchaseOrderNumber != null && x.PurchaseOrderNumber != string.Empty).ToList();

            var dicFabricPurchaseOrder = fabricPurchaseOrderRepository.GetFabricPurchaseOrders(fabricOrderNumber.Select(x => x.FabricPurchaseOrderNumber)
                                                                   .ToList())
                                                                   .ToDictionary(x => x.Number);
            var dicPurchaseOrder = purchaseOrderRepository.GetOnlyPurchaseOrders(purchaseOrderNumber.Select(x => x.PurchaseOrderNumber)
                                                       .ToList())
                                                       .ToDictionary(x => x.Number);

            var dicNumberOrder = new Dictionary<string, IssuedSupplierInfoDto>();

            foreach (var storageDetail in storageDetails)
            {
                if (!string.IsNullOrEmpty(storageDetail.PurchaseOrderNumber) &&
                    dicPurchaseOrder.TryGetValue(storageDetail.PurchaseOrderNumber, out PurchaseOrder rsPurchaseOrder))
                {
                    var keyPO = storageDetail.ID + rsPurchaseOrder.Number;

                    if (!dicNumberOrder.ContainsKey(keyPO))
                    {
                        IssuedSupplierInfoDto issuedSupplierDto = new IssuedSupplierInfoDto();
                        issuedSupplierDto.StorageDetailID = storageDetail.ID;
                        issuedSupplierDto.PurchaseOrderNumber = rsPurchaseOrder.Number;
                        issuedSupplierDto.SupplierName = rsPurchaseOrder.Vendor?.Name;

                        dicNumberOrder[keyPO] = issuedSupplierDto;
                    }
                }
                else if (!string.IsNullOrEmpty(storageDetail.FabricPurchaseOrderNumber) &&
                    dicFabricPurchaseOrder.TryGetValue(storageDetail.FabricPurchaseOrderNumber, out FabricPurchaseOrder rsFabricPurchaseOrder))
                {
                    var keyPO = storageDetail.ID + rsFabricPurchaseOrder.Number;

                    if (!dicNumberOrder.ContainsKey(keyPO))
                    {
                        IssuedSupplierInfoDto issuedSupplierDto = new IssuedSupplierInfoDto();
                        issuedSupplierDto.StorageDetailID = storageDetail.ID;
                        issuedSupplierDto.PurchaseOrderNumber = rsFabricPurchaseOrder.Number;
                        issuedSupplierDto.SupplierName = rsFabricPurchaseOrder.FabricSupplier;

                        dicNumberOrder[keyPO] = issuedSupplierDto;
                    }
                }
            }

            List<IssuedSupplierInfoDto> rsData = new List<IssuedSupplierInfoDto>();
            foreach (var item in dicNumberOrder)
            {
                rsData.Add(item.Value);
            }

            try
            {
                result.IsSuccess = true;
                IssuedSupplierDto supplier = new IssuedSupplierDto
                {
                    Details = rsData
                };
                result.Result = supplier;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
            }


            return result;
        }
    }
}
