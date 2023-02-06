using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Validator
{
    public class PurchaseRequestValidator
    {
        private readonly IDivisionRepository divisionRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly ICurrencyRepository currencyRepository;

        public PurchaseRequestValidator(IDivisionRepository divisionRepository,
            ICustomerRepository customerRepository,
            ICurrencyRepository currencyRepository)
        {
            this.divisionRepository = divisionRepository;
            this.customerRepository = customerRepository;
            this.currencyRepository = currencyRepository;
        }

        public bool IsValid(PurchaseRequest purchaseRequest, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!string.IsNullOrEmpty(purchaseRequest.CustomerID))
            {
                if (!customerRepository.IsExist(purchaseRequest.CustomerID))
                {
                    errorMessage = "Invalid customer input";
                    return false;
                }
            }
            else
            {
                errorMessage = "Customer is required";
                return false;
            }

            if (!string.IsNullOrEmpty(purchaseRequest.DivisionID))
            {
                if (!divisionRepository.IsExist(purchaseRequest.DivisionID))
                {
                    errorMessage = "Invalid division input";
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(purchaseRequest.CurrencyID))
            {
                if(!currencyRepository.IsExist(purchaseRequest.CurrencyID))
                {
                    errorMessage = "Invalid currency input";
                    return false;
                }
            }

            return true;
        }
    }
}
