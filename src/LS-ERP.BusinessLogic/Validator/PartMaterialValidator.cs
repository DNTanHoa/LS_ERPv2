using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Validator
{
    public class PartMaterialValidator
    {
        private readonly ICurrencyRepository currencyRepository;
        private readonly IVendorRepository vendorRepository;
        private readonly IUnitRepository unitRepository;

        public PartMaterialValidator(ICurrencyRepository currencyRepository,
            IVendorRepository vendorRepository,
            IUnitRepository unitRepository)
        {
            this.currencyRepository = currencyRepository;
            this.vendorRepository = vendorRepository;
            this.unitRepository = unitRepository;
        }

        public bool IsValid(PartMaterial partMaterial, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!string.IsNullOrEmpty(partMaterial.CurrencyID))
            {
                if (currencyRepository.IsExist(partMaterial.CurrencyID))
                {
                    errorMessage = "Invalid currency";
                    return false;
                }
            }
            else
            {
                errorMessage = "Currency is null";
                return false;
            }

            if (!string.IsNullOrEmpty(partMaterial.VendorID))
            {
                if (currencyRepository.IsExist(partMaterial.VendorID))
                {
                    errorMessage = "Invalid vendor";
                    return false;
                }
            }
            else
            {
                errorMessage = "Vendor is null";
                return false;
            }

            if (!string.IsNullOrEmpty(partMaterial.PerUnitID))
            {
                if (currencyRepository.IsExist(partMaterial.PerUnitID))
                {
                    errorMessage = "Invalid per unit";
                    return false;
                }
            }
            else
            {
                errorMessage = "PerrUnit is null";
                return false;
            }

            if (!string.IsNullOrEmpty(partMaterial.PriceUnitID))
            {
                if (currencyRepository.IsExist(partMaterial.PriceUnitID))
                {
                    errorMessage = "Invalid price unit";
                    return false;
                }
            }
            else
            {
                errorMessage = "PriceUnit is null";
                return false;
            }

            return true;
        }

        public bool IsValid(List<PartMaterial> partMaterials, out string errorMessage)
        {
            errorMessage = string.Empty;

            string currentCurrencyID = string.Empty;
            string currentVendorID = string.Empty;
            string currentPerUnitID = string.Empty;
            string currentPriceUnitID = string.Empty;

            foreach(var partMaterial in partMaterials)
            {
                if (!string.IsNullOrEmpty(partMaterial.CurrencyID) &&
                    partMaterial.CurrencyID != currentCurrencyID)
                {
                    currentCurrencyID = partMaterial.CurrencyID;
                    if (!currencyRepository.IsExist(partMaterial.CurrencyID))
                    {
                        errorMessage = "Invalid currency for item code " + partMaterial.ItemID;
                        return false;
                    }
                }
                else if(string.IsNullOrEmpty(partMaterial.CurrencyID))
                {
                    errorMessage = "Currency is null for item code " + partMaterial.ItemID;
                    return false;
                }

                if (!string.IsNullOrEmpty(partMaterial.VendorID) &&
                    partMaterial.VendorID != currentVendorID)
                {
                    currentVendorID = partMaterial.VendorID;
                    if (!vendorRepository.IsExist(partMaterial.VendorID))
                    {
                        errorMessage = "Invalid vendor for item code " + partMaterial.ItemID;
                        return false;
                    }
                }
                else if(string.IsNullOrEmpty(partMaterial.CurrencyID))
                {
                    errorMessage = "Vendor is null for item code " + partMaterial.ItemID;
                    return false;
                }

                if (!string.IsNullOrEmpty(partMaterial.PerUnitID) &&
                    partMaterial.PerUnitID != currentPerUnitID)
                {
                    currentPerUnitID = partMaterial.PerUnitID;
                    if (!unitRepository.IsExist(partMaterial.PerUnitID))
                    {
                        errorMessage = "Invalid per unit for item code" + partMaterial.ItemID;
                        return false;
                    }
                }
                else if(string.IsNullOrEmpty(partMaterial.PerUnitID))
                {
                    errorMessage = "PerUnit is null for item code " + partMaterial.ItemID;
                    return false;
                }

                if (!string.IsNullOrEmpty(partMaterial.PriceUnitID))
                {
                    currentPriceUnitID = partMaterial.PriceUnitID;
                    if (!unitRepository.IsExist(partMaterial.PriceUnitID) &&
                        partMaterial.PriceUnitID != currentPriceUnitID)
                    {
                        errorMessage = "Invalid price unit" + partMaterial.ItemID;
                        return false;
                    }
                }
                else if(string.IsNullOrEmpty(partMaterial.PriceUnitID))
                {
                    errorMessage = "PriceUnit is null for item code " + partMaterial.ItemID;
                    return false;
                }
            }

            return true;
        }
    }
}
