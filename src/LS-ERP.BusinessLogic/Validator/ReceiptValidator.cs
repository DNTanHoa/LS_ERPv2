using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Validator
{
    public class ReceiptValidator
    {
        private readonly IPurchaseOrderRepository purchaseOrderRepository;
        private readonly IVendorRepository vendorRepository;
        private readonly ICustomerRepository customerRepository;

        public ReceiptValidator(IPurchaseOrderRepository purchaseOrderRepository,
            IVendorRepository vendorRepository,
            ICustomerRepository customerRepository)
        {
            this.purchaseOrderRepository = purchaseOrderRepository;
            this.vendorRepository = vendorRepository;
            this.customerRepository = customerRepository;
        }

        public bool IsValid(Receipt receipt, out string errorMessage)
        {
            errorMessage = string.Empty;



            return true;
        }
    }
}
