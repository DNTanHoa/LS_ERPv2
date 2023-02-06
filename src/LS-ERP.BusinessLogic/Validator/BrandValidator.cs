using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Validator
{
    public class BrandValidator
    {
        private readonly ICustomerRepository customerRepository;

        public BrandValidator(ICustomerRepository customerRepository)
        {
            this.customerRepository = customerRepository;
        }

        public bool IsValid(Brand brand, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!string.IsNullOrEmpty(brand.CustomerID))
            {
                if (!customerRepository.IsExist(brand.CustomerID))
                {
                    errorMessage = "Invalid customer";
                    return false;
                }
            }

            return true;
        }
    }
}
