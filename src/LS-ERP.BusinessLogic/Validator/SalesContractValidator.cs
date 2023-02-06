using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Validator
{
    public class SalesContractValidator
    {
        private readonly ICustomerRepository customerRepository;
        private readonly IUnitRepository unitRepository;

        public SalesContractValidator(ICustomerRepository customerRepository,
            IUnitRepository unitRepository)
        {
            this.customerRepository = customerRepository;
            this.unitRepository = unitRepository;
        }

        public bool IsValid(SalesContract salesContract, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!string.IsNullOrEmpty(salesContract.CustomerID))
            {
                if (!customerRepository.IsExist(salesContract.CustomerID, out Customer customer))
                {
                    errorMessage = "Invalid customer";
                    return false;
                }
            }

            if (salesContract.ContractDetails != null &&
               salesContract.ContractDetails.Any())
            {
                string currentUnitID = string.Empty;
                foreach (var contractDetail in salesContract.ContractDetails)
                {
                    if (string.IsNullOrEmpty(contractDetail.UnitID))
                    {
                        errorMessage = "Unit is null";
                        return false;
                    }
                    else if (contractDetail.UnitID != currentUnitID)
                    {
                        currentUnitID = contractDetail.UnitID;
                        if (!unitRepository.IsExist(contractDetail.UnitID))
                        {
                            errorMessage = "Invalid unit for item code " + contractDetail.UnitID;
                            return false;
                        }
                    }

                }
            }


            return true;
        }
    }
}
