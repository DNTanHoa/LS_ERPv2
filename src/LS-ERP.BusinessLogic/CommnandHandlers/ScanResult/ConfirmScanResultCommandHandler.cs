using Common.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Events;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{ 
    public class ConfirmScanResultCommandHandler : IRequestHandler<ConfirmScanResultCommand, CommonCommandResult>
    {
        private readonly SqlServerAppDbContext context;
        private readonly IMediator mediator;

        public ConfirmScanResultCommandHandler(SqlServerAppDbContext context,
            IMediator mediator)
        {
            this.context = context;
            this.mediator = mediator;
        }

        public Task<CommonCommandResult> Handle(ConfirmScanResultCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult();

            try
            {
                var scanResults = context.ScanResult
                    .Where(x => request.Data.Contains(x.ID)).ToList();

                foreach (var scanResult in scanResults)
                {
                    scanResult.IsConfirm = true;
                }

                context.ScanResult.UpdateRange(scanResults);
                context.SaveChanges();
                result.Success = true;

                mediator.Publish(new ScanResultConfirmedEvent()
                {

                });

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Task.FromResult(result);
        }
    }
}
