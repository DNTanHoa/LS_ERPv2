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
    public class UpdateVendorCommandHandler
        : IRequestHandler<UpdateVendorCommand, CommonCommandResult<Vendor>>
    {
        private readonly ILogger<UpdateVendorCommandHandler> logger;
        private readonly IVendorRepository VendorRepository;
        private readonly IMapper mapper;
        private readonly SqlServerAppDbContext context;

        public UpdateVendorCommandHandler(ILogger<UpdateVendorCommandHandler> logger,
            IVendorRepository VendorRepository,
            IMapper mapper,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.VendorRepository = VendorRepository;
            this.mapper = mapper;
            this.context = context;
        }

        public async Task<CommonCommandResult<Vendor>> Handle(UpdateVendorCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute command", DateTime.Now.ToString());
            var result = new CommonCommandResult<Vendor>();

            var existVendor = VendorRepository.GetVendor(request.ID);

            if (existVendor == null)
            {
                result.Message = "Not found Vendor";
                return result;
            }

            mapper.Map(request, existVendor);

            existVendor.SetUpdateAudit(request.GetUser());

            try
            {
                VendorRepository.Update(existVendor);
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
                result.Result = existVendor;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
            }
            return result;
        }
    }
}
