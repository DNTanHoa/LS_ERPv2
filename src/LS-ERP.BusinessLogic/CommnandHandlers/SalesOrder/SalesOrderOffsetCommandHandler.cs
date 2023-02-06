using Common.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.EntityFrameworkCore.Entities;
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
    public class SalesOrderOffsetCommandHandler
        : IRequestHandler<SalesOrderOffsetCommand, CommonCommandResult>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<SalesOrderOffsetCommandHandler> logger;

        public SalesOrderOffsetCommandHandler(SqlServerAppDbContext context,
            ILogger<SalesOrderOffsetCommandHandler> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        public Task<CommonCommandResult> Handle(SalesOrderOffsetCommand request, 
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult();

            try
            {
                foreach(var item in request.Data)
                {
                    item.SetCreateAudit(request.UserName);
                }
                context.SalesOrderOffset.AddRange(request.Data);

                var sourceStyles = request.Data.Select(x => x.SourceLSStyle);
                var targetStyles = request.Data.Select(x => x.TargetLSStyle);

                context.SaveChanges();

                var sourceItemStyles = context.ItemStyle
                    .Include(x => x.OrderDetails)
                    .Include(x => x.ProductionBOMs)
                    .Where(x => sourceStyles.Contains(x.LSStyle)).ToList();

                var targetItemStyles = context.ItemStyle
                    .Include(x => x.OrderDetails)
                    .Include(x => x.ProductionBOMs)
                    .Where(x => targetStyles.Contains(x.LSStyle)).ToList();

                var sourceProductionBoms = context.ProductionBOM
                    .Where(x => targetItemStyles.Select(s => s.Number).Contains(x.ItemStyleNumber))
                    .ToList();

                var processResult = SalesOrderOffsetDataProcess.Calculate(request.Data, sourceItemStyles,
                    targetItemStyles, sourceProductionBoms, out List<ProductionBOM> updatedProductionBoms,
                    out List<SalesOrderOffset> saleOrderOffsets);

                if (processResult)
                {
                    context.SalesOrderOffset.UpdateRange(saleOrderOffsets);
                    context.ProductionBOM.UpdateRange(updatedProductionBoms);
                    context.SaveChanges();
                }

                result.Success = true;
            }
            catch(Exception ex)
            {
                result.Message = ex.Message;
            }

            return Task.FromResult(result);
        }
    }
}
