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
    public class CreateBrandCommandHandler
        : IRequestHandler<CreateBrandCommand, CommonCommandResult<Brand>>
    {
        private readonly ILogger<CreateBrandCommandHandler> logger;
        private readonly IMapper mapper;
        private readonly IBrandRepository brandRepository;
        private readonly BrandValidator brandValidator;
        private readonly SqlServerAppDbContext context;

        public CreateBrandCommandHandler(ILogger<CreateBrandCommandHandler> logger,
            IMapper mapper,
            IBrandRepository brandRepository,
            BrandValidator brandValidator,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.brandRepository = brandRepository;
            this.brandValidator = brandValidator;
            this.context = context;
        }

        public async Task<CommonCommandResult<Brand>> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute command", DateTime.Now.ToString());
            var result = new CommonCommandResult<Brand>();
            var brand = mapper.Map<Brand>(request);

            if(!brandValidator.IsValid(brand, out string errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            brand.SetCreateAudit(request.GetUser());

            try
            {
                brand = brandRepository.Add(brand);
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
                result.Result = brand;
            }
            catch (DbUpdateException ex)
            {
                if (brandRepository.IsExist(request.Code))
                {
                    result.Message = "Brand with code " + request.Code + "has exist";
                }
                else
                {
                    result.Message = ex.InnerException.Message;
                }
            }
            return result;
        }
    }
}
