using Common.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class DeleteStorageImportCommandHandler
        : IRequestHandler<DeleteStorageImportCommand, CommonCommandResult>
    {
        private readonly SqlServerAppDbContext context;

        public DeleteStorageImportCommandHandler(SqlServerAppDbContext context)
        {
            this.context = context;
        }

        public Task<CommonCommandResult> Handle(DeleteStorageImportCommand request, 
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult();

            try
            {
                /// Lấy dữ liệu import
                var storageImport = context.StorageImport
                    .Include(x => x.Details)
                    .FirstOrDefault(x => x.ID == request.ID);

                /// Lấy dữ liệu material transaction tương ứng
                var transactions = context.MaterialTransaction
                    .Where(x => x.StorageImportID == storageImport.ID)
                    .ToList();

                if(storageImport != null)
                {


                    context.StorageImportDetail.RemoveRange(storageImport.Details);
                    context.StorageImport.Remove(storageImport);

                    context.SaveChanges();
                }
                else
                {
                    result.Message = "Not find data";
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Task.FromResult(result);
        }
    }
}
