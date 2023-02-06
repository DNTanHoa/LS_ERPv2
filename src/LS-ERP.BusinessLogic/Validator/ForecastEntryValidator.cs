using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Validator
{
    public class ForecastEntryValidator
    {
        public bool CanDelete(ForecastEntry entry)
        {
            if (entry.IsDeActive)
            {
                return true;
            }
            else
            {
                if(entry.ForecastOveralls != null &&
                   entry.ForecastOveralls.Any())
                {
                    var forecastMaterials = entry.ForecastOveralls
                        .SelectMany(x => x.ForecastMaterials);

                    var purchasedForecastMaterial = forecastMaterials
                        .FirstOrDefault(x => x.ReservedQuantity > 0);

                    return purchasedForecastMaterial != null ? false : true;
                }
            }

            return false;
        }
    }
}
