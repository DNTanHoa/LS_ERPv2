using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.BusinessLogic.Validator;
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
    public class UpdateBrandCommandHandler
        : IRequestHandler<UpdateBrandCommand, CommonCommandResult<Brand>>
    {
        private readonly ILogger<UpdateBrandCommandHandler> logger;
        private readonly IBrandRepository brandRepository;
        private readonly IMapper mapper;
        private readonly BrandValidator brandValidator;
        private readonly SqlServerAppDbContext context;

        public UpdateBrandCommandHandler(ILogger<UpdateBrandCommandHandler> logger,
            IBrandRepository brandRepository,
            IMapper mapper,
            BrandValidator brandValidator,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.brandRepository = brandRepository;
            this.mapper = mapper;
            this.brandValidator = brandValidator;
            this.context = context;
        }

        public async Task<CommonCommandResult<Brand>> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute command", DateTime.Now.ToString());
            var result = new CommonCommandResult<Brand>();

            var existBrand = brandRepository.GetBrand(request.Code);

            if (existBrand == null)
            {
                result.Message = "Not found brand";
                return result;
            }

            mapper.Map(request, existBrand);

            if (!brandValidator.IsValid(existBrand, out string errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            existBrand.SetUpdateAudit(request.GetUser());

            try
            {
                brandRepository.Update(existBrand);
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
                result.Result = existBrand;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
            }
            return result;
        }
    }
}
