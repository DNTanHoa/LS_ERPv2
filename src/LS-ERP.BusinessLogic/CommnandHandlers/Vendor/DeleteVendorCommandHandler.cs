using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared;
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
    public class DeleteVendorCommandHandler
        : IRequestHandler<DeleteVendorCommand, CommonCommandResult<Vendor>>
    {
        private readonly ILogger<DeleteVendorCommandHandler> logger;
        private readonly IVendorRepository VendorRepository;
        private readonly SqlServerAppDbContext context;

        public DeleteVendorCommandHandler(ILogger<DeleteVendorCommandHandler> logger,
            IVendorRepository VendorRepository,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.VendorRepository = VendorRepository;
            this.context = context;
        }

        public async Task<CommonCommandResult<Vendor>> Handle(DeleteVendorCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult<Vendor>();

            if (VendorRepository.IsExist(request.ID, out Vendor willDeleteVendor))
            {
                result.Message = "Vendor not exist";
                return result;
            }

            try
            {
                VendorRepository.Delete(willDeleteVendor);
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
            }


            return result;
        }
    }
}
