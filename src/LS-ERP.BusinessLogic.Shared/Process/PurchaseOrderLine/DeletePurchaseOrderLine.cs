using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public class DeletePurchaseOrderLine
    {
        public static void DeleteOne(List<ProductionBOM> productionBOMs,
            List<ReservationEntry> reservationEntries, out List<ProductionBOM> updateProductionBOMs)
        {
            foreach(var reservationEntry in reservationEntries)
            {
                var productionBOM = productionBOMs
                    .FirstOrDefault(x => x.ID == reservationEntry.ProductionBOMID);
                
                if(productionBOM != null)
                {
                    productionBOM.ReservedQuantity -= reservationEntry.ReservedQuantity;
                    productionBOM.RemainQuantity += reservationEntry.ReservedQuantity;
                }
            }

            updateProductionBOMs = productionBOMs;
        }
    }
}
