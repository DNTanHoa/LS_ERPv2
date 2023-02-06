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
    public class CreateVendorCommandHandler
        : IRequestHandler<CreateVendorCommand, CommonCommandResult<Vendor>>
    {
        private readonly ILogger<CreateVendorCommandHandler> logger;
        private readonly IMapper mapper;
        private readonly IVendorRepository VendorRepository;
        private readonly SqlServerAppDbContext context;

        public CreateVendorCommandHandler(ILogger<CreateVendorCommandHandler> logger,
            IMapper mapper,
            IVendorRepository VendorRepository,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.VendorRepository = VendorRepository;
            this.context = context;
        }

        public async Task<CommonCommandResult<Vendor>> Handle(CreateVendorCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute command", DateTime.Now.ToString());
            var result = new CommonCommandResult<Vendor>();
            var Vendor = mapper.Map<Vendor>(request);

            Vendor.SetCreateAudit(request.GetUser());

            try
            {
                Vendor = VendorRepository.Add(Vendor);
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
                result.Result = Vendor;
            }
            catch (DbUpdateException ex)
            {
                if (VendorRepository.IsExist(request.ID))
                {
                    result.Message = "Vendor with code " + request.ID + "has exist";
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
