using AutoMapper;
using LS_ERP.BusinessLogic.Dtos.Customer;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries.Customer
{
    public class CustomerQueries : ICustomerQueries
    {
        private readonly ICustomerRepository customerRepository;
        private readonly IMapper mapper;

        public CustomerQueries(ICustomerRepository customerRepository,
            IMapper mapper)
        {
            this.customerRepository = customerRepository;
            this.mapper = mapper;
        }

        public CustomerDetailDtos GetCustomerByID(string id)
        {
            var customer = customerRepository.GetCustomer(id);
            
            if(customer != null)
            {
                var data = mapper.Map<CustomerDetailDtos>(customer);
                return data;
            }
            
            return null;
        }

        public IEnumerable<CustomerSummaryDtos> GetCustomerSummary()
        {
            var customers = customerRepository.GetCustomers();
            return customers.Select(x => mapper.Map<CustomerSummaryDtos>(x));
        }
    }
}
