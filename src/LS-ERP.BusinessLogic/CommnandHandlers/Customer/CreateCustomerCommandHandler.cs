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
    public class CreateCustomerCommandHandler :
        IRequestHandler<CreateCustomerCommand, CommonCommandResult<Customer>>
    {
        private readonly ILogger<CreateCustomerCommandHandler> logger;
        private readonly ICustomerRepository customerRepository;
        private readonly CustomerValidator customerValidator;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;

        public CreateCustomerCommandHandler(ILogger<CreateCustomerCommandHandler> logger,
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

        public async Task<CommonCommandResult<Customer>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute command", DateTime.Now.ToString());
            var result = new CommonCommandResult<Customer>();
            var customer = mapper.Map<Customer>(request);
            customer.SetCreateAudit(request.GetUser());

            if (!customerValidator.IsValid(customer, out string errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            try
            {
                customerRepository.Add(customer);
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
                result.Result = customer;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
