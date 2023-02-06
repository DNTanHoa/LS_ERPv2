using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Validator
{
    public class CustomerValidator
    {
        private readonly ICurrencyRepository currencyRepository;
        private readonly IDivisionRepository divisionRepository;
        private readonly IPaymentTermRepository paymentTermRepository;
        private readonly IPriceTermRepository priceTermRepository;

        public CustomerValidator(ICurrencyRepository currencyRepository,
            IDivisionRepository divisionRepository,
            IPaymentTermRepository paymentTermRepository,
            IPriceTermRepository priceTermRepository)
        {
            this.currencyRepository = currencyRepository;
            this.divisionRepository = divisionRepository;
            this.paymentTermRepository = paymentTermRepository;
            this.priceTermRepository = priceTermRepository;
        }

        public bool IsValid(Customer customer, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!string.IsNullOrEmpty(customer.CurrencyID))
            {
                if (!currencyRepository.IsExist(customer.CurrencyID))
                {
                    errorMessage = "Invalid currency";
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(customer.DivisionID))
            {
                if (!divisionRepository.IsExist(customer.DivisionID))
                {
                    errorMessage = "Invalid division";
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(customer.PaymentTermCode))
            {
                if (!paymentTermRepository.IsExist(customer.PaymentTermCode))
                {
                    errorMessage = "Invalid payment term";
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(customer.PriceTermCode))
            {
                if (!priceTermRepository.IsExist(customer.PriceTermCode))
                {
                    errorMessage = "Invalid price term";
                    return false;
                }
            }

            return true;
        }
    }
}
