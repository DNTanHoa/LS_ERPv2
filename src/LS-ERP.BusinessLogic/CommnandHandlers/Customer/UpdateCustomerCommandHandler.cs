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
    public class UpdateCustomerCommandHandler
        : IRequestHandler<UpdateCustomerCommand, CommonCommandResult<Customer>>
    {
        private readonly ILogger<CreateCustomerCommandHandler> logger;
        private readonly ICustomerRepository customerRepository;
        private readonly CustomerValidator customerValidator;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;

        public UpdateCustomerCommandHandler(ILogger<CreateCustomerCommandHandler> logger,
            ICustomerRepository customerRepository,
            CustomerValidator customerValidator,
            SqlServerAppDbContext context,
            IMapper mapper)
        {
            this.logger = logger;
            this.customerRepository = customerRepository;
            this.customerValidator = customerValidator;
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<CommonCommandResult<Customer>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute command", DateTime.Now.ToString());
            var result = new CommonCommandResult<Customer>();
            var existCustomer = customerRepository.GetCustomer(request.ID);

            if (existCustomer == null)
            {
                result.Message = "Not found customer";
                return result;
            }

            mapper.Map(request, existCustomer);

            if (!customerValidator.IsValid(existCustomer, out string errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            existCustomer.SetUpdateAudit(request.GetUser());


            try
            {
                customerRepository.Update(existCustomer);
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
                result.Result = existCustomer;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
