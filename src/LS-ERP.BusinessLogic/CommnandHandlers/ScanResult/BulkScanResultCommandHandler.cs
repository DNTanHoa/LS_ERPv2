using Common.Model;
using LS_ERP.BusinessLogic.Commands;
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
    public class BulkScanResultCommandHandler
        : IRequestHandler<BulkScanResultCommand, CommonCommandResult>
    {
        private readonly SqlServerAppDbContext context;

        public BulkScanResultCommandHandler(SqlServerAppDbContext context)
        {
            this.context = context;
        }

        public Task<CommonCommandResult> Handle(BulkScanResultCommand request, 
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult();

            try
            {
                var oldResults = context.ScanResult
                    .Where(x => request.Data.Select(d => d.Oid).Contains(x.Oid));

                context.ScanResult
                    .AddRange(request.Data.Where(x => !oldResults.Select(r => r.Oid)
                    .Contains(x.Oid)));

                context.SaveChanges();
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Task.FromResult(result);
        }
    }
}
