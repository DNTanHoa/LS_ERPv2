using Common.Model;
using LS_ERP.BusinessLogic.Commands;
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
    public class DeleteScanResultDetailCommandHandler
        : IRequestHandler<DeleteScanResultDetailCommand, CommonCommandResult>
    {
        private readonly SqlServerAppDbContext context;

        public DeleteScanResultDetailCommandHandler(SqlServerAppDbContext context)
        {
            this.context = context;
        }

        public Task<CommonCommandResult> Handle(DeleteScanResultDetailCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult();

            try
            {
                var scanResultAudits = new List<ScanResultAudit>();

                var scanResultDetails = context.ScanResultDetail
                    .Where(x => request.Data.Contains(x.ID)).ToList();

                scanResultDetails.ForEach(x =>
                {
                    x.IsDeleted = true;

                    var scanResultAudit = new ScanResultAudit()
                    {
                        ScanResultDetailID = x.ID,
                        CustomerStyle = x.CustomerStyle,
                        GarmentColorCode = x.GarmentColorCode,
                        GarmentSize = x.GarmentSize,
                        TagID = x.TagID,
                        Expected = x.Expected,
                        Found = x.Found,
                        Unexpected = x.Unexpected,
                        Missing = x.Missing,
                        Description = x.Description,
                        ItemCode = x.ItemCode,
                        AuditUserName = request.UserName,
                        AuditDate = DateTime.Now
                    };

                    scanResultAudits.Add(scanResultAudit);
                });

                context.ScanResultAudit.AddRange(scanResultAudits);
                context.ScanResultDetail.UpdateRange(scanResultDetails);
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
