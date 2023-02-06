using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public class UpdateProductionBOMQuantityProcess
    {
        public static bool CalculateReservedQuantity(List<ReservationEntry> reservationEntries, 
            List<ProductionBOM> productionBOMs, 
            out string errorMessage)
        {
            errorMessage = string.Empty;

            foreach(var productionBOM in productionBOMs)
            {
                productionBOM.ReservedQuantity = 0;
                productionBOM.ReservedQuantity += reservationEntries?
                    .Where(x => x.ProductionBOMID == productionBOM.ID)
                    .Sum(x => x.ReservedQuantity);

                productionBOM.RemainQuantity = productionBOM.RequiredQuantity - productionBOM.ReservedQuantity;
            }

            return true;
        }
    }
}
