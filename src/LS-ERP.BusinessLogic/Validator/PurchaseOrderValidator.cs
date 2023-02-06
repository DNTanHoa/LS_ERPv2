using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Validator
{
    public class PurchaseOrderValidator
    {
        private readonly ICustomerRepository customerRepository;
        private readonly IVendorRepository vendorRepository;
        private readonly ICurrencyRepository currencyRepository;

        public PurchaseOrderValidator(ICustomerRepository customerRepository,
            IVendorRepository vendorRepository,
            ICurrencyRepository currencyRepository)
        {
            this.customerRepository = customerRepository;
            this.vendorRepository = vendorRepository;
            this.currencyRepository = currencyRepository;
        }

        public bool IsValid(PurchaseOrder purchaseOrder, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!string.IsNullOrEmpty(purchaseOrder.CustomerID))
            {
                if (!customerRepository.IsExist(purchaseOrder.CustomerID))
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

            if (!string.IsNullOrEmpty(purchaseOrder.VendorID))
            {
                if (!vendorRepository.IsExist(purchaseOrder.VendorID))
                {
                    errorMessage = "Invalid vendor input";
                    return false;
                }
            }
            else
            {
                errorMessage = "Vendor is required";
                return false;
            }

            if (!string.IsNullOrEmpty(purchaseOrder.CurrencyID))
            {
                if (!currencyRepository.IsExist(purchaseOrder.CurrencyID))
                {
                    errorMessage = "Invalid currency input";
                    return false;
                }
            }

            return true;
        }

        public bool CanDelete(PurchaseOrder purchaseOrder, out string errorMessage)
        {
            errorMessage = string.Empty;

            if(purchaseOrder != null)
            {
                var receiptLine = purchaseOrder.PurchaseOrderGroupLines
                    .FirstOrDefault(x => x.ReceiptQuantity != null &&
                                         x.ReceiptQuantity > 0);
                if(receiptLine != null)
                {
                    errorMessage = "Purchase order has receipt. Can't deleted";
                    return false;
                }    
            }

            return true;
        }
    }
}
