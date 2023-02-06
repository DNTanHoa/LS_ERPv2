using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Validator
{
    public class ItemPriceValidator
    {
        private readonly IVendorRepository vendorRepository;
        private readonly ICurrencyRepository currencyRepository;
        private readonly IUnitRepository unitRepository;

        public ItemPriceValidator(IVendorRepository vendorRepository,
            ICurrencyRepository currencyRepository,
            IUnitRepository unitRepository)
        {
            this.vendorRepository = vendorRepository;
            this.currencyRepository = currencyRepository;
            this.unitRepository = unitRepository;
        }

        public bool IsValid(ItemPrice itemPrice, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!string.IsNullOrEmpty(itemPrice.PriceUnitID))
            {
                if (!unitRepository.IsExist(itemPrice.PriceUnitID))
                {
                    errorMessage = "Invalid price unit";
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(itemPrice.VendorID))
            {
                if (!vendorRepository.IsExist(itemPrice.VendorID))
                {
                    errorMessage = "Invalid vendor";
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(itemPrice.CurrencyID))
            {
                if (!currencyRepository.IsExist(itemPrice.CurrencyID))
                {
                    errorMessage = "Invalid currency";
                    return false;
                }
            }

            return true;
        }

        public bool IsValid(List<ItemPrice> itemPrices, out string errorMessage)
        {
            errorMessage = string.Empty;

            string currentCurrencyID = string.Empty;
            string currentVendorID = string.Empty;
            string currentPriceUnitID = string.Empty;

            foreach (var itemPrice in itemPrices)
            {
                if(itemPrice.CurrencyID != currentCurrencyID)
                {
                    currentCurrencyID = itemPrice.CurrencyID;
                    if (!string.IsNullOrEmpty(itemPrice.CurrencyID))
                    {
                        if (!currencyRepository.IsExist(itemPrice.CurrencyID))
                        {
                            errorMessage = "Invalid currency for item " + itemPrice.ID + " - "  + itemPrice.ItemName;
                            return false;
                        }
                    }
                }

                if(itemPrice.VendorID != currentVendorID)
                {
                    currentVendorID = itemPrice.VendorID;
                    if (!string.IsNullOrEmpty(itemPrice.VendorID) &&
                        !vendorRepository.IsExist(itemPrice.VendorID))
                    {
                        errorMessage = "Invalid vendor for item " + itemPrice.ID + " - " + itemPrice.ItemName;
                        return false;
                    }
                }

                if(itemPrice.PriceUnitID != currentPriceUnitID)
                {
                    currentPriceUnitID = itemPrice.PriceUnitID;
                    if (!string.IsNullOrEmpty(itemPrice.PriceUnitID))
                    {
                        if (!unitRepository.IsExist(itemPrice.PriceUnitID))
                        {
                            errorMessage = "Invalid price unit for item " + itemPrice.ID + " - " + itemPrice.ItemName;
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
