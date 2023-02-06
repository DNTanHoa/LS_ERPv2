using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Validator
{
    public class SalesOrderValidator
    {
        private readonly ICurrencyRepository currencyRepository;
        private readonly IDivisionRepository divisionRepository;
        private readonly IPaymentTermRepository paymentTermRepository;
        private readonly IPriceTermRepository priceTermRepository;
        private readonly ISizeRepository sizeRepository;
        private readonly ICustomerRepository customerRepository;

        public SalesOrderValidator(ICurrencyRepository currencyRepository,
            IDivisionRepository divisionRepository,
            IPaymentTermRepository paymentTermRepository,
            IPriceTermRepository priceTermRepository,
            ISizeRepository sizeRepository,
            ICustomerRepository customerRepository)
        {
            this.currencyRepository = currencyRepository;
            this.divisionRepository = divisionRepository;
            this.paymentTermRepository = paymentTermRepository;
            this.priceTermRepository = priceTermRepository;
            this.sizeRepository = sizeRepository;
            this.customerRepository = customerRepository;
        }

        public bool IsValid(SalesOrder salesOrder, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!string.IsNullOrEmpty(salesOrder.CurrencyID))
            {
                if (!currencyRepository.IsExist(salesOrder.CurrencyID, out Currency currency))
                {
                    errorMessage = "Invalid currency";
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(salesOrder.DivisionID))
            {
                if (!divisionRepository.IsExist(salesOrder.DivisionID))
                {
                    errorMessage = "Invalid division";
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(salesOrder.PaymentTermCode))
            {
                if (!paymentTermRepository.IsExist(salesOrder.PaymentTermCode, out PaymentTerm paymentTerm))
                {
                    errorMessage = "Invalid payment term";
                    return false;
                }
                salesOrder.PaymentTermDescription = paymentTerm.Description;
            }

            if (!string.IsNullOrEmpty(salesOrder.PriceTermCode))
            {
                if (!priceTermRepository.IsExist(salesOrder.PriceTermCode, out PriceTerm priceTerm))
                {
                    errorMessage = "Invalid price term";
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(salesOrder.CustomerID))
            {
                if (!customerRepository.IsExist(salesOrder.CustomerID, out Customer customer))
                {
                    errorMessage = "Invalid customer";
                    return false;
                }
                salesOrder.CustomerName = customer.Name;
            }

            return true;
        }
    }
}
