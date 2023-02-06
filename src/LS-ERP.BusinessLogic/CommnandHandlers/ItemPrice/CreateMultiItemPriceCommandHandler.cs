using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.BusinessLogic.Validator;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared;
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
    public class CreateMultiItemPriceCommandHandler
        : IRequestHandler<CreateMultiItemPriceCommand, CommonCommandResult<List<ItemPrice>>>
    {
        private readonly ILogger<CreateMultiItemPriceCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly ItemPriceValidator validator;

        public CreateMultiItemPriceCommandHandler(ILogger<CreateMultiItemPriceCommandHandler> logger,
            SqlServerAppDbContext context,
            ItemPriceValidator validator)
        {
            this.logger = logger;
            this.context = context;
            this.validator = validator;
        }

        public async Task<CommonCommandResult<List<ItemPrice>>> Handle(CreateMultiItemPriceCommand request, 
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult<List<ItemPrice>>();
            logger.LogInformation("{@time} - Execute import item price command", DateTime.Now.ToString());

            if(!validator.IsValid(request.ItemPrices, out string errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            try
            {
                request.ItemPrices.ForEach(x =>
                {
                    if (string.IsNullOrEmpty(x.ShippingTermCode))
                        x.ShippingTermCode = null;
                });

                context.ItemPrice.AddRange(request.ItemPrices);
                context.SaveChanges();

                result.IsSuccess = true;
                result.Result = request.ItemPrices;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
            }

            return result;
        }
    }
}
