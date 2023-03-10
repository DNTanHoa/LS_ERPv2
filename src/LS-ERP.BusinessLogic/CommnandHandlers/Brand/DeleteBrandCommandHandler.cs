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
    public class DeleteBrandCommandHandler
        : IRequestHandler<DeleteBrandCommand, CommonCommandResult<Brand>>
    {
        private readonly ILogger<DeleteBrandCommandHandler> logger;
        private readonly IBrandRepository brandRepository;
        private readonly SqlServerAppDbContext context;

        public DeleteBrandCommandHandler(ILogger<DeleteBrandCommandHandler> logger,
            IBrandRepository brandRepository,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.brandRepository = brandRepository;
            this.context = context;
        }

        public async Task<CommonCommandResult<Brand>> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult<Brand>();

            if (brandRepository.IsExist(request.Code, out Brand willDeleteBrand))
            {
                result.Message = "Brand not exist";
                return result;
            }

            try
            {
                brandRepository.Delete(willDeleteBrand);
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
